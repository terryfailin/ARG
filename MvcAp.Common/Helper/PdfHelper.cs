using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MvcAp.Common.Helper
{
    public static class PdfHelper
    {
        private static string RenderViewToString<T>(T controller, string viewName, object model) where T : Controller
        {
            var oldController = controller.RouteData.Values["controller"].ToString();

            var oldModel = controller.ViewData.Model;

            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);

                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                //Cleanup
                controller.ViewData.Model = oldModel;
                controller.RouteData.Values["controller"] = oldController;

                return sw.GetStringBuilder().ToString();
            }
        }

        public static byte[] GeneratePdf<T>(T notificationController, string viewname, object data) where T : Controller
        {
            string folderPath = Path.Combine(Path.GetTempPath(), "PdfFiles");

            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (!di.Exists)
            {
                di.Create();
            }

            string pdfSourceDomain = Config.GetAppSetting("PdfSourceDomain");
            string viewHtml = RenderViewToString(notificationController, viewname, data);
            viewHtml = viewHtml.Replace("src=\"/", "src=\"" + pdfSourceDomain);
            viewHtml = viewHtml.Replace("href=\"/", "href=\"" + pdfSourceDomain);
            string timetick = string.Format("{0:HHmmssfff}", DateTime.Now);

            string htmlFileName = string.Format("{0}.htm", timetick);
            string htmlFilePath = Path.Combine(di.FullName, htmlFileName);
            using (StreamWriter sw = System.IO.File.CreateText(htmlFilePath))
            {
                sw.WriteLine(viewHtml);
                sw.Close();
            }

            string fileName = string.Format("{0:HHmmssfff}.pdf", DateTime.Now);
            string filePath = Path.Combine(di.FullName, fileName);

            //執行wkhtmltopdf.exe
            string str = HttpContext.Current.Server.MapPath("~/App_Data/Tool/wkhtmltopdf-0.8.3.exe");
            Process p = Process.Start(str, htmlFilePath + " " + filePath);
            p.WaitForExit();

            //把檔案讀進串流
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] file = new byte[fs.Length];
            fs.Read(file, 0, file.Length);
            fs.Close();
            return file;
        }
    }
}
