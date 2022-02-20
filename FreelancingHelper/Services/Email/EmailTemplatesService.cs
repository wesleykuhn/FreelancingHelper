using FreelancingHelper.Enums;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Settings;
using System;
using System.Globalization;

namespace FreelancingHelper.Services.Email
{
    public class EmailTemplatesService : IEmailTemplatesService
    {
        private ISettingsService _settingsService;
        public EmailTemplatesService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string GetEmailTemplateSubject(EmailTemplateEnum emailTemplate, object param = null)
        {
            switch (emailTemplate)
            {
                case EmailTemplateEnum.WorkingTimeReport:
                    CheckIfParamIsValid(param, typeof(DayWork));
                    return WorkingHoursReportTemplateSubject(((DayWork)param).Started);

                default:
                    throw new NotImplementedException();
            }
        }

        public string GetEmailTemplateBody(EmailTemplateEnum emailTemplate, object param = null)
        {
            switch (emailTemplate)
            {
                case EmailTemplateEnum.WorkingTimeReport:
                    CheckIfParamIsValid(param, typeof(DayWork));
                    return WorkingHoursReportTemplateBody((DayWork)param);

                default:
                    throw new NotImplementedException();
            }
        }

        private void CheckIfParamIsValid(object param, Type rightType)
        {
            if (param == null || param.GetType() != rightType)
                throw new TypeAccessException("Developer's exception: [Unhandled scenario] The parameter given in the template body wasn't a DayWork valid object!");
        }

        #region [ SUBJECTS ]

        private string WorkingHoursReportTemplateSubject(DateTime startedAt) => _settingsService.AppConfiguration.CurLanguage.Type switch
        {
            AppAvailableLanguageEnum.English =>
                $"{_settingsService.AppConfiguration.DevName} - Freelancing Working Time Report (" +
                $"{startedAt.ToString("dddd yyyy/MM/dd", ConstantsAndSettings.EnUSSpecificCultureInfo)})",

            AppAvailableLanguageEnum.PortugueseBr =>
                $"{_settingsService.AppConfiguration.DevName} - Relatório de Horas Trabalhadas em Freelancing (" +
                $"{startedAt.ToString("dddd dd-MM-yyyy", ConstantsAndSettings.PtBRSpecificCultureInfo)})",

            _ => throw new NotImplementedException()
        };

        private string WorkingHoursReportTemplateBody(DayWork dayWork)
        {
            var childOfBody = "";

            switch (_settingsService.AppConfiguration.CurLanguage.Type)
            {
                case AppAvailableLanguageEnum.English:
                    childOfBody += GetHtmlDarkThemeSubtitle("INTERVALS");
                    foreach (var interval in dayWork.DayWorkingTimes)
                    {
                        childOfBody += GetHtmlDarkThemeDottedItem($"Started: {interval.StartedAt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)}.");
                        childOfBody += GetHtmlDarkThemeDottedItem($"End: {interval.FinishedAt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)}.");
                        childOfBody += GetHtmlDarkThemeDottedItem($"Total: {(interval.FinishedAt - interval.StartedAt):hh\\:mm\\:ss}.");
                        childOfBody += GetHtmlBreak();
                    }
                    childOfBody += GetHtmlDarkThemeSubtitle($"TOTAL TIME WORKED: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                case AppAvailableLanguageEnum.PortugueseBr:
                    childOfBody += GetHtmlDarkThemeSubtitle("INTERVALOS");
                    foreach (var interval in dayWork.DayWorkingTimes)
                    {
                        childOfBody += GetHtmlDarkThemeDottedItem($"Início: {interval.StartedAt:HH:mm:ss}.");
                        childOfBody += GetHtmlDarkThemeDottedItem($"Término: {interval.FinishedAt:HH:mm:ss}.");
                        childOfBody += GetHtmlDarkThemeDottedItem($"Total: {(interval.FinishedAt - interval.StartedAt):hh\\:mm\\:ss}.");
                        childOfBody += GetHtmlBreak();
                    }
                    childOfBody += GetHtmlDarkThemeSubtitle($"TEMPO TOTAL TRABALHADO: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region [ HTML ELEMENTS ]

        private string GetHtmlDarkThemeBody(string childOfBody) =>
            $"<div style='border-radius: 5px; background-color: #333337;font-family: Verdana; padding: 2px 0px 5px 10px; color: #ebebeb;'>{childOfBody}" +
            $"<br><p style='font-size: 8px;'>Sent by WK:Freelancing Helper</p></div>";

        private string GetHtmlDefaultThemeBody(string childOfBody) =>
            $"<div style='font-family: Verdana; padding: 2px 0px 5px 10px; color: #333337;'>{childOfBody}" +
            $"<br><p style='font-size: 8px;'>Sent by WK:Freelancing Helper</p></div>";

        private string GetHtmlDarkThemeSubtitle(string content) =>
            $"<p style='font-size: 16px; font-weight:bold;'>{content}</p>";

        private string GetHtmlDarkThemeDottedItem(string content) =>
            $"<p style='font-size: 12px; margin-left: 10px;'>• {content}</p>";

        private string GetHtmlBreak() => "<br>";

        #endregion
    }
}
