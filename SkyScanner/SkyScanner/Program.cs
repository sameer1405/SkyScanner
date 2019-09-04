using SkyScanner.SDK.Host;
using NodaTime;
using System;

namespace SkyScanner.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            SkyScannerHost
                .ConfigureAndRunFromAppSettings();
        }
    }
}
