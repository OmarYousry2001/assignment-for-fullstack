using BL.GeneralService.CMS;
using BL.Abstracts;
using MailKit.Net.Smtp;
using MimeKit;
using Domains.Helpers;


namespace BL.GeneralService.CMS
{
    public class EmailService : IEmailService
    {
        #region Fields
        private readonly EmailSettings _emailSettings;
        #endregion

        #region Constructor
        public EmailService(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        #endregion

        #region Functions
        public async Task SendEmail(string email, string receiverName, string message, string subject)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, _emailSettings.EnableSsl);
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                    var bodyBuilder = new BodyBuilder()
                    {
                        HtmlBody = message,
                    };

                    var mimeMessage = new MimeMessage()
                    {
                        Body = bodyBuilder.ToMessageBody()
                    };
                    mimeMessage.From.Add(new MailboxAddress(_emailSettings.App, _emailSettings.FromEmail));
                    mimeMessage.To.Add(new MailboxAddress(receiverName, email));
                    mimeMessage.Subject = subject == null ? "No Submitted" : subject;
                    await client.SendAsync(mimeMessage);
                    client.Disconnect(true);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to send email. Please check your email settings."); 
            }
        }
        #endregion
    }
}
