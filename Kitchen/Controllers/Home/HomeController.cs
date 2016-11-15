using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcStudy.Models;
using MvcStudy.Models.Home;
using MvcStudy.Models.Menu;
using MvcStudy.Utils;

namespace MvcStudy.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string error = null, bool? getFutureMenu = null)
        {
            try
            {
                AuthStub();

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                getFutureMenu = getFutureMenu ?? false;
                var selectedMenuCollectionVm = new SelectedMenuCollectionViewModel {DbRepository = DataRepository};
                selectedMenuCollectionVm.Initialization(getFutureMenu.Value);
                return View(selectedMenuCollectionVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Home.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult AddOrUpdateAllMenuSelection()
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home",
                        new {error = "Авторизация не выполнена", getFutureMenu = false});

                var futureWeekUserMenuWm = new FutureWeekUserMenuViewModel
                {
                    DbRepository = DataRepository,
                    UseDateFilter = false,
                    DateFilter = DateTime.Now.AddDays(7).Date
                };
                futureWeekUserMenuWm.Initialization(CurrentUser.id);
                var query = System.Web.HttpContext.Current.Request.UrlReferrer;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    futureWeekUserMenuWm.IsFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                if (futureWeekUserMenuWm.ErrorList.Any())
                    return RedirectToAction("Index", "Home",
                        new
                        {
                            error = futureWeekUserMenuWm.ErrorList[0],
                            getFutureMenu = futureWeekUserMenuWm.IsFutureWeek
                        });

                return View("AddOrUpdateAllMenuSelection", futureWeekUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(
                    string.Concat("Error has occured in [HttpGet]Home.AddOrUpdateAllMenuSelection.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrUpdateAllMenuSelection(
            [Bind(Include = "NextWeekMenuForUser,UserId,ErrorList,UseDateFilter,MaxColumnCount,IsFutureWeek")] FutureWeekUserMenuViewModel futureWeekUserMenuWm)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home",
                        new {error = "Авторизация не выполнена", getFutureMenu = false});

                if (!ModelState.IsValid)
                    return View("AddOrUpdateAllMenuSelection", futureWeekUserMenuWm);

                string error;

                if (MenuUpdateActions(futureWeekUserMenuWm, out error) && string.IsNullOrEmpty(error.Trim()))
                    return RedirectToAction("Index", "Home", new {getFutureMenu = futureWeekUserMenuWm.IsFutureWeek});

                ModelState.AddModelError("", error);
                return View("AddOrUpdateAllMenuSelection", futureWeekUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(
                    string.Concat("Error has occured in [HttpPost]Home.AddOrUpdateAllMenuSelection.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult UpdateMenuSelection(DateTime selectedDate, long userId)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home",
                        new {error = "Авторизация не выполнена", getFutureMenu = false});

                var futureWeekUserMenuWm = new FutureWeekUserMenuViewModel
                {
                    DbRepository = DataRepository,
                    UseDateFilter = true,
                    DateFilter = selectedDate
                };
                futureWeekUserMenuWm.Initialization(userId);
                var query = System.Web.HttpContext.Current.Request.UrlReferrer;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    futureWeekUserMenuWm.IsFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                if (futureWeekUserMenuWm.ErrorList.Any())
                    return RedirectToAction("Index", "Home",
                        new
                        {
                            error = futureWeekUserMenuWm.ErrorList[0],
                            getFutureMenu = futureWeekUserMenuWm.IsFutureWeek
                        });

                return PartialView("UpdateMenuSelection", futureWeekUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet]Home.UpdateMenuSelection.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMenuSelection(
            [Bind(Include = "NextWeekMenuForUser,UserId,ErrorList,UseDateFilter,MaxColumnCount,IsFutureWeek")] FutureWeekUserMenuViewModel futureWeekUserMenuWm)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home",
                        new {error = "Авторизация не выполнена", getFutureMenu = false});

                if (!ModelState.IsValid)
                    return PartialView("UpdateMenuSelection", futureWeekUserMenuWm);

                string error;

                if (MenuUpdateActions(futureWeekUserMenuWm, out error) && string.IsNullOrEmpty(error.Trim()))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView("UpdateMenuSelection", futureWeekUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost]Home.UpdateMenuSelection.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        private bool MenuUpdateActions(FutureWeekUserMenuViewModel futureWeekUserMenuWm, out string error)
        {
            error = string.Empty;

            try
            {
                foreach (var futureWeekMenu in futureWeekUserMenuWm.NextWeekMenuForUser)
                {
                    var userMenuSelection = new tbl_menu_selection
                    {
                        id_people = futureWeekUserMenuWm.UserId,
                        id_menu = futureWeekMenu.id,
                        qty = (byte) (futureWeekMenu.Qty)
                    };
                    var menuSelection = DataRepository.GetMenuSelection(userMenuSelection, out error);
                    error = string.Empty;

                    if (futureWeekMenu.Selected)
                    {
                        if (menuSelection != null && menuSelection.Any())
                        {
                            if (menuSelection[0].qty == userMenuSelection.qty)
                                continue;

                            userMenuSelection.id = menuSelection[0].id;

                            if (!DataRepository.UpdateMenuSelection(userMenuSelection, out error) ||
                                !string.IsNullOrEmpty(error.Trim()))
                                break;

                            continue;
                        }

                        if (!DataRepository.AddMenuSelection(userMenuSelection, out error) ||
                            !string.IsNullOrEmpty(error.Trim()))
                            break;
                    }
                    else
                    {
                        if (menuSelection == null || !menuSelection.Any())
                            continue;

                        if (!DataRepository.DeleteMenuSelection(menuSelection[0].id, out error) ||
                            !string.IsNullOrEmpty(error.Trim()))
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(error.Trim()))
                    throw new Exception(error);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        [HttpGet]
        public FileStreamResult PrintMenuSelection(bool? getFutureMenu)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole || !ModelState.IsValid)
                    throw new Exception("Пользователь не авторизован");

                var selectedMenuVm = new SelectedMenuCollectionViewModel {DbRepository = DataRepository};
                selectedMenuVm.Initialization(getFutureMenu ?? false);

                if (selectedMenuVm == null || selectedMenuVm.MenuSelectionForWeek == null ||
                    !selectedMenuVm.MenuSelectionForWeek.Any()
                    || selectedMenuVm.WeekDays == null || !selectedMenuVm.WeekDays.Any())
                    throw new Exception("Записи не найдены");

                var ms = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);
                writer.CloseStream = false;
                document.Open();
                document.AddCreationDate();
                document.AddHeader("Они обедают в офисе", "Они обедают в офисе");
                string error;
                var baseFont = Tools.RegisterFont("verdana.ttf", out error);
                var fontHeading = new iTextSharp.text.Font(baseFont, 12.5f, iTextSharp.text.Font.BOLD);
                var fontBody = new iTextSharp.text.Font(baseFont, 9.5f);
                var paragraph = new Paragraph(2.5f,
                    string.Concat(selectedMenuVm.RecordsNumber, " людей из ", selectedMenuVm.TotalDinnerPeople,
                        " обедают в офисе с ", selectedMenuVm.WeekDays[selectedMenuVm.WeekDays.Keys.Min()],
                        " по ", selectedMenuVm.WeekDays[selectedMenuVm.WeekDays.Keys.Max()]), fontHeading);
                paragraph.SetLeading(3f, 2f);
                paragraph.Alignment = Element.ALIGN_CENTER;
                document.Add(paragraph);
                document.Add(new Paragraph(2.5f, "            "));

                float[] columnDefinitionSize = {10f, 35f, 25f, 25f, 25f, 25f, 25f, 25f, 25f};
                var table = new PdfPTable(columnDefinitionSize) {WidthPercentage = 100};
                var rowCounter = 0;
                var cell = new PdfPCell(new Phrase("№", fontHeading))
                {
                    BackgroundColor = new BaseColor(Color.Aquamarine)
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("ФИО", fontHeading))
                {
                    BackgroundColor = new BaseColor(Color.Khaki)
                };
                table.AddCell(cell);

                foreach (var date in selectedMenuVm.WeekDays.Keys)
                {
                    cell = new PdfPCell(new Phrase(selectedMenuVm.WeekDays[date], fontHeading))
                    {
                        BackgroundColor = new BaseColor(Color.LightBlue)
                    };
                    table.AddCell(cell);
                }

                foreach (var selectedMnu in selectedMenuVm.MenuSelectionForWeek)
                {
                    var cellColorMeal = new BaseColor(rowCounter%2 != 0 ? Color.Gainsboro : Color.GhostWhite);
                    var cellColorName = new BaseColor(rowCounter%2 != 0 ? Color.Khaki : Color.LightYellow);
                    var cellColorNum = new BaseColor(rowCounter%2 != 0 ? Color.Aquamarine : Color.DarkSeaGreen);
                    cell = new PdfPCell(new Phrase((rowCounter + 1).ToString(), fontBody))
                    {
                        BackgroundColor = cellColorNum
                    };
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase(selectedMnu.User.people_name, fontBody))
                    {
                        BackgroundColor = cellColorName
                    };
                    table.AddCell(cell);

                    foreach (var date in selectedMenuVm.WeekDays.Keys)
                    {
                        var selectedMeal = "Меню не назначено";
                        var mealColl = selectedMnu.MenuSelectionGroup.Where(x => x.tbl_menu.date == date);

                        if (mealColl != null && mealColl.Any())
                        {
                            selectedMeal = string.Empty;
                            var mealCounter = 1;

                            foreach (var menu in mealColl)
                            {
                                var mealName = menu.tbl_menu.tbl_meal.meal_name;
                                mealName = mealColl.Count() > 1 ? string.Concat(mealCounter, ". ", mealName) : mealName;
                                selectedMeal = string.Concat(selectedMeal,
                                    string.IsNullOrEmpty(selectedMeal.Trim())
                                        ? string.Concat(mealName, ", к-во:", menu.qty)
                                        : string.Concat(Environment.NewLine, mealName, ", к-во:", menu.qty));
                                mealCounter++;
                            }
                        }

                        cell = new PdfPCell(new Phrase(selectedMeal, fontBody)) {BackgroundColor = cellColorMeal};
                        table.AddCell(cell);
                    }

                    rowCounter++;
                }

                document.Add(table);
                document.Close();
                var file = ms.ToArray();
                var output = new MemoryStream();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet]Home.PrintMenuSelection.", ex.Message), ex);
                return ThrowPdfError(ex.Message);
            }
        }

        [HttpGet]
        public FileStreamResult PrintSelectedMeal(bool? getFutureMenu)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole || !ModelState.IsValid)
                    throw new Exception("Пользователь не авторизован");

                var selectedMenuVm = new SelectedMenuCollectionViewModel {DbRepository = DataRepository};
                selectedMenuVm.Initialization(getFutureMenu ?? false);

                if (selectedMenuVm == null || selectedMenuVm.SelectedMealCountForWeek == null ||
                    !selectedMenuVm.SelectedMealCountForWeek.Any()
                    || selectedMenuVm.WeekDays == null || !selectedMenuVm.WeekDays.Any())
                    throw new Exception("Записи не найдены");

                var ms = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);
                writer.CloseStream = false;
                document.Open();
                document.AddCreationDate();
                document.AddHeader("Они обедают в офисе", "Общее количество заказанных блюд");
                string error;
                var baseFont = Tools.RegisterFont("verdana.ttf", out error);
                var fontHeading = new iTextSharp.text.Font(baseFont, 12.5f, iTextSharp.text.Font.BOLD);
                var fontBody = new iTextSharp.text.Font(baseFont, 9.5f);
                var paragraph = new Paragraph(2.5f,
                    string.Concat("Общее количество заказанных блюд с ",
                        selectedMenuVm.WeekDays[selectedMenuVm.WeekDays.Keys.Min()],
                        " по ", selectedMenuVm.WeekDays[selectedMenuVm.WeekDays.Keys.Max()]), fontHeading);
                paragraph.SetLeading(3f, 2f);
                paragraph.Alignment = Element.ALIGN_CENTER;
                document.Add(paragraph);
                document.Add(new Paragraph(2.5f, "            "));

                float[] columnDefinitionSize = {30f, 30f, 30f, 30f, 30f, 30f, 30f};
                var table = new PdfPTable(columnDefinitionSize) {WidthPercentage = 100};
                PdfPCell cell;

                foreach (var date in selectedMenuVm.WeekDays.Keys)
                {
                    cell = new PdfPCell(new Phrase(selectedMenuVm.WeekDays[date], fontHeading))
                    {
                        BackgroundColor = new BaseColor(Color.LightBlue)
                    };
                    table.AddCell(cell);
                }

                var totalMealCounterLst = new List<int>();

                foreach (var date in selectedMenuVm.WeekDays.Keys)
                {
                    var totalMealCounter = 0;
                    var selectedMeal = "Меню не назначено";
                    var mealColl = selectedMenuVm.SelectedMealCountForWeek.Where(x => x.Key.date == date);

                    if (mealColl != null && mealColl.Any())
                    {
                        selectedMeal = string.Empty;
                        var mealCounter = 1;

                        foreach (var mealItm in mealColl)
                        {
                            var mealName = mealItm.Key.tbl_meal.meal_name;
                            totalMealCounter += mealItm.Value;
                            mealName = mealColl.Count() > 1
                                ? string.Concat(mealCounter, ". ", mealName, " - ", mealItm.Value)
                                : string.Concat(mealName, " - ", mealItm.Value);
                            selectedMeal = string.Concat(selectedMeal,
                                string.IsNullOrEmpty(selectedMeal.Trim())
                                    ? mealName
                                    : string.Concat(Environment.NewLine, mealName));
                            mealCounter++;
                        }
                    }

                    cell = new PdfPCell(new Phrase(selectedMeal, fontBody));
                    table.AddCell(cell);
                    totalMealCounterLst.Add(totalMealCounter);
                }

                foreach (var counter in totalMealCounterLst)
                {
                    cell = new PdfPCell(new Phrase(string.Concat("Всего - ", counter), fontBody));
                    table.AddCell(cell);
                }

                document.Add(table);
                document.Close();
                var file = ms.ToArray();
                var output = new MemoryStream();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet]Home.PrintSelectedMeal.", ex.Message), ex);
                return ThrowPdfError(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ShowError(string message)
        {
            try
            {
                AuthStub();
            }
            catch
            {
            }

            return View(message);
        }

        [HttpGet]
        public ActionResult SignUnsigned()
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home",
                        new {error = "Авторизация не выполнена", getFutureMenu = false});

                var unsignedUserMenuWm = new MenuForUnsignedUserViewModel
                {
                    DbRepository = DataRepository
                };

                var query = System.Web.HttpContext.Current.Request.UrlReferrer;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    unsignedUserMenuWm.IsFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                unsignedUserMenuWm.DateFilter = unsignedUserMenuWm.IsFutureWeek
                        ? DateTime.Now.AddDays(7).Date
                        : DateTime.Now.Date;
                unsignedUserMenuWm.Initialization();

                if (unsignedUserMenuWm.ErrorList.Any())
                    return RedirectToAction("Index", "Home",
                        new
                        {
                            error = unsignedUserMenuWm.ErrorList[0],
                            getFutureMenu = unsignedUserMenuWm.IsFutureWeek
                        });

                return View("SignUnsigned", unsignedUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(
                    string.Concat("Error has occured in [HttpGet]Home.SignUnsigned.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUnsigned(
            [Bind(Include = "MenuLst,UserId,ErrorList,UnsignedPeople,MaxColumnCount,IsFutureWeek")] MenuForUnsignedUserViewModel unsignedUserMenuWm)
        {
            try
            {
                if (CurrentUser == null)
                    return RedirectToAction("Index", "Home",
                        new { error = "Авторизация не выполнена", getFutureMenu = false });

                if (!ModelState.IsValid)
                    return View("SignUnsigned", unsignedUserMenuWm);

                if(!unsignedUserMenuWm.MenuLst.Any(x=>x.Selected))
                    return RedirectToAction("Index", "Home", new { getFutureMenu = unsignedUserMenuWm.IsFutureWeek });

                var  error=string.Empty;

                foreach (var menu in unsignedUserMenuWm.MenuLst.Where(x=>x.Selected))
                {
                    var userMenuSelection=new tbl_menu_selection
                    {
                        id_menu = menu.id,
                        id_people = unsignedUserMenuWm.UserId,
                        qty = (byte)(menu.Qty)
                    };

                    if (!DataRepository.AddMenuSelection(userMenuSelection, out error) ||
                            !string.IsNullOrEmpty(error.Trim()))
                        break;
                }

                if (string.IsNullOrEmpty(error.Trim()))
                    return RedirectToAction("Index", "Home", new {getFutureMenu = unsignedUserMenuWm.IsFutureWeek});

                ModelState.AddModelError("", error);
                return View("SignUnsigned", unsignedUserMenuWm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost]Home.SignUnsigned.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
