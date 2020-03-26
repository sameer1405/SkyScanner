using Arkive.SkyScanner.Constants;
using SkyScanner.SDK.Dto;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using static SkyScanner.SDK.Configuration.Constants.SkyJson;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SkyScanner.SDK.DataProvider
{
    public class SkyScannerParser_Web
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private Regex regex = new Regex(@"\d{4}-\d{2}-\d{2}");

        public SkyScanner_File Parse(SkyScanner_FileMetadataDto metadata, string rawData)
        {
            var finalList = new List<FinalDataToGet>();

            var it = JsonConvert.DeserializeObject<Rootobject>(rawData);
            var counter = 0;
            foreach (var a in it.itineraries.ToList())
            {
                var lparsed = new List<Leg2>();

                foreach (var le in a.leg_ids.ToList())
                {
                    lparsed.AddRange(it.legs.Where(x => x.id.Contains(le)));
                }

                foreach (var b in a.pricing_options)
                {
                    foreach (var c in b.items)
                    {
                        if (c.price.amount != 0 && c.price.amount < 500)
                        {
                            var durationGoing = lparsed[0].arrival - lparsed[0].departure.Add(TimeSpan.FromHours(metadata.Tz.BaseUtcOffset.TotalHours));
                            var durationComingBack = lparsed[1].arrival.Add(TimeSpan.FromHours(metadata.Tz.BaseUtcOffset.TotalHours)) - lparsed[1].departure;

                            if (durationGoing < TimeSpan.FromHours(20) && durationComingBack < TimeSpan.FromHours(20))
                            {
                                var rating = ((999.99 - c.price.amount) + (durationGoing.TotalHours * 9.99) + (durationComingBack.TotalHours * 9.99)) / 100;
                                var datecount = _getDateCount(c.url.ToString());

                                //europe datecount.Count > 6
                                if (datecount.Count > 8)
                                {
                                    finalList.Add(new FinalDataToGet
                                    {
                                        UrlFinal = SkyScannerConstants.BaseLink + c.url,
                                        DateDeparture = lparsed[0].departure,
                                        DurationGoing = _roundUp(durationGoing.TotalHours),
                                        DateArrival = lparsed[1].arrival,
                                        DurationComingBack = _roundUp(durationComingBack.TotalHours),
                                        Rating = _roundUp(rating),
                                        AmountMoney = c.price.amount,
                                        UrlChecksum = _getCheckSumMD5(c.url + counter),
                                        legParsed = lparsed
                                    });
                                }
                                counter++;
                            }
                        }
                    }
                }
            }

            var parsedData = new SkyScanner_File<FinalDataToGet>(metadata)
            {
                ParsedData = finalList.Distinct().ToList()
            };
            return parsedData;
        }
        private List<DateTime> _getDateCount(string url)
        {
            //For india 8 count 
            //for europe 6 count
            var datecount = new List<DateTime>();

            foreach (Match m in regex.Matches(url))
            {
                if (DateTime.TryParseExact(m.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    datecount.Add(dt);
            }
            return datecount;
        }

        private double _roundUp(double val)
        {
            return Math.Round(val, 2);
        }
        private string _getCheckSumMD5(string stringData)
        {
            byte[] rawData = Encoding.ASCII.GetBytes(stringData);

            using (SHA256 shaHash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = shaHash.ComputeHash(rawData);

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

    }
}
