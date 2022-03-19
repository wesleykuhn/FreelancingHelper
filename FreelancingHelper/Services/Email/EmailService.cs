using FreelancingHelper.Enums;
using FreelancingHelper.Models;
using FreelancingHelper.Services.Settings;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FreelancingHelper.Services.Email
{
    public class EmailService : IEmailService
    {
        private string _smtpAddress = "mail.kuhnper.com.br";
        private int _smtpPort = 587;
        private string _originEmail = "noreply@kuhnper.com.br";
        private string _originEmailPassword = "Auto@2019#gc.wesley-k.com";

        private SmtpClient _smtpClient;
        private MailMessage _mailMessage;

        private ISettingsService _settingsService;
        private IEmailTemplatesService _emailTemplatesService;
        public EmailService(IEmailTemplatesService emailTemplatesService, ISettingsService settingsService)
        {
            _emailTemplatesService = emailTemplatesService;
            _settingsService = settingsService;

            _smtpAddress = _settingsService.AppConfiguration.CurSmtpAddress;
            _smtpPort = _settingsService.AppConfiguration.CurSmtpPort;
            _originEmail = _settingsService.AppConfiguration.CurOriginEmail;
            _originEmailPassword = _settingsService.AppConfiguration.CurOriginEmailPswd;
        }


        private void SetSmtpClient()
        {
            _smtpClient = new SmtpClient(_smtpAddress, _smtpPort);
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.EnableSsl = true;
            _smtpClient.Credentials = new NetworkCredential(_originEmail, _originEmailPassword);
        }

        public EmailService SetMailMessage(string to, string subject, string body)
        {
            _mailMessage = new(_originEmail, to);
            _mailMessage.Subject = subject;
            _mailMessage.Body = body;
            _mailMessage.BodyEncoding = Encoding.UTF8;
            _mailMessage.SubjectEncoding = Encoding.UTF8;
            _mailMessage.IsBodyHtml = true;

            return this;
        }

        public EmailService SetMailMessageAsWorkingTimeReport(string to, DayWork dayWorkToReport)
        {
            _mailMessage = new(_originEmail, to);
            _mailMessage.Subject = _emailTemplatesService.GetEmailTemplateSubject(EmailTemplateEnum.WorkingTimeReport, dayWorkToReport);
            _mailMessage.Body = _emailTemplatesService.GetEmailTemplateBody(EmailTemplateEnum.WorkingTimeReport, dayWorkToReport); ;
            _mailMessage.BodyEncoding = Encoding.UTF8;
            _mailMessage.SubjectEncoding = Encoding.UTF8;
            _mailMessage.IsBodyHtml = true;

            return this;
        }

        public EmailService SetMailMessageAsWorkingTimeReportList(string to, IEnumerable<DayWork> daysWorkToReport)
        {
            _mailMessage = new(_originEmail, to);
            _mailMessage.Subject = _emailTemplatesService.GetEmailTemplateSubject(EmailTemplateEnum.WorkingTimeReportList, daysWorkToReport);
            _mailMessage.Body = _emailTemplatesService.GetEmailTemplateBody(EmailTemplateEnum.WorkingTimeReportList, daysWorkToReport); ;
            _mailMessage.BodyEncoding = Encoding.UTF8;
            _mailMessage.SubjectEncoding = Encoding.UTF8;
            _mailMessage.IsBodyHtml = true;

            return this;
        }

        /// <summary>
        /// Handle all the third instantiation methods and send the e-mail.
        /// </summary>
        /// <param name="exceptionMessage">If there was an exception, the exception's message.</param>
        /// <returns>If the delivery was successful.</returns>
        public Task<bool> Send(out string exceptionMessage)
        {
            if (!_settingsService.CheckIfEmailSettingsAreSet())
                throw new Exception("Developer's exception: [Unhandled scenario] The SMTP credentials and settings were not set!");

            try
            {
                SetSmtpClient();
                _smtpClient.Send(_mailMessage);

                exceptionMessage = null;
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                return Task.FromResult(false);
            }
        }
    }
}
