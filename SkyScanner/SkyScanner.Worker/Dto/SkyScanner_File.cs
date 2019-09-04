using Ark.Tools.ResourceWatcher;
using Ark.Tools.ResourceWatcher.WorkerHost;
using NodaTime;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SkyScanner.SDK.Dto
{

    public abstract class SkyScanner_File : IResource<SkyScanner_FileMetadataDto>
    {
        public SkyScanner_File(SkyScanner_FileMetadataDto metadata)
        {
            Contract.Requires(metadata != null);

            Metadata = metadata;
        }

        public Instant DownloadedAt { get; set; }
        public string CheckSum { get; internal set; }
        public SkyScanner_FileMetadataDto Metadata { get; protected set; }

        string IResourceState.CheckSum => CheckSum;
        Instant IResourceState.RetrievedAt => DownloadedAt;
    }

    public class SkyScanner_File<TLAGIEFileValue> : SkyScanner_File
    {
        public SkyScanner_File(SkyScanner_FileMetadataDto metadata)
            : base(metadata)
        {
            Contract.Requires(metadata != null);
        }

        public IList<TLAGIEFileValue> ParsedData { get; set; }
    }

    public class Proxy
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }
}
