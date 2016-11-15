namespace MvcStudy.Models
{
    public class SettingsViewModel : tbl_settings
    {
        public string ConfirmMailHostPwd { get; set; }

        public SettingsViewModel()
        {
        }

        public SettingsViewModel(tbl_settings settings)
        {
            BaseHostAddress = settings.BaseHostAddress;
            MailHost = settings.MailHost;
            MailHostLogin = settings.MailHostLogin;
            MailHostPwd = settings.MailHostPwd;
            MailHostPort = settings.MailHostPort;
            MailSubjectPattern = settings.MailSubjectPattern;
            MailBodyPattern = settings.MailBodyPattern;
            CookieName = settings.CookieName;
            id = settings.id;
            ckey = settings.ckey;
            MailHostUseSsl = settings.MailHostUseSsl;
        }

        public tbl_settings CreateClone()
        {
            var result = new tbl_settings
            {
                BaseHostAddress = this.BaseHostAddress,
                MailHost = this.MailHost,
                MailHostLogin = this.MailHostLogin,
                MailHostPwd = this.MailHostPwd,
                MailHostPort = this.MailHostPort,
                MailSubjectPattern = this.MailSubjectPattern,
                MailBodyPattern = this.MailBodyPattern,
                CookieName = this.CookieName,
                id = this.id,
                ckey = this.ckey,
                MailHostUseSsl = this.MailHostUseSsl
            };
            return result;
        }
    }
}