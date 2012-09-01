using ImageProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageBlur.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public FileResult Image(string name)
        {
            string tempFileNameAndPath = Path.Combine(Path.GetTempPath(), name);
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFileNameAndPath);
            return base.File(fileBytes, "image/png");
        }

        [HttpPost]
        public JsonResult UploadImage()
        {
            Debug.WriteLine("Number of files: " + Request.Files.Count);
            string tempFileName = Path.GetRandomFileName();
            Request.Files[0].SaveAs(Path.Combine(Path.GetTempPath(), tempFileName));

            return Json(tempFileName);
        }


        [HttpPost]
        public JsonResult BlurSection(string fileName, int x, int x2, int y, int y2)
        {
            Debug.WriteLine("fileName: " + fileName + ", X: " + x + ", Y: " + y + ", X2: " + x2 + ", Y2: ", y2);

            Gaussian g = new Gaussian();
            Bitmap processedImage = g.FilterProcessImage(3, new System.Drawing.Bitmap(Path.Combine(Path.GetTempPath(), fileName)), x, x2, y, y2);

            ImageConverter converter = new ImageConverter();
            byte[] fileBytes = (byte[])converter.ConvertTo(processedImage, typeof(byte[]));

            string tempFileName = Path.GetRandomFileName();

            string tempFileNameAndPath = Path.Combine(Path.GetTempPath(), tempFileName);

            System.IO.File.WriteAllBytes(tempFileNameAndPath, fileBytes);

            return Json(tempFileName);
        }
    }
}
