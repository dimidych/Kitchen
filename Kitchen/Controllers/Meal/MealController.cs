using System;
using System.Linq;
using System.Web.Mvc;
using MvcStudy.Models;

namespace MvcStudy.Controllers.Meal
{
    public class MealController : BaseController
    {
        public ActionResult Index(string error = null)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                var allMeal = DataRepository.GetMeals(null, out error);

                if (!string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                return View(allMeal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Meal.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult CreateMeal()
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                return PartialView("CreateMeal", new tbl_meal());
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Meal.CreateMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMeal([Bind(Include = "id,meal_name")] tbl_meal newMeal)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

               if (string.IsNullOrEmpty(newMeal.meal_name))
               {
                  ModelState.AddModelError("Name", "Введите наименование еды");
                  return PartialView("CreateMeal", newMeal);
               }

               var error = string.Empty;

                if (ModelState.IsValid && DataRepository.AddMeal(newMeal, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView("CreateMeal", newMeal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Meal.CreateMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult DeleteMeal(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "Meal", new {@error = "Еда не определена"});

                string error;
                var meal = DataRepository.GetMeals(null, out error).FirstOrDefault(x => x.id == id);

                if (meal == null)
                    return RedirectToAction("Index", "Meal",
                        new {@error = string.Concat("Еда с ИД ", id, " не найденa. ", error)});

                return PartialView("DeleteMeal", meal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Meal.DeleteMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMeal([Bind(Include = "id")] tbl_meal meal)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.DeleteMeal(meal.id, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView("DeleteMeal", meal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Meal.DeleteMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult UpdateMeal(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (id == null)
                    return RedirectToAction("Index", "Meal", new {@error = "Необходимо указать ИД"});

                string error;
                var meal = DataRepository.GetMeals(null, out error).FirstOrDefault(x => x.id == id);

                if (meal == null || !string.IsNullOrEmpty(error.Trim()))
                    return RedirectToAction("Index", "Meal",
                        new {@error = string.Concat("Еда с ИД ", id, " не найдена")});

                return PartialView("UpdateMeal", meal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Meal.UpdateMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMeal([Bind(Include = "id,meal_name")] tbl_meal meal)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

               if (string.IsNullOrEmpty(meal.meal_name))
                {
                   ModelState.AddModelError("Name", "Введите наименование еды");
                   return PartialView("UpdateMeal", meal);
                }

                var error = string.Empty;

                if (ModelState.IsValid && DataRepository.UpdateMeal(meal, out error))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView("UpdateMeal", meal);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Meal.UpdateMeal.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
