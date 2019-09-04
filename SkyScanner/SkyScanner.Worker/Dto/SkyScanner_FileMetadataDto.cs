using Ark.Tools.ResourceWatcher;
using NodaTime;
using System;

namespace SkyScanner.SDK.Dto
{
    public class SkyScanner_FileMetadataDto : IResourceMetadata
    {
        public int Counter { get; internal set; }
        public string FromPlace { get; internal set; }
        public string ToPlace { get; internal set; }
        public DateTime From { get; internal set; }
        public DateTime To { get; internal set; }
        public TimeZoneInfo Tz { get; set; }
        public int NoDays { get; set; }

        public string ResourceId => string.Format("{0}_{1}_{2}_{3}", FromPlace, From.Date.ToString("dd-MM-yyyy"), ToPlace, To.Date.ToString("dd-MM-yyyy"));
        LocalDateTime IResourceMetadata.Modified { get; } = LocalDateTime.FromDateTime(DateTime.UtcNow);
        public object Extensions { get; set; }
    }
}
