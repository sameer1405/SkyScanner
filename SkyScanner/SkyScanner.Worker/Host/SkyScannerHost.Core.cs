using Ark.Tools.Nodatime;
using Ark.Tools.ResourceWatcher.WorkerHost;
using SkyScanner.SDK.Configuration.Interfaces;
using SkyScanner.SDK.DataProvider;
using SkyScanner.SDK.DataWriter;
using SkyScanner.SDK.Dto;
using NLog;
using NodaTime;

namespace SkyScanner.SDK.Host
{
    public static partial class SkyScannerHost
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #region host class

        public class Host : WorkerHost<SkyScanner_File, SkyScanner_FileMetadataDto, SkyScannerProvider_Filter>
        {
            public Host(ISkyScanner_Host_Config config) : base(config)
            {
                this.UseDataProvider<SkyScannerProvider_Web>();

                this.UseSqlStateProvider(config);               
            }

            public Host WithWriter()
            {
                this.AppendFileProcessor<SkyScanner_Writer>();
                return this;
            }
        }
        #endregion
    }
}
