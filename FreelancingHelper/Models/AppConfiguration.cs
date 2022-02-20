using System.Windows.Media;

namespace FreelancingHelper.Models
{
    public class AppConfiguration
    {
        public AppLanguage CurLanguage { get; set; }

        public string DevName { get; set; }
        public long CurrentSelectedHirerId { get; set; }
        public Color PrimaryColor { get; set; }

        public long HirerIdCounter { get; set; }
        public long DayWorkIdCounter { get; set; }

        public string CurSmtpAddress { get;set; }
        public int CurSmtpPort { get; set; }
        public string CurOriginEmail { get; set; }
        public string CurOriginEmailPswd { get; set; }
    }
}
