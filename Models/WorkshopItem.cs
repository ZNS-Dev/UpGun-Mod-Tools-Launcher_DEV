using Steamworks;

namespace UpGun_Mod_Tools_Launcher.Models
{
    public class WorkshopItem
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public long FileSize { get; set; }
        public PublishedFileId_t FileId { get; set; }

        public string DisplayTitle => string.IsNullOrEmpty(Title) ? "NO TITLE!" : Title;

        public string FormattedSize
        {
            get
            {
                double bytes = FileSize;
                if (bytes >= 1_000_000_000) return $"{bytes / 1_000_000_000.0:0.00} GB";
                if (bytes >= 1_000_000) return $"{bytes / 1_000_000.0:0.00} MB";
                if (bytes >= 1_000) return $"{bytes / 1_000.0:0.00} KB";
                return $"{bytes} B";
            }
        }

        /// <summary>Premier tag affiché comme badge principal dans la carte de la liste.</summary>
        public string PrimaryTag
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Tags)) return "—";
                var first = Tags.Split(',')[0].Trim();
                return string.IsNullOrEmpty(first) ? "—" : first;
            }
        }

        public override string ToString() => DisplayTitle;
    }
}
