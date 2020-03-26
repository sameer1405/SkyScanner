using Ark.Tools.ResourceWatcher;
using Ark.Tools.ResourceWatcher.WorkerHost;
using SkyScanner.SDK.Dto;
using NLog;
using NodaTime;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Arkive.SkyScanner.Constants.SkyScannerConstants;
using MoreLinq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SkyScanner.SDK.DataProvider
{
    public class SkyScannerProvider_Filter
    {
    }

    public class SkyScannerProvider_Web : IResourceProvider<SkyScanner_FileMetadataDto, SkyScanner_File, SkyScannerProvider_Filter>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static TimeZoneInfo _zone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private SkyScannerParser_Web _parser = new SkyScannerParser_Web();
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private Random _rnd = new Random();

        private HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        private Regex _utId = new Regex(@"(?<=\Wutid\W\W\W)(.*)");
        private Regex _viewId = new Regex(@"(?<=\WviewId\W\W\W)(.*)");
        private Regex _filterPattern = new Regex("[,:;'\"]");

        private AsyncPolicy retrier = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(new[]
              {
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(30),
                            TimeSpan.FromMinutes(1),
                            TimeSpan.FromMinutes(1.5),
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(10),

              }, (ex, ts) =>
              {
                  _logger.Warn(ex, $"Failed download. Trying again in {ts} seconds...");
              });

        public async Task<IEnumerable<SkyScanner_FileMetadataDto>> GetMetadata(SkyScannerProvider_Filter filter, CancellationToken ctk = default)
        {
            int counter = 0;


                var metadataList = new List<SkyScanner_FileMetadataDto>();
                var range = Enumerable.Range(30, 10);
                foreach (var travelDay in range)
                {
                    var year = DateTime.Now.Year;
                    var months = new int[] {  4 };

                    foreach (var days in _getDays(year, months))
                    {
                        ////temporary should be changed
                        if (days.Day >= 15)
                        {
                            metadataList.Add(new SkyScanner_FileMetadataDto()
                            {
                                Counter = counter++,
                                FromPlace = "dub",
                                ToPlace = "bom",
                                From = days,
                                To = days.AddDays(travelDay),
                                Tz = _zone,
                                NoDays = travelDay,

                            });
                        }
                    }
                }

                //foreach (var travelDay in Enumerable.Range(12, 10))
                //{
                //    var year = DateTime.Now.AddYears(1).Year;
                //    var months = new int[] { 1 };

                //    foreach (var days in _getDays(year, months))
                //    {
                //        //temporary should be changed
                //        if (days.Day >= 13 && days.AddDays(travelDay).Month > 1)
                //        {
                //            metadataList.Add(new SkyScanner_FileMetadataDto()
                //            {
                //                Counter = counter++,
                //                FromPlace = "Dublin",
                //                ToPlace = "Mumbai",
                //                From = days,
                //                To = days.AddDays(travelDay),
                //                Tz = _zone,
                //                NoDays = travelDay
                //            });
                //        }
                //    }
                //}

                return metadataList.AsEnumerable().DistinctBy(x=>x.ResourceId);
             
        }

        public async Task<SkyScanner_File> GetResource(SkyScanner_FileMetadataDto metadata, IResourceTrackedState lastState, CancellationToken ctk = default)
        {
            var resId = string.Format("{0}_{1}_{2}_{3}", metadata.FromPlace, metadata.From.Date.ToString("dd-MM-yyyy"), metadata.ToPlace, metadata.To.Date.ToString("dd-MM-yyyy"));
            if (lastState?.ResourceId == resId && lastState?.RetryCount == 0)
                return null;

            var rawData = await retrier.ExecuteAsync(async ct =>
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    int number = _rnd.Next(45, 120);

                    //to avaoid ban
                    await Task.Delay(TimeSpan.FromSeconds(number));
                    return await _getRawData(metadata, ct);
                }
                finally
                {
                    _semaphoreSlim.Release();
                }

            }, ctk);

            if (rawData == null)
                return null;

            var checkSum = _getCheckSumMD5(rawData);
            if (lastState?.CheckSum == checkSum)
                return null;

            var parsedFile = _parser.Parse(metadata, rawData);
            parsedFile.CheckSum = checkSum;
            parsedFile.DownloadedAt = SystemClock.Instance.GetCurrentInstant();
            return parsedFile; 
        }

        private string _getCheckSumMD5(string stringData)
        {
            byte[] rawData = Encoding.ASCII.GetBytes(stringData);

            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(rawData);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        private List<DateTime> _getDays(int year, int[] months)
        {
            var finalDates = new List<DateTime>();

            foreach (var mon in months)
            {
                finalDates.AddRange(Enumerable.Range(1, DateTime.DaysInMonth(year, mon))  // Days: 1, 2 ... 31 etc.
                                .Select(day => new DateTime(year, mon, day)) // Map each day to a date
                                .ToList());// Load dates into a list
            }

            return finalDates;
        }

        private string _getPostData(SkyScanner_FileMetadataDto meta, string viewId, string utId)
        {
            var postLink = string.Format("Configuration/PostData/{0}.json", meta.ToPlace.ToLowerInvariant());
            using (StreamReader reader = new StreamReader(postLink))
            {
                var post = reader.ReadToEnd()
                    .Replace("ViewIdFromWorker", viewId).Replace("TravellerContextIdFromWorker", utId)
                    .Replace("StartDateOfJourney", meta.From.ToString("yyyy-MM-dd")).Replace("EndDateOfJourney", meta.To.ToString("yyyy-MM-dd"));

                return post;
            }
        }

        public async Task<string> _getRawData(SkyScanner_FileMetadataDto meta, CancellationToken ctk)
        {
            var initialLink = string.Format(InitialLink, meta.FromPlace, meta.ToPlace, meta.From.ToString("yyMMdd"), meta.To.ToString("yyMMdd"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");

            using (var webPage = await _httpClient.GetAsync(initialLink, ctk))
            {
                if (webPage.IsSuccessStatusCode)
                {
                    var wb = await webPage.Content.ReadAsStringAsync();
                    var viewId = _filterPattern.Replace(_viewId.Match(wb).Value, string.Empty);
                    var utId = _filterPattern.Replace(_utId.Match(wb).Value, string.Empty);
                    if (string.IsNullOrEmpty(viewId) || string.IsNullOrEmpty(utId))
                    {
                        throw new Exception($"Unable to parse viewId and utId.. Please check regex pattern from webpage {wb}");
                    }

                    var postData = _getPostData(meta, viewId, utId);
                    using (var httpContentPostData = new StringContent(postData, Encoding.UTF8, "application/json"))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, PostUrl))
                        {
                            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
                            request.Headers.Add("ContentType", "application/json");
                            request.Headers.Add("Accept", "application/json");
                            request.Headers.Add("accept-encoding", "gzip, deflate, br");
                            request.Headers.Add("accept-language", "en-US,en;q=0.9,es;q=0.8,it;q=0.7");
                            request.Headers.Add("origin", BaseLink);
                            request.Headers.Add("referer", initialLink);
                            request.Headers.Add("x-skyscanner-channelid", "website");
                            request.Headers.Add("x-skyscanner-devicedetection-ismobile", "false");
                            request.Headers.Add("x-skyscanner-devicedetection-istablet", "false");
                            request.Headers.Add("x-skyscanner-traveller-context", utId);
                            request.Headers.Add("x-skyscanner-utid", utId);
                            request.Headers.Add("x-skyscanner-viewid", viewId);
                            request.Content = httpContentPostData;

                            using (var response = await _httpClient.SendAsync(request))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContentString = await response.Content.ReadAsStringAsync();
                                    return responseContentString;
                                }
                                else
                                {
                                    throw new Exception($"Unable to download the webpage string from {PostUrl} with error {response.StatusCode}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception($"Unable to download the webpage string from {initialLink} with error {webPage.StatusCode}");
                }
            }            
        }
    }

}
