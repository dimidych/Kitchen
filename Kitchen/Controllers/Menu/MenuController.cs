using System;
using System.Linq;
using System.Web.Mvc;
using MvcStudy.Models;
using MvcStudy.Models.Menu;

namespace MvcStudy.Controllers.Menu
{
    public class MenuController : BaseController
    {
        private string _filterDate;

        [HttpGet]
        public ActionResult Index(string filterDate = null, string error = null)
        {
            try
            {
                AuthStub();

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                filterDate = filterDate ?? DateTime.Now.ToString("dd.MM.yyyy");
                _filterDate = filterDate;
                var menuCollectionVm = new MenuCollectionViewModel {DbRepository = DataRepository};
                menuCollectionVm.Initialization(filterDate);
                return View(menuCollectionVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Menu.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult AddMenu(DateTime? date)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                date = date ?? DateTime.Now;
                var menuVm = new MenuViewModel {date = date.Value, DbRepository = DataRepository};
                menuVm.InitMealCollection();
                return PartialView("AddMenu", menuVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Menu.AddMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMenu([Bind(Include = "MealCollection,date,id_weekday")] MenuViewModel menu)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var error = string.Empty;

                if (ModelState.IsValid)
                {
                    if (menu == null || !menu.MealCollection.Any() || !menu.MealCollection.Any(x => x.Selected))
                        return PartialView("_Ok");

                    foreach (var meal in menu.MealCollection.Where(x => x.Selected))
                    {
                        var menuItm = new tbl_menu
                        {
                            id = 0,
                            id_meal = meal.id,
                            date = DateTime.Parse(menu.DateView),
                        };

                        if (!DataRepository.AddMenu(menuItm, out error) || !string.IsNullOrEmpty(error.Trim()))
                            break;
                    }

                    if (!string.IsNullOrEmpty(error.Trim()))
                    {
                        ModelState.AddModelError("", string.Concat("При составлении меню произошла ошибка. ", error));
                        return PartialView("AddMenu", menu);
                    }

                    return PartialView("_Ok");
                }

                ModelState.AddModelError("", string.Concat("При составлении меню произошла ошибка. ", error));
                return PartialView("AddMenu", menu);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Menu.AddMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult DeleteMenu(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "Menu",
                        new {error = "Необходимо выбрать блюдо в меню", filterDate = _filterDate});

                string error;
                var menu = DataRepository.GetMenus(new tbl_menu {id = id.Value}, out error).FirstOrDefault();

                if (menu == null)
                    return RedirectToAction("Index", "Menu",
                        new {error = string.Concat("Меню с ИД ", id, " не найдено"), filterDate = _filterDate});

                var menuVm = new MenuViewModel(menu) {DbRepository = DataRepository};
                return PartialView("DeleteMenu", menuVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Menu.DeleteMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMenu([Bind(Include = "id")] MenuViewModel menu)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.DeleteMenu(menu.id, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", string.Concat("При удалении меню произошла ошибка. ", error));
                return PartialView("DeleteMenu", menu);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Menu.DeleteMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult UpdateMenu(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "Menu",
                        new {error = "Необходимо выбрать блюдо в меню", filterDate = _filterDate});

                string error;
                var menu = DataRepository.GetMenus(new tbl_menu {id = id.Value}, out error).FirstOrDefault();

                if (menu == null)
                    return RedirectToAction("Index", "Menu",
                        new {error = string.Concat("Меню с ИД ", id, " не найдено"), filterDate = _filterDate});

                var menuVm = new MenuViewModel(menu) {DbRepository = DataRepository};
                menuVm.InitMealCollection();
                return PartialView("UpdateMenu", menuVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Menu.UpdateMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMenu([Bind(Include = "date,id,id_meal")] MenuViewModel menu)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.UpdateMenu(menu, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", string.Concat("При изменении блюда в меню произошла ошибка. ", error));
                return PartialView("UpdateMenu", menu);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Menu.UpdateMenu.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
