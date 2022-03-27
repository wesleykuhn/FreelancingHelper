using FreelancingHelper.Enums;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

                case EmailTemplateEnum.WorkingTimeReportList:
                    //CheckIfParamIsValid(param, typeof(IEnumerable<DayWork>));
                    var dayWorks = param as IEnumerable<DayWork>;
                    return WorkingHourReportListTemplateSubject(dayWorks.First().Started, dayWorks.Last().Finished);

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

                case EmailTemplateEnum.WorkingTimeReportList:
                    //CheckIfParamIsValid(param, typeof(IEnumerable<DayWork>));
                    return WorkingHourReportListTemplateBody(param as IEnumerable<DayWork>);

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

        private string WorkingHoursReportTemplateSubject(DateTime startedAt) =>
            _settingsService.AppConfiguration.CurLanguage.Type switch
            {
                AppAvailableLanguageEnum.English =>
                    $"{_settingsService.AppConfiguration.DevName} - Freelancing Working Time Report (" +
                    $"{startedAt.ToString("dddd yyyy-MM-dd", ConstantsAndSettings.EnUSSpecificCultureInfo)})",

                AppAvailableLanguageEnum.PortugueseBr =>
                    $"{_settingsService.AppConfiguration.DevName} - Relatório de Horas Trabalhadas em Freelancing (" +
                    $"{startedAt.ToString("dddd dd/MM/yyyy", ConstantsAndSettings.PtBRSpecificCultureInfo)})",

                _ => throw new NotImplementedException()
            };

        private string WorkingHourReportListTemplateSubject(DateTime startingDay, DateTime finishingDay) =>
            _settingsService.AppConfiguration.CurLanguage.Type switch
            {
                AppAvailableLanguageEnum.English =>
                    $"{_settingsService.AppConfiguration.DevName} - Freelancing Working Time Report (From " +
                    $"{startingDay.ToString("dddd yyyy-MM-dd", ConstantsAndSettings.EnUSSpecificCultureInfo)} To " +
                    $"{finishingDay.ToString("dddd yyyy-MM-dd", ConstantsAndSettings.EnUSSpecificCultureInfo)})",

                AppAvailableLanguageEnum.PortugueseBr =>
                    $"{_settingsService.AppConfiguration.DevName} - Relatório de Horas Trabalhadas em Freelancing (De " +
                    $"{startingDay.ToString("dddd dd/MM/yyyy", ConstantsAndSettings.PtBRSpecificCultureInfo)} à " +
                    $"{finishingDay.ToString("dddd dd/MM/yyyy", ConstantsAndSettings.PtBRSpecificCultureInfo)})",

                _ => throw new NotImplementedException()
            };

        private string WorkingHoursReportTemplateBody(DayWork dayWork)
        {
            var childOfBody = "";

            switch (_settingsService.AppConfiguration.CurLanguage.Type)
            {
                case AppAvailableLanguageEnum.English:
                    childOfBody += GetHtmlSubtitle("INTERVALS");
                    foreach (var interval in dayWork.DayWorkingTimes)
                    {
                        childOfBody += GetHtmlDottedItem($"Started: {interval.StartedAt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)}.");
                        childOfBody += GetHtmlDottedItem($"End: {interval.FinishedAt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture)}.");
                        childOfBody += GetHtmlDottedItem($"Total: {(interval.FinishedAt - interval.StartedAt):hh\\:mm\\:ss}.");
                        childOfBody += GetHtmlBreak();
                    }
                    childOfBody += GetHtmlSubtitle($"TOTAL TIME WORKED: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                case AppAvailableLanguageEnum.PortugueseBr:
                    childOfBody += GetHtmlSubtitle("INTERVALOS");
                    foreach (var interval in dayWork.DayWorkingTimes)
                    {
                        childOfBody += GetHtmlDottedItem($"Início: {interval.StartedAt:HH:mm:ss}.");
                        childOfBody += GetHtmlDottedItem($"Término: {interval.FinishedAt:HH:mm:ss}.");
                        childOfBody += GetHtmlDottedItem($"Total: {(interval.FinishedAt - interval.StartedAt):hh\\:mm\\:ss}.");
                        childOfBody += GetHtmlBreak();
                    }
                    childOfBody += GetHtmlSubtitle($"TEMPO TOTAL TRABALHADO: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                default:
                    throw new NotImplementedException();
            }
        }

        private string WorkingHourReportListTemplateBody(IEnumerable<DayWork> daysWork)
        {
            var childOfBody = "";
            double totalWorkedSeconds = 0;
            TimeSpan totalWorked;

            switch (_settingsService.AppConfiguration.CurLanguage.Type)
            {
                case AppAvailableLanguageEnum.English:
                    childOfBody += GetHtmlSubtitle("WORKED DAYS");
                    foreach (var dayWork in daysWork)
                    {
                        totalWorkedSeconds += dayWork.TotalWorkingTime.TotalSeconds;
                        childOfBody += GetHtmlDottedItem($"{dayWork.Started:yyyy-MM-dd}: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}.");
                    }
                    totalWorked = TimeSpan.FromSeconds(totalWorkedSeconds);
                    childOfBody += GetHtmlSubtitle($"Total of hours: {totalWorked.TotalHours.ToString("n2", CultureInfo.InvariantCulture)}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                case AppAvailableLanguageEnum.PortugueseBr:
                    childOfBody += GetHtmlSubtitle("DIAS TRABALHADOS");
                    foreach (var dayWork in daysWork)
                    {
                        totalWorkedSeconds += dayWork.TotalWorkingTime.TotalSeconds;
                        childOfBody += GetHtmlDottedItem($"{dayWork.Started:dd/MM/yyyy}: {dayWork.TotalWorkingTime:hh\\:mm\\:ss}.");
                    }
                    totalWorked = TimeSpan.FromSeconds(totalWorkedSeconds);
                    childOfBody += GetHtmlSubtitle($"Total de horas: {totalWorked.TotalHours:n2}");
                    return GetHtmlDefaultThemeBody(childOfBody);

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region [ HTML ELEMENTS ]

        private string GetHtmlDefaultThemeBody(string childOfBody) =>
            $"<div style='font-family: Verdana; padding: 2px 0px 5px 10px; color: #333337;'>{childOfBody}" +
            $"<br><p style='font-size: 8px;'>Sent by WK:Freelancing Helper</p></div>";

        private string GetHtmlSubtitle(string content) =>
            $"<p style='font-size: 15px; font-weight:500;'>{content}</p>";

        private string GetHtmlDottedItem(string content) =>
            $"<p style='font-size: 12px; margin-left: 10px;'>• {content}</p>";

        private string GetHtmlBreak() => "<br>";

        #endregion
    }
}
