using Arkive.SkyScanner.Constants;
using SkyScanner.SDK.Configuration;
using SkyScanner.SDK.Configuration.Interfaces;
using NodaTime;
using System;
using System.Configuration;

namespace SkyScanner.SDK.Host
{
    public static partial class SkyScannerHost
    {
        #region SkyScanner
        private static Host _configureForSKYSCANNERFromConfig(ISkyScanner_Host_Config baseCfg)
        {
            _logger.Info("Configuring host for SkyScanner");

            return new Host(baseCfg)
                .WithWriter();
        }

        private static Host _configureForSKYSCANNERFromAppSettings(Action<SkyScanner_Host_Config> configurationOverrider = null)
        {
            var baseCfg = new SkyScanner_Host_Config()
            {
                StateDbConnectionString = ConfigurationManager.ConnectionStrings[SkyScannerConstants.ConfigKeys.SkyScannerDatabase].ConnectionString
            };
            configurationOverrider?.Invoke(baseCfg);

            return _configureForSKYSCANNERFromConfig(baseCfg);
        }
        #endregion SkyScanner
    }
}
