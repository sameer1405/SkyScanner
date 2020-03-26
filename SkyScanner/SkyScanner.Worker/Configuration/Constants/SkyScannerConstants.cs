
namespace Arkive.SkyScanner.Constants
{
    public class SkyScannerConstants
    {
        public const string BaseLink = @"https://www.skyscanner.ie";
        public const string PostUrl = "https://www.skyscanner.ie/g/conductor/v1/fps3/search/?geo_schema=skyscanner&carrier_schema=skyscanner&response_include=query%3Bdeeplink%3Bsegment%3Bstats%3Bfqs%3Bpqs";
        public const string InitialLink = "https://www.skyscanner.ie/transport/flights/{0}/{1}/{2}/{3}/?adults=1&children=0&adultsv2=1&childrenv2=&infants=0&cabinclass=economy&rtn=1&preferdirects=false&outboundaltsenabled=false&inboundaltsenabled=false&ref=home";

        public static string[] placesTo = new string[] { "Banglore" };
        public const string ProviderName = "SkyScanner";
        public static int WorkerSleepDefault = 12;
        public const string AppName = "SkyScanner.Worker";

        public static class ConfigKeys
        {
            public const string NLogMailingList = "NLog.MailingList";
            public const string SkyScannerRecipe = "SkyScanner.Recipe";
            public const string SkyScannerDatabase = "SkyScanner.Database";
        }
    }
}
