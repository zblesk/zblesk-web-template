namespace zblesk_web.Services;

public interface IMailerService
{
    Task SendMail(string subject, string body, params string[] recipients);
}
