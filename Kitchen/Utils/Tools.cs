using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcStudy.Models;
using NLog;

namespace MvcStudy.Utils
{
    internal class Tools
    {
        private const int Dim = 256;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static string CreateCryptedStr(string str)
        {
            string error;
            var setting = (new KitchenRepository()).GetSettings(out error);
            return (new Cryptography.Cryptography {Dim = Dim}.Crypt(str, setting.ckey));
        }

        public static string CreateDecryptedStr(string cryptstr)
        {
            string error;
            var setting = (new KitchenRepository()).GetSettings(out error);
            return (new Cryptography.Cryptography {Dim = Dim}.Decrypt(cryptstr, setting.ckey));
        }

        public static BaseFont RegisterFont(string fontName, out string error)
        {
            error = string.Empty;

            try
            {
                var fontFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fontName);

                if (!File.Exists(fontFile))
                {
                    error = string.Concat("Файл шрифта ", fontName, " не найден");
                    return null;
                }

                FontFactory.Register(fontFile);
                return BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            }
            catch (Exception ex)
            {
                _logger.Error("Не удалось зарегистрировать шрифт", ex);
                return null;
            }
        }

        public static bool SendEmail(string toEmailAddress, long id, out string error)
        {
            error = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(toEmailAddress.Trim()))
                    throw new Exception("Пустой e-mail");

                var setting = (new KitchenRepository()).GetSettings(out error);

                if (setting == null || !string.IsNullOrEmpty(error.Trim()))
                    throw new Exception(error);

                var link = string.Concat("<a href=\"http://", setting.BaseHostAddress, "/User/SubmitRegister/ioc=", id,
                    "\"></a>");
                var mail = new MailMessage
                {
                    From = new MailAddress(setting.MailHostLogin),
                    Subject = setting.MailSubjectPattern,
                    IsBodyHtml = true,
                    Body = string.Format(setting.MailBodyPattern, link)
                };
                mail.To.Add(toEmailAddress);
                var smtp = new SmtpClient(setting.MailHost)
                {
                    Host = setting.MailHost,
                    UseDefaultCredentials = false,
                    Port = setting.MailHostPort ?? 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials =
                        new NetworkCredential(setting.MailHostLogin, CreateDecryptedStr(setting.MailHostPwd)),
                    EnableSsl = setting.MailHostUseSsl ?? false,
                    Timeout = 30000
                };
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                error = string.Concat("Во время отправки регистрационного письма возникла ошибка. ", ex.Message);
                _logger.Error(error, ex);
                return false;
            }
        }
    }
}