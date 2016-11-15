using System.IO;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MvcStudy.Auth;
using MvcStudy.Models;
using MvcStudy.Utils;
using NLog;

namespace MvcStudy.Controllers
{
    public abstract class BaseController : Controller
    {
        private IAuthentication _authentication;

        public Logger Loggy
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        public IKitchenRepository DataRepository
        {
            get; 
            private set; 
        }

        public IAuthentication Auth
        {
            get
            {
                return _authentication ??
                       (_authentication =
                           new CustomAuthentication(ControllerContext.HttpContext.ApplicationInstance.Context, DataRepository));
            }
        }

        public tbl_people CurrentUser
        {
            get
            {
                var currUser = Auth.CurrentUser;
                ViewData["CurrentUser"] = currUser;
                return currUser;
            }
        }

        public bool IsCurrentUserInAdminRole
        {
            get
            {
                var isAdmin = Auth.IsCurrentUserInAdminRole;
                ViewData["IsCurrentUserInAdminRole"] = isAdmin;
                return isAdmin;
            }
        }

        public BaseController()
        {
            DataRepository = new KitchenRepository();
        }

        protected override void Dispose(bool isDisposed)
        {
            if (isDisposed)
                DataRepository.Dispose();

            base.Dispose(isDisposed);
        }

        internal void AuthStub()
        {
            var user = CurrentUser;
            var isAdmin = IsCurrentUserInAdminRole;
        }

        internal void ReinitAuthentication()
        {
            _authentication =
                new CustomAuthentication(ControllerContext.HttpContext.ApplicationInstance.Context, DataRepository);
        }

        [HttpGet]
        internal FileStreamResult ThrowPdfError(string message)
        {
            try
            {
                var ms = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate());
                var writer = PdfWriter.GetInstance(document, ms);
                writer.CloseStream = false;
                document.Open();
                string error;
                var baseFont = Tools.RegisterFont("verdana.ttf", out error);
                var fontHeading = new iTextSharp.text.Font(baseFont, 12.5f, iTextSharp.text.Font.BOLD);
                var paragraph = new Paragraph(2.5f, string.Concat("Непредвиденная ошибка. ", message), fontHeading);
                paragraph.Alignment = Element.ALIGN_CENTER;
                document.Add(paragraph);
                document.Close();
                var file = ms.ToArray();
                var output = new MemoryStream();
                output.Write(file, 0, file.Length);
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }
            catch
            {
                return null;
            }
        }
    }
}
