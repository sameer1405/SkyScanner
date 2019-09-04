using System;
using Ark.Tools.ResourceWatcher;
using Ark.Tools.ResourceWatcher.WorkerHost;
using Arkive.SkyScanner.Constants;
using SkyScanner.SDK.Configuration.Interfaces;

namespace SkyScanner.SDK.Configuration
{
    public class SkyScanner_Host_Config : DefaultHostConfig, ISkyScanner_Host_Config
    {
        public SkyScanner_Host_Config()
        {
            WorkerName = SkyScannerConstants.ProviderName;
            Sleep = TimeSpan.FromHours(SkyScannerConstants.WorkerSleepDefault);
            DegreeOfParallelism = 1;
        }

        public string StateDbConnectionString { get; set; }
        string ISqlStateProviderConfig.DbConnectionString => StateDbConnectionString;
    }
}
