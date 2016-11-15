using System;
using System.Web.Mvc;
using MvcStudy.Models;

namespace MvcStudy.Controllers.Settings
{
    public class SettingsController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string error = null)
        {
            if (CurrentUser == null || !IsCurrentUserInAdminRole)
                return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

            if (error != null && !string.IsNullOrEmpty(error.Trim()))
                ModelState.AddModelError("", error);

            string err;
            var settings = DataRepository.GetSettings(out err);

            if (settings != null && string.IsNullOrEmpty(err.Trim())) 
                return View(new SettingsViewModel(settings));

            ModelState.AddModelError("", err);
            return View(new SettingsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SettingsViewModel settings)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if(string.IsNullOrEmpty(settings.BaseHostAddress))
                {
                    ModelState.AddModelError("BaseHostAddress", "Введите адрес базового хоста");
                    return View(settings);
                }

                if (string.IsNullOrEmpty(settings.MailHost))
                {
                    ModelState.AddModelError("MailHost", "Введите адрес почтового сервера");
                    return View(settings);
                }

                if (string.IsNullOrEmpty(settings.MailHostLogin))
                {
                    ModelState.AddModelError("MailHostLogin", "Введите логин для почтового сервера");
                    return View(settings);
                }

                if (string.IsNullOrEmpty(settings.MailHostPwd))
                {
                    ModelState.AddModelError("MailHostPwd", "Введите пароль для почтового сервера");
                    return View(settings);
                }

                if (!string.Equals(settings.ConfirmMailHostPwd, settings.MailHostPwd, StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("ConfirmMailHostPwd", "Пустой пароль, либо пароли не совпадают");
                    return View(settings);
                }

                if (settings.MailHostPort==null)
                {
                    ModelState.AddModelError("MailHostPwd", "Введите порт исходящих сообщений почтового сервера");
                    return View(settings);
                }

                if (string.IsNullOrEmpty(settings.MailSubjectPattern))
                {
                    ModelState.AddModelError("MailSubjectPattern", "Введите шаблон заголовка почтового сообщения");
                    return View(settings);
                }

                if (string.IsNullOrEmpty(settings.MailBodyPattern))
                {
                    ModelState.AddModelError("MailBodyPattern", "Введите шаблон тела почтового сообщения");
                    return View(settings);
                }

                var result = settings.CreateClone();
                var error=string.Empty;

                if (ModelState.IsValid && DataRepository.UpdateSettings(result, out error))
                {
                    ReinitAuthentication();
                    return View(settings);
                }

                ModelState.AddModelError("", error);
                return View(settings);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Settings.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
