using System;
using System.Linq;
using System.Web.Mvc;
using MvcStudy.Models;
using MvcStudy.Models.User;
using MvcStudy.Utils;

namespace MvcStudy.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index(string error = null)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                var allUsers = DataRepository.GetUsers(null, out error);

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                return View(allUsers);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        public ActionResult Details(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "User", new {@error = "Необходимо указать ИД"});

                string error;
                var user = DataRepository.GetUsers(null, out error).FirstOrDefault(x => x.id == id);

                if (!string.IsNullOrEmpty(error.Trim()))
                    return RedirectToAction("Index", "User", new {@error = error});

                if (user == null)
                    return RedirectToAction("Index", "User",
                        new {@error = string.Concat("Юзверь с ИД ", id, " не найден")});

                return PartialView("Details", new UserViewModel(user) {DbRepository = DataRepository});
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.Details.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var user = new UserViewModel() {DbRepository = DataRepository};
                return PartialView("CreateUser", user);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.CreateUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(
            [Bind(
                Include =
                    "id,people_name,email,id_group,takes_meal,excepts_duty,hased_pwd,ConfirmPassword,UserTakesMeal,UserExceptsDuty"
                )] UserViewModel newUser)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (string.IsNullOrEmpty(newUser.people_name))
                {
                    ModelState.AddModelError("Name", "Введите ФИО");
                    return PartialView("CreateUser", newUser);
                }

                if (string.IsNullOrEmpty(newUser.email))
                {
                    ModelState.AddModelError("Email", "Введите e-mail");
                    return PartialView("CreateUser", newUser);
                }

                if (string.IsNullOrEmpty(newUser.ConfirmPassword) ||
                    !string.Equals(newUser.ConfirmPassword, newUser.hased_pwd,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("ConfirmedPassword", "Пустой пароль, либо пароли не совпадают");
                    return PartialView("CreateUser", newUser);
                }

                var result = newUser.CreateClone();
                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.AddUser(result, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("",
                    string.Concat("Поля были некорректно заполнены, либо пользователь уже существует. ", error));
                return PartialView("CreateUser", newUser);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.CreateUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult UpdateUser(long? id, bool forProfile)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if(!forProfile&& !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new { @error = "Авторизация не выполнена" });

                if (id == null)
                    return RedirectToAction("Index", "User", new {@error = "Необходимо указать ИД"});

                string error;
                var user = DataRepository.GetUsers(null, out error).FirstOrDefault(x => x.id == id);

                if (user == null)
                    return RedirectToAction("Index", "User",
                        new {@error = string.Concat("Юзверь с ИД ", id, " не найден")});

                return PartialView("UpdateUser",
                    new UserViewModel(user) {DbRepository = DataRepository, ForProfile = forProfile});
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.UpdateUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(
            [Bind(
                Include =
                    "id,people_name,email,id_group,takes_meal,excepts_duty,hased_pwd,ConfirmPassword,UserTakesMeal,UserExceptsDuty,ForProfile"
                )] UserViewModel user)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home", new { @error = "Авторизация не выполнена" });

                if (!user.ForProfile && !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new { @error = "Авторизация не выполнена" });

                if (string.IsNullOrEmpty(user.people_name))
                {
                    ModelState.AddModelError("Name", "Введите ФИО");
                    return PartialView("UpdateUser", user);
                }

                if (string.IsNullOrEmpty(user.email))
                {
                    ModelState.AddModelError("Email", "Введите e-mail");
                    return PartialView("UpdateUser", user);
                }

                if (!string.Equals(user.ConfirmPassword, user.hased_pwd, StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("ConfirmedPassword", "Пустой пароль, либо пароли не совпадают");
                    return PartialView("UpdateUser", user);
                }

                var error = string.Empty;
                var result = user.CreateClone();

                if (ModelState.IsValid && DataRepository.UpdateUser(result, user.ForProfile, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", string.Concat("Поля были некорректно заполнены. ", error));
                return PartialView("UpdateUser", user);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.UpdateUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult DeleteUser(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "User", new {@error = "Необходимо указать ИД"});

                string error;
                var user = DataRepository.GetUsers(null, out error).FirstOrDefault(x => x.id == id);

                if (user == null)
                    return RedirectToAction("Index", "User",
                        new {@error = string.Concat("Юзверь с ИД ", id, " не найден")});

                return PartialView("DeleteUser", new UserViewModel(user) {DbRepository = DataRepository});
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.DeleteUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser([Bind(Include = "id")] UserViewModel user)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.DeleteUser(user.id, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", string.Concat("Пользователь с указанным ИД не был найден. ", error));
                return PartialView("DeleteUser",
                    new UserViewModel(DataRepository.GetUsers(user, out error)[0]) {DbRepository = DataRepository});
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.DeleteUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Auth.Logout();
                ReinitAuthentication();
                ViewData["CurrentUser"] = null;
                TempData["CurrentUser"] = null;
                PartialView("_Ok");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.Logout.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        public ActionResult UserLogin()
        {
            try
            {
                ViewData["CurrentUser"] = CurrentUser;
                return PartialView("_UserLogin", CurrentUser);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.UserLogin.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                var user = new tbl_people();
                return PartialView("Login", user);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.Login.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "id,email,hased_pwd")] tbl_people user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.email))
                {
                    ModelState.AddModelError("email", "E-mail не может быть пустым");
                    return PartialView("Login", user);
                }

                if (string.IsNullOrEmpty(user.hased_pwd))
                {
                    ModelState.AddModelError("pwd", "Пароль не может быть пустым");
                    return PartialView("Login", user);
                }

                var selected = Auth.Login(user.email, user.hased_pwd);

                if (selected != null)
                {
                    ViewData["CurrentUser"] = CurrentUser;
                    string error;

                    return DataRepository.ShowDutyAlert(CurrentUser.id, out error)
                        ? PartialView("_AlertWindow", "Поздравляем!!! Вы сегодня дежурите!")
                        : PartialView("_Ok");
                }

                ModelState.AddModelError("", "Авторизация не выполнена. Пользователь не найден");
                return PartialView("Login", user);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.Login.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            try
            {
                return PartialView(new UserViewModel() {DbRepository = DataRepository});
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.RegisterUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(
            [Bind(Include = "email,people_name,hased_pwd,ConfirmPassword")] UserViewModel newUser)
        {
            try
            {
                if (string.IsNullOrEmpty(newUser.people_name))
                {
                    ModelState.AddModelError("Name", "Введите ФИО");
                    return PartialView("RegisterUser", newUser);
                }

                if (string.IsNullOrEmpty(newUser.email))
                {
                    ModelState.AddModelError("Email", "Введите e-mail");
                    return PartialView("RegisterUser", newUser);
                }

                if (string.IsNullOrEmpty(newUser.ConfirmPassword) ||
                    !string.Equals(newUser.ConfirmPassword, newUser.hased_pwd,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    ModelState.AddModelError("ConfirmedPassword", "Пустой пароль, либо пароли не совпадают");
                    return PartialView("RegisterUser", newUser);
                }

                var result = new tbl_people
                {
                    people_name = newUser.people_name,
                    email = newUser.email,
                    hased_pwd = newUser.hased_pwd,
                    excepts_duty = false,
                    takes_meal = false,
                    id_group = 2
                };

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.AddUser(result, out error))
                {
                    var ioc =
                        DataRepository.GetUsers(null, out error)
                            .FirstOrDefault(x => x.email.Trim().ToLower() == newUser.email.Trim().ToLower())
                            .id;

                    if (!Tools.SendEmail(newUser.email, ioc, out error))
                    {
                        ModelState.AddModelError("",
                            string.Concat("Обратитесь к администратору для продолжения регистрации. ", error));
                        return PartialView("RegisterUser", newUser);
                    }

                    return PartialView("_Ok");
                }

                ModelState.AddModelError("",
                    string.Concat("Поля были некорректно заполнены, либо пользователь уже существует. ", error));
                return PartialView("RegisterUser", newUser);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] User.RegisterUser.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult SubmitRegister(long ioc)
        {
            try
            {
                string error;
                var user = DataRepository.GetUsers(new tbl_people {id = ioc}, out error).FirstOrDefault();

                if (user != null)
                {
                    user.takes_meal = true;
                    DataRepository.UpdateUser(user, false, out error);
                    Auth.Login(user.email, user.hased_pwd);
                    ViewData["CurrentUser"] = CurrentUser;
                }

                return RedirectToAction("Index", "Home",
                    new
                    {
                        @error =
                            string.IsNullOrEmpty(error.Trim())
                                ? "Регистрация завершена."
                                : string.Concat("Регистрация не завершена. ", error)
                    });
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] User.SubmitRegister.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
