using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MvcAp.Common;
using MvcAp.Models;
using MvcAp.Models.Enums;
using MvcAp.Models.ViewModels;
using Newtonsoft.Json;

namespace MvcAp.Services
{
    public class DataService : GenericService
    {

        public Dictionary<int, string> GetPostalData()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(County)))
            {
                dic.Add((int)item, item.ToString());
            }
            return dic;
        }

        public Dictionary<int, string> GetAgeData()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)Age.二十歲以下, "20歲以下");
            dic.Add((int)Age.二十一至三十歲, "21-30歲");
            dic.Add((int)Age.三十一至四十歲, "31-40歲");
            dic.Add((int)Age.四十一至五十歲, "41-50歲");
            dic.Add((int)Age.五十一至六十歲, "51-60歲");
            dic.Add((int)Age.六十歲以上, "60歲以上");

            return dic;
        }

        public Dictionary<int, string> GetJobData()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(工作類別)))
            {
                dic.Add((int)item, item.ToString());
            }
            return dic;
        }

        public string RebuildContent(string content)
        {
            string result =
                "<html><head><link href='https://unpkg.com/ace-css/css/ace.min.css' rel='stylesheet'><link href='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/css/bootstrap.min.css' rel='stylesheet'><script src='https://ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js'></script><script src='https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.7/js/bootstrap.min.js'></script></head><body><div class='container'><div class='row'>";
            result += content;
            result += "</div></div></body></html>";
            return result;
        }
        public string AddEventButton(string content, string eventCode)
        {
            string result = content;
            result += "<div class='center'> <a href='http://59.124.223.208:1007/Announce/Sign?eventCode=";
            result += eventCode;
            result += "' class='btn btn-success'> <i class='ace - icon fa fa-arrow - right'></i> 前往報名 </a> </div>";
            return result;
        }
    }
}
