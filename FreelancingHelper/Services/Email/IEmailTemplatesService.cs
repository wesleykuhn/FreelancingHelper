using FreelancingHelper.Enums;

namespace FreelancingHelper.Services.Email
{
    public interface IEmailTemplatesService
    {
        string GetEmailTemplateBody(EmailTemplateEnum emailTemplate, object param = null);
        string GetEmailTemplateSubject(EmailTemplateEnum emailTemplate, object param = null);
    }
}