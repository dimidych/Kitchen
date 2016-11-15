using System;
using System.Linq;
using MvcStudy.Utils;

namespace MvcStudy.Models
{
    public partial class KitchenRepository : IKitchenRepository
    {
        public tbl_settings GetSettings(out string error)
        {
            error = string.Empty;
            tbl_settings result;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                result = DbCtx.tbl_settings.FirstOrDefault();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }

            return result;
        }

        public bool UpdateSettings(tbl_settings setting, out string error)
        {
            error = string.Empty;

            try
            {
                if (DbCtx == null)
                    throw new Exception("Отсутствует подключение к БД");

                if (DbCtx.tbl_settings.Any())
                {
                    var settings = DbCtx.tbl_settings.FirstOrDefault();
                    settings.BaseHostAddress = setting.BaseHostAddress;
                    settings.CookieName = setting.CookieName;
                    settings.MailBodyPattern = setting.MailBodyPattern;
                    settings.MailHost = setting.MailHost;
                    settings.MailHostLogin = setting.MailHostLogin;
                    settings.MailHostPort = setting.MailHostPort;

                    if (!string.IsNullOrEmpty(settings.MailHostPwd))
                        settings.MailHostPwd = Tools.CreateCryptedStr(setting.MailHostPwd);

                    settings.MailHostUseSsl = setting.MailHostUseSsl;
                    settings.MailSubjectPattern = setting.MailSubjectPattern;
                    settings.ckey = setting.ckey;
                }
                else
                {
                    if (setting.id < 1)
                        setting.id = 1;

                    DbCtx.tbl_settings.Add(setting);
                }

                if (DbCtx.SaveChanges() <= 0)
                    throw new Exception("Не удалось изменить настройки");

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}