using System;
using System.Net.Mail;
using System.Threading.Tasks;
using log4net;

namespace VTT.libs
{
    public static class EmailHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailHelper));

        /// <summary>
        /// Gửi email nền (không chặn luồng chính). Tự đọc cấu hình từ <mailSettings> trong Web.config.
        /// Lỗi SMTP chỉ ghi log, không ném ra ngoài làm hỏng luồng Duyệt/Gửi duyệt.
        /// </summary>
        public static void SendAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail)) return;

            Task.Run(() =>
            {
                try
                {
                    using (var smtp = new SmtpClient())
                    using (var mail = new MailMessage())
                    {
                        mail.To.Add(toEmail);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = false;
                        smtp.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Lỗi gửi email tới " + toEmail + ": " + ex.Message, ex);
                }
            });
        }
    }
}