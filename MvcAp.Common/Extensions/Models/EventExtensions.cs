using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcAp.Models
{
    public static class EventExtensions
    {
        public static string TitleImgPath(this Event vo, bool withDomain = false)
        {
            if (string.IsNullOrWhiteSpace(vo.TitleImgPath))
            {
                return string.Empty;
            }

            string folderName = ConfigurationManager.AppSettings["ImgArticle"];

            string result = "/" + folderName + vo.TitleImgPath;

            if (withDomain)
            {
                result = ConfigurationManager.AppSettings["SiteUrl"] + result;
            }

            return result;
        }
    }
}
