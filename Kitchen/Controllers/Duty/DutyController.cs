using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcStudy.Models;
using MvcStudy.Models.Duty;
using MvcStudy.Utils;

namespace MvcStudy.Controllers.Duty
{
    public class DutyController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string error = null, bool? forFutureWeek = null)
        {
            try
            {
                AuthStub();

                if (error != null && !string.IsNullOrEmpty(error.Trim()))
                    ModelState.AddModelError("", error);

                forFutureWeek = forFutureWeek ?? false;
                var dutyVm = new DutyViewModel {DbRepository = DataRepository};
                dutyVm.Initialization(forFutureWeek.Value);
                return View(dutyVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Duty.Index.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult UpdateAcceptedDutyMan(long? id)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var query = System.Web.HttpContext.Current.Request.UrlReferrer;
                var isFutureWeek = false;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    isFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                if (id == null)
                    return RedirectToAction("Index", "Duty",
                        new {error = "Необходимо указать ИД", forFutureWeek = isFutureWeek});

                string error;
                var dutyRec = DataRepository.GetDutyRecords(null, out error).FirstOrDefault(x => x.id == id);

                if (dutyRec == null)
                    return RedirectToAction("Index", "Duty",
                        new
                        {
                            error = string.Concat("Запись о дежурстве с ИД ", id, " не найденa"),
                            forFutureWeek = isFutureWeek
                        });

                var dutyVm = new ChangingDutyManViewModel(dutyRec) {DbRepository = DataRepository};

                if (dutyVm.DutySupposedPeople == null || !dutyVm.DutySupposedPeople.Any())
                    return RedirectToAction("Index", "Duty",
                        new {error = "Этого дежурного совсем не на кого поменять..", forFutureWeek = isFutureWeek});

                return PartialView(dutyVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Duty.UpdateAcceptedDutyMan.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateAcceptedDutyMan(
            [Bind(Include = "id,id_people,duty_date")] ChangingDutyManViewModel dutyRecVm)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Поля были некорректно заполнены. ");
                    return PartialView(dutyRecVm);
                }

                var error = string.Empty;
                var dutyRec = dutyRecVm.CreateClone();

                if (DataRepository.UpdateDutyMan(dutyRec, out error) && string.IsNullOrEmpty(error.Trim()))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView(dutyRecVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Duty.UpdateAcceptedDutyMan.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult AcceptedDutyPeople(bool? forFutureWeek)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var dutyVm = new DutyViewModel {DbRepository = DataRepository};
                dutyVm.Initialization(forFutureWeek ?? false);
                string error;
                var query = System.Web.HttpContext.Current.Request.UrlReferrer;
                var isFutureWeek = false;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    isFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                if (!DataRepository.AcceptDutyPeople(dutyVm.DutyPeopleLst, out error) ||
                    !string.IsNullOrEmpty(error.Trim()))
                    return RedirectToAction("Index", "Duty",
                        new {error = error, forFutureWeek = isFutureWeek});

                return View("Index", dutyVm);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Duty.AcceptedDutyPeople.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpGet]
        public FileStreamResult PrintDuty(bool? forFutureWeek)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole || !ModelState.IsValid)
                    return null;

                var dutyVm = new DutyViewModel {DbRepository = DataRepository};
                dutyVm.Initialization(forFutureWeek ?? false);

                if (dutyVm.WeekDays == null || !dutyVm.WeekDays.Any() ||
                    dutyVm.DutyPeopleLst == null || !dutyVm.DutyPeopleLst.Any())
                    return null;

                var ms = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);
                writer.CloseStream = false;
                document.Open();
                string error;
                var baseFont = Tools.RegisterFont("verdana.ttf", out error);
                var fontHeading = new iTextSharp.text.Font(baseFont, 12.5f, iTextSharp.text.Font.BOLD);
                var fontBody = new iTextSharp.text.Font(baseFont, 9.5f);
                var paragraph = new Paragraph(2.5f,
                    string.Concat("Дежурства c ", dutyVm.WeekDays[dutyVm.WeekDays.Keys.Min()], " по ",
                        dutyVm.WeekDays[dutyVm.WeekDays.Keys.Max()]), fontHeading);
                paragraph.SetLeading(3f, 2f);
                paragraph.Alignment = Element.ALIGN_CENTER;
                document.Add(paragraph);
                document.Add(new Paragraph(2.5f, "            "));

                float[] columnDefinitionSize = {90f, 90f};
                var table = new PdfPTable(columnDefinitionSize);
                table.WidthPercentage = 100;
                var rowCounter = 0;
                var cell = new PdfPCell(new Phrase("Дата", fontHeading))
                {
                    BackgroundColor = new BaseColor(Color.LightBlue)
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Дежурный", fontHeading))
                {
                    BackgroundColor = new BaseColor(Color.LightBlue)
                };
                table.AddCell(cell);

                foreach (var date in dutyVm.WeekDays.Keys)
                {
                    var cellColorName = new BaseColor(rowCounter%2 != 0 ? Color.Gainsboro : Color.GhostWhite);
                    var cellColorDate = new BaseColor(rowCounter%2 != 0 ? Color.Khaki : Color.LightYellow);
                    cell = new PdfPCell(new Phrase(dutyVm.WeekDays[date], fontBody)) {BackgroundColor = cellColorDate};
                    table.AddCell(cell);

                    var dutyRec = dutyVm.DutyPeopleLst.FirstOrDefault(x => x.duty_date.Date == date.Date);
                    cell =
                        new PdfPCell(new Phrase(dutyRec == null ? "Никто не дежурит" : dutyRec.tbl_people.people_name,
                            fontBody)) {BackgroundColor = cellColorName};
                    table.AddCell(cell);

                    rowCounter++;
                }

                document.Add(table);
                document.Add(new Paragraph(2.5f, "            "));
                fontHeading.SetColor(Color.Red.R, Color.Red.G, Color.Red.B);
                paragraph = new Paragraph(2.5f,
                    "Дежурный, НЕ забывай помыть тарелку из-под салата в конце рабочего дня!",
                    fontHeading);
                paragraph.SetLeading(3f, 2f);
                paragraph.Alignment = Element.ALIGN_CENTER;
                document.Add(paragraph);

                document.Close();
                var file = ms.ToArray();
                var output = new MemoryStream();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Duty.PrintDuty.", ex.Message), ex);
                return ThrowPdfError(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult PunishDutyMan(long? dutyPeopleId)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                var query = System.Web.HttpContext.Current.Request.UrlReferrer;
                var isFutureWeek = false;

                if (query != null && !string.IsNullOrEmpty(query.Query))
                    isFutureWeek = bool.Parse(query.Query.Split('=')[1].Trim());

                if (dutyPeopleId == null)
                    return RedirectToAction("Index", "Duty",
                        new {error = "Пустой ИД наказываемого", forFutureWeek = isFutureWeek});

                string error;
                return PartialView(DataRepository.GetUsers(new tbl_people {id = dutyPeopleId.Value}, out error)[0]);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpGet] Duty.PunishDutyMan.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PunishDutyMan(tbl_people dutyPeople)
        {
            try
            {
                if (CurrentUser == null || !IsCurrentUserInAdminRole)
                    return RedirectToAction("Index", "Home", new {@error = "Авторизация не выполнена"});

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Что-то пошло не так");
                    return PartialView(dutyPeople);
                }

                var error = string.Empty;

                if (DataRepository.PunishDutyMan(dutyPeople.id, out error) && string.IsNullOrEmpty(error.Trim()))
                    return PartialView("_Ok");

                ModelState.AddModelError("", error);
                return PartialView(dutyPeople);
            }
            catch (Exception ex)
            {
                Loggy.Error(string.Concat("Error has occured in [HttpPost] Duty.PunishDutyMan.", ex.Message), ex);
                return View("ShowError", string.Empty, ex.Message);
            }
        }
    }
}
