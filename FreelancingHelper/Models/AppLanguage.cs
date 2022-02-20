using FreelancingHelper.Enums;

namespace FreelancingHelper.Models
{
    public class AppLanguage
    {
        public AppAvailableLanguageEnum Type { get; set; }
        public string Name { get; set; }

        public AppLanguage() { }

        public AppLanguage(AppAvailableLanguageEnum type, string name)
        {
            Type=type;
            Name=name;
        }
    }
}
