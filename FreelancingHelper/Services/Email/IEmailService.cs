using FreelancingHelper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Email
{
    public interface IEmailService
    {
        Task<bool> Send(out string exceptionMessage);
        EmailService SetMailMessage(string to, string subject, string body);
        EmailService SetMailMessageAsWorkingTimeReport(string to, DayWork dayWorkToReport);
        EmailService SetMailMessageAsWorkingTimeReportList(string to, IEnumerable<DayWork> daysWorkToReport);
    }
}