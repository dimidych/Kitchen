using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using MvcStudy.Models;
using MvcStudy.Utils;

namespace MvcStudy.Auth
{
    public class CustomAuthentication : IAuthentication
    {
        private tbl_people _currentUser;
        private readonly HttpContext _httpCtx;
        private readonly IKitchenRepository _repository;
        private readonly tbl_settings _settings;

        public tbl_people CurrentUser
        {
            get
            {
                if (_currentUser != null)
                    return _currentUser;

                var cookie = _httpCtx.Request.Cookies[_settings.CookieName];

                if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                    return null;

                var decryptedTicket = FormsAuthentication.Decrypt(cookie.Value);

                if (decryptedTicket == null)
                    return null;

                string error;
                _currentUser = _repository.GetUsers(null, out error).FirstOrDefault(x =>
                    string.Equals(x.email.Trim().ToLower(), decryptedTicket.Name.Trim().ToLower()));
                return _currentUser;
            }
        }

        public bool IsCurrentUserInAdminRole
        {
            get
            {
                string error;
                return _currentUser != null
                       &&
                       ((_currentUser.tbl_group == null
                           ? _repository.GetGroupByUser(_currentUser.id, out error).is_admin
                           : _currentUser.tbl_group.is_admin) ?? false);
            }
        }

        public CustomAuthentication(HttpContext httpCtx, IKitchenRepository repository)
        {
            _httpCtx = httpCtx;
            _repository = repository;
            string error;
            _settings = _repository.GetSettings(out error);
        }

        public tbl_people Login(string login, string password)
        {
            string error;

            try
            {
                var cryptedPwd = Tools.CreateCryptedStr(password);
                var user = _repository.GetUsers(null, out error)
                    .FirstOrDefault(
                        x =>
                            string.Equals((x.email ?? string.Empty).Trim().ToLower(),
                                (login ?? string.Empty).Trim().ToLower()) &&
                            string.Equals((x.hased_pwd ?? string.Empty).Trim(), cryptedPwd));

                if (user != null)
                    CreateCookie(login);

                return user;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        private void CreateCookie(string login)
        {
            var ticket = new FormsAuthenticationTicket(1, login, DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout), true, string.Empty);
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(_settings.CookieName)
            {
                Value = encryptedTicket,
                Expires = DateTime.Now.AddDays(30d)
            };
            _httpCtx.Response.Cookies.Set(cookie);
        }

        public void Logout()
        {
            var cookie = _httpCtx.Response.Cookies[_settings.CookieName];

            if (cookie != null)
            {
                cookie.Value = string.Empty;

                var myCookie = new HttpCookie(_settings.CookieName)
                {
                    Value = string.Empty,
                    Expires = DateTime.Now.AddDays(-33d)
                };
                _httpCtx.Response.Cookies.Add(myCookie);
            }
        }

    }
}