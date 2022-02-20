using FreelancingHelper.Models;
using System.Globalization;

namespace FreelancingHelper
{
    public sealed class ConstantsAndSettings
    {
        public const bool EnableBagNavigation = true;

        public const byte DefaultCrimsonPrimaryColorR = 220;
        public const byte DefaultCrimsonPrimaryColorG = 20;
        public const byte DefaultCrimsonPrimaryColorB = 60;

        public static readonly CultureInfo PtBRSpecificCultureInfo = CultureInfo.CreateSpecificCulture("pt-BR");
        public static readonly CultureInfo EnUSSpecificCultureInfo = CultureInfo.CreateSpecificCulture("en-US");

        public static AppLanguage[] AvailableLanguages => new AppLanguage[]
        {
            new AppLanguage(Enums.AppAvailableLanguageEnum.English, "English - US"),
            new AppLanguage(Enums.AppAvailableLanguageEnum.PortugueseBr, "Português - BR")
        };
    }
}
