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
using Newtonsoft.Json;
using MoreLinq;
using System.Net.Http;

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
        private Random _rnd = new Random();

        Policy retrier = Policy
           .Handle<Exception>()
           .WaitAndRetry(new[]
                       {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                       }, (ex, ts) =>
                       {
                           _logger.Warn(ex, $"Failed download. Trying again in {ts} seconds...");
                       });

        public Task<IEnumerable<SkyScanner_FileMetadataDto>> GetMetadata(SkyScannerProvider_Filter filter, CancellationToken ctk = default)
        {
            int counter = 0;

            return Task.Run(() =>
            {
                var metadataList = new List<SkyScanner_FileMetadataDto>();

                var year = DateTime.Now.Year;
                var months = new int[] { 12 };

                foreach (var travelDay in Enumerable.Range(16, 6))
                {
                    foreach (var days in _getDays(year, months))
                    {
                        //temporary should be changed
                        if (days.Day <20 && days.Month ==12)
                        {
                            continue;
                        }
                        metadataList.Add(new SkyScanner_FileMetadataDto()
                        {
                            Counter = counter++,
                            FromPlace = "Dublin",
                            ToPlace = "Mumbai",
                            From = days,
                            To = days.AddDays(travelDay),
                            Tz = _zone,
                            NoDays = travelDay
                        });
                        
                    }
                }

                 return metadataList.AsEnumerable().DistinctBy(x=>x.ResourceId);
            });
             
        }

        public async Task<SkyScanner_File> GetResource(SkyScanner_FileMetadataDto metadata, IResourceTrackedState lastState, CancellationToken ctk = default)
        {
            var rawData = await retrier.Execute(async() =>
            {
                return await _getRawData(metadata);
            });

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

        public async Task<string> _getRawData(SkyScanner_FileMetadataDto meta)
        {
            int number = _rnd.Next(5, 5);
            Thread.Sleep(TimeSpan.FromSeconds(number));

            var responseString = await retrier.Execute(async () =>
            {
                var postLink = string.Format("Configuration/PostData/{0}.json", meta.ToPlace.ToLowerInvariant());
                using (StreamReader reader = new StreamReader(postLink))
                {
                    var post = reader.ReadToEnd().Replace("StartDateOfJourney", meta.From.ToString("yyyy-MM-dd")).Replace("EndDateOfJourney", meta.To.ToString("yyyy-MM-dd"));
                    var referer = string.Format(Referer,meta.From.ToString("yyMMdd"), meta.To.ToString("yyMMdd"));

                    using (var httpContentPostData = new StringContent(post, Encoding.UTF8, "application/json"))
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri(BaseLink);
                        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                        httpClient.DefaultRequestHeaders.Referrer = new Uri(referer);
                        httpClient.DefaultRequestHeaders.Add("DNT", "1");
                        httpClient.DefaultRequestHeaders.Add("Origin", BaseLink);
                        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36");
                        httpClient.DefaultRequestHeaders.Add("x-skyscanner-channelid", "website");
                        httpClient.DefaultRequestHeaders.Add("x-skyscanner-devicedetection-ismobile", "false");
                        httpClient.DefaultRequestHeaders.Add("x-skyscanner-devicedetection-istablet", "false");
                        using (var request = new HttpRequestMessage(HttpMethod.Post, PostUrl))
                        {
                            request.Content = httpContentPostData;
                            using (var response = await httpClient.SendAsync(request))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContentString = await response.Content.ReadAsStringAsync();
                                    return responseContentString;
                                }
                                else
                                {
                                    _logger.Warn($"Failed download metadata for id no.");
                                    return null;
                                }
                            }
                        }
                    }
                }                
            });
            return responseString;
        }
    }
}
