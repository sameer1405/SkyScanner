using Ark.Tools.NLog;
using Arkive.SkyScanner.Constants;
using SkyScanner.SDK.Configuration;
using NodaTime;
using System;
using System.Configuration;

namespace SkyScanner.SDK.Host
{
    public static partial class SkyScannerHost
    {
        public static Host ConfigureFromAppSettings(SkyScanner_Recipe? recipe = null, Action<SkyScanner_Host_Config> configurationOverrider = null)
        {
            try
            {
                var localRecipe = ConfigurationManager.AppSettings[SkyScannerConstants.ConfigKeys.SkyScannerRecipe];
                var mailingList = ConfigurationManager.AppSettings[SkyScannerConstants.ConfigKeys.NLogMailingList];

                SkyScanner_Recipe r = SkyScanner_Recipe.SkyScanner;


                if (recipe.HasValue
                    || Enum.TryParse<SkyScanner_Recipe>(localRecipe, out r))
                {

                    if (recipe.HasValue)
                        r = recipe.Value;

                    NLogConfigurer.For(SkyScannerConstants.AppName)
                                  .WithDefaultTargetsAndRulesFromAppSettings($"Logs_{r}", "SkyScanner_Worker", mailingList)
                                  .Apply();

                    Host h = null;

                    switch (recipe.GetValueOrDefault(r))
                    {
                        case SkyScanner_Recipe.SkyScanner:
                            {
                                h = _configureForSKYSCANNERFromAppSettings(configurationOverrider);

                                break;
                            }
                        default:
                            throw new NotSupportedException($"not supported recipe {recipe}");
                    }

                    return h;
                }

                throw new InvalidOperationException("invalid recipe");
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Failed to configure");
                throw;
            }
        }

        public static void ConfigureAndRunFromAppSettings(SkyScanner_Recipe? recipe = null)
         => ConfigureFromAppSettings(recipe).RunAndBlock();
    }
}
