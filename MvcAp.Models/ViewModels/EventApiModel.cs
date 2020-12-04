using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcAp.Models;

namespace MvcAp.Models
{
    public class EventApiModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string EventStart { get; set; }
        public string EventEnd { get; set; }
        public string SignStart { get; set; }
        public string SignEnd { get; set; }
        public string ViewNum { get; set; }
        public string Note { get; set; }
    }
}