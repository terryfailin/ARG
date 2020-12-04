using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using MvcAp.Common.Classes;
using MvcAp.Common.Helper;
using MvcAp.Models;
using MvcAp.Models.ViewModels;

namespace System.Web.Mvc.Html
{
    public static class ViewExtensions
    {
        const string URL = "URL";
        const string ORDERBY = "ORDERBY";
        const string ORDERDIR = "ORDERDIR";
        const string PAGEINDEX = "PAGEINDEX";
        const string PAGESiZE = "PAGESIZE";
        const string SOURCEKEY = "LIST-SOURCE-";
        const string SEARCHTYPE = "SEARCHTYPE";
        const string SEARCHTEXT = "SEARCHTEXT";
        const string KEYURL = SOURCEKEY + URL;
        const string KEYORDERBY = SOURCEKEY + ORDERBY;
        const string KEYORDERDIRECTION = SOURCEKEY + ORDERDIR;
        const string KEYPAGEINDEX = SOURCEKEY + PAGEINDEX;
        const string KEYPAGESIZE = SOURCEKEY + PAGESiZE;
        const string KEYSEARCHTYPE = SOURCEKEY + SEARCHTYPE;
        const string KEYSEARCHTEXT = SOURCEKEY + SEARCHTEXT;
        public static MvcHtmlString PagerContainer(this AjaxHelper helper)
        {
            var pagedList = helper.ViewData.Model as IPagedList;
            var vd = helper.ViewData;
            string controller = (string)helper.ViewContext.RouteData.Values["controller"];
            string action = (string)helper.ViewContext.RouteData.Values["action"];
            var routeValues = new RouteValueDictionary();
            if (!String.IsNullOrEmpty(action)) routeValues.Add("action", action);
            if (!String.IsNullOrEmpty(controller)) routeValues.Add("controller", controller);

            var pager = new Pager(helper.ViewContext, pagedList.PageSize, pagedList.PageNumber, pagedList.TotalItemCount, routeValues);
            var html = pager.RenderHtml();
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SetContainer(this AjaxHelper helper)
        {
            var vd = helper.ViewData;
            string controller = (string)helper.ViewContext.RouteData.Values["controller"];
            string action = (string)helper.ViewContext.RouteData.Values["action"];
            var html = new StringBuilder();

            //URL
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYURL, "/" + controller + "/" + action));
            //OrderBy
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYORDERBY, vd[KEYORDERBY]));
            //OrderDirection
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYORDERDIRECTION, vd[KEYORDERDIRECTION]));
            //PageIndex
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYPAGEINDEX, vd[KEYPAGEINDEX]));
            //PageSize
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYPAGESIZE, vd[KEYPAGESIZE]));
            //Search
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYSEARCHTYPE, vd[KEYSEARCHTYPE]));
            html.AppendLine(string.Format("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", KEYSEARCHTEXT, vd[KEYSEARCHTEXT]));
            return MvcHtmlString.Create(html.ToString());
        }

        public static RequestArguments SetContainer(this Controller controller, string column = "ID", string orderdir = "ASC", int page = 1, int pagesize = 10)
        {
            RequestArguments args = new RequestArguments();
            args.OrderBy = controller.Request[KEYORDERBY] == null ? column : controller.Request[KEYORDERBY];
            args.OrderDirection = controller.Request[KEYORDERDIRECTION] == null ? orderdir : controller.Request[KEYORDERDIRECTION];
            args.PageIndex = controller.Request[KEYPAGEINDEX] == null ? page - 1 : int.Parse(controller.Request[KEYPAGEINDEX]);
            args.PageSize = controller.Request[KEYPAGESIZE] == null ? pagesize : int.Parse(controller.Request[KEYPAGESIZE]);

            args.SearchType = controller.Request[KEYSEARCHTYPE];
            args.SearchText = controller.Request[KEYSEARCHTEXT];

            controller.ViewData[KEYORDERBY] = args.OrderBy;
            controller.ViewData[KEYORDERDIRECTION] = args.OrderDirection;
            controller.ViewData[KEYPAGEINDEX] = args.PageIndex;
            controller.ViewData[KEYPAGESIZE] = args.PageSize;
            controller.ViewData[KEYSEARCHTYPE] = args.SearchType;
            controller.ViewData[KEYSEARCHTEXT] = args.SearchText;

            foreach (var item in controller.Request.Params.AllKeys)
            {
                if (item.StartsWith(SOURCEKEY))
                {
                    string itemVal = controller.Request[item];
                    args.Wheres.Add(item.Replace(SOURCEKEY, ""), itemVal);
                }
            }

            return args;
        }

        public static MvcHtmlString MvcApValidationSummary(this HtmlHelper html, string validationMessage = "")
        {
            string result = "";

            if (!html.ViewData.ModelState.IsValid)
            {
                foreach (var modelError in html.ViewData.ModelState.SelectMany(p => p.Value.Errors))
                {
                    string tmpStr = "";
                    if (modelError.ErrorMessage == string.Empty)
                    {
                        tmpStr = modelError.Exception.Message;
                    }
                    else
                    {
                        tmpStr = modelError.ErrorMessage;
                    }
                    result += "<span class='help-block note alert-danger'><i class='icon-warning-sign'></i> " + tmpStr + "</span>";
                }
            }

            return MvcHtmlString.Create(result);
        }

        /// <summary>
        /// 產生功能Menu
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static MvcHtmlString GenerateMenus(this HtmlHelper helper, UserData userData)
        {
            if (userData == null)
            {
                return MvcHtmlString.Create("");
            }

            string result = string.Empty;

            #region 主控台
            StringBuilder navi_Main = new StringBuilder();
            navi_Main.Append("<li class='home'>");
            navi_Main.Append("<a href='/Authorize/Index'>");
            navi_Main.Append("<i class='menu-icon fa fa-tachometer'></i>");
            navi_Main.Append("<span class='menu-text'> Profile </span>");
            navi_Main.Append("</a>");
            navi_Main.Append("<b class='arrow'></b>");
            navi_Main.Append("</li>");
            result = navi_Main.ToString();
            #endregion

            #region 活動設定
            StringBuilder navi_Event = new StringBuilder();
            navi_Event.Append("<li class='menu-Event'>");
            navi_Event.Append("<a href='/Event/Index'>");
            navi_Event.Append("<i class='menu-icon fa fa-file-text-o'></i>");
            navi_Event.Append("<span class='menu-text'> Activities </span>");
            navi_Event.Append("</a>");
            navi_Event.Append("<b class='arrow'></b>");
            navi_Event.Append("</li>");
            result += navi_Event.ToString();
            #endregion
            #region Agreement
            if (userData.RoleList.Any(p => p == "Admin" || p == "System Admin"))
            {
                StringBuilder navi_User = new StringBuilder();
                navi_User.Append("<li class='menu-Department'>");
                navi_User.Append("<a href='/Agreement/Index'>");
                navi_User.Append("<i class='menu-icon fa fa-user'></i>");
                navi_User.Append("<span class='menu-text'> Agreement </span>");
                navi_User.Append("</a>");
                navi_User.Append("<b class='arrow'></b>");
                navi_User.Append("</li>");
                result += navi_User.ToString();
            }
            #endregion

            #region User
            if (userData.RoleList.Any(p => p == "Admin" || p == "System Admin"))
            {
                StringBuilder navi_User = new StringBuilder();
                navi_User.Append("<li class='menu-User'>");
                navi_User.Append("<a href='/SysUser/Index'>");
                navi_User.Append("<i class='menu-icon fa fa-user'></i>");
                navi_User.Append("<span class='menu-text'> User </span>");
                navi_User.Append("</a>");
                navi_User.Append("<b class='arrow'></b>");
                navi_User.Append("</li>");
                result += navi_User.ToString();
            }
            #endregion
            #region Department
            if (userData.RoleList.Any(p => p == "Admin" || p == "System Admin"))
            {
                StringBuilder navi_User = new StringBuilder();
                navi_User.Append("<li class='menu-Department'>");
                navi_User.Append("<a href='/Department/Index'>");
                navi_User.Append("<i class='menu-icon fa fa-user'></i>");
                navi_User.Append("<span class='menu-text'> Department </span>");
                navi_User.Append("</a>");
                navi_User.Append("<b class='arrow'></b>");
                navi_User.Append("</li>");
                result += navi_User.ToString();
            }
            #endregion

            #region System
            if (userData.RoleList.Any(p => p == "System Admin"))
            {
                StringBuilder navi_System = new StringBuilder();
                navi_System.Append("<li class='menu-System'>");
                navi_System.Append("<a href='/System/Index'>");
                navi_System.Append("<i class='menu-icon fa fa-cog'></i>");
                navi_System.Append("<span class='menu-text'> System </span>");
                navi_System.Append("</a>");
                navi_System.Append("<b class='arrow'></b>");
                navi_System.Append("</li>");
                result += navi_System.ToString();
            }

            #endregion

            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString GenerateModal(this HtmlHelper helper, string tableName, int width = 600, int height = 600)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<div id='" + tableName + "' class=\"modal fade\" tabindex=\"-1\" data-keyboard=\"false\" data-backdrop=\"static\">");
            html.Append("<div class=\"modal-dialog\" style=\"width: " + width + "px;\">");
            html.Append("<div class=\"modal-content\" style=\"height: " + height + "px; width: " + width + "px;\">");
            html.Append("<div class=\"modal-header no-padding\">");
            html.Append("<div class=\"table-header\">");
            html.Append("<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\" style=\"z-index: 9999; position: relative; left: -8px;\">");
            html.Append("<span>&times;</span>");
            html.Append("</button>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("<div class=\"modal-body no-padding\" style=\"height: 100%; position: relative; top: -33px; \">");
            html.Append("<div class=\"loading\">Loading..Please wait</div>");
            html.Append("<iframe src=\"\" frameborder=\"0\" style=\"width: 100%; height: 100%; \"></iframe>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");
            html.Append("</div>");
            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString GenerateDDL(this HtmlHelper helper, string name, List<SelectListItem> datasource, string defaultVal = null, string style = null)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<select id='" + name + "' name='" + name + "' class=\"" + style + "\">");
            if (datasource != null)
            {
                foreach (var item in datasource)
                {
                    string selected = !string.IsNullOrEmpty(defaultVal) && item.Value == defaultVal ? "selected" : string.Empty;
                    html.Append("<option value='" + item.Value + "' " + selected + ">");
                    html.Append(item.Text);
                    html.Append("</option>");
                }
            }
            html.Append("</select>");
            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString SortColumn(this AjaxHelper helper, string sortCol, string dsiplayTitle, string classStyle = null)
        {
            var html = new StringBuilder();

            //var vd = helper.ViewData;

            html.AppendLine(string.Format("<th class=\"{0} {3}\" sort-column=\"{2}\">{1}</th>", helper.SortStyle(sortCol), dsiplayTitle, sortCol, classStyle));

            return MvcHtmlString.Create(html.ToString());
        }

        public static string SortStyle(this AjaxHelper helper, string column)
        {
            var vd = helper.ViewData;

            string result = string.Empty;

            if (vd != null && vd[KEYORDERBY] != null && vd[KEYORDERDIRECTION] != null)
            {
                if (vd[KEYORDERBY].ToString() == column)
                {
                    if (vd[KEYORDERDIRECTION].ToString() == "DESC")
                    {
                        result = "sorting_desc";
                    }
                    else
                    {
                        result = "sorting_asc";
                    }
                }
                else
                {
                    result = "sorting";
                }
            }

            return result;
        }
    }
}
