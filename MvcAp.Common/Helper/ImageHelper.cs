using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZXing;

namespace MvcAp.Common.Helper
{
    public static class ImageHelper
    {
        public static Image CropImageFile(Image img, int x, int y, float w, float h)
        {
            int wInt = Convert.ToInt32(Math.Floor(w));
            int hInt = Convert.ToInt32(Math.Floor(h));
            img = ResizeImg(img, 400, 222);
            if (x == 0 || y == 0 || w == 0 || h == 0)
            {
                return img;
            }

            Image cropImage = new Bitmap(wInt, hInt);
            Graphics graphics2 = Graphics.FromImage(cropImage);
            Rectangle cropRect = new Rectangle(x, y, wInt, hInt);
            graphics2.DrawImage(img, 0, 0, cropRect, GraphicsUnit.Point);
            return cropImage;
        }
        private static Image ResizeImg(Image img, int maxWidth, int maxHeight)
        {
            double ratioX = (double)maxWidth / img.Width;
            double ratioY = (double)maxHeight / img.Height;
            double ratio = Math.Max(ratioX, ratioY);

            //int newWidth = (int)(img.Width * ratio);
            //int newHeight = (int)(img.Height * ratio);
            int newWidth = maxWidth;
            int newHeight = maxHeight;

            Bitmap newImg = new Bitmap(newWidth, newHeight);
            newImg.SetResolution(72, 72);
            using (Graphics graph = Graphics.FromImage(newImg))
            {
                graph.DrawImage(img, 0, 0, newWidth, newHeight);
            }
            return newImg;
        }
        public static string GenerateId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        public static string GenerateQRcodeImg(string regData)
        {
            string savePath = "";
            BarcodeWriter bw = new BarcodeWriter();
            bw.Format = BarcodeFormat.QR_CODE;
            bw.Options.Width = 300;
            bw.Options.Height = 300;
            bw.Options.PureBarcode = false;
            try
            {
                Bitmap bitmap = bw.Write(regData);

                string moveToFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QRcodeImgs");
                DirectoryInfo diMoveTo = new DirectoryInfo(moveToFolder);
                if (!diMoveTo.Exists)
                {
                    diMoveTo.Create();
                }

                string fileName = Guid.NewGuid().ToString() + ".png";
                savePath = Path.Combine(moveToFolder, fileName);
                bitmap.Save(savePath, ImageFormat.Png);
            }
            catch (Exception e)
            {
                savePath = "";
            }
            return savePath;
        }

        public static void DeleteQRcodeImg(string savePath)
        {
            try
            {
                FileInfo fi=new FileInfo(savePath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch (Exception e)
            {                
            }
        }
    }
}
