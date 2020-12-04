using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace System.Web
{
    public class Pager
    {
        public const String txtPrev = "<i class=\"ace-icon fa fa-angle-double-left\"></i>";
        public const String txtNext = "<i class=\"ace-icon fa fa-angle-double-right\"></i>";
        public const String txtSign = "";


        private ViewContext viewContext;
        private readonly int pageSize;
        private readonly int currentPage;
        private readonly int totalItemCount;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
        }

        public string RenderHtml()
        {
            int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
            int pagesDisplayNumber = 10;

            var sb = new StringBuilder();
            sb.Append("<div class=\"pull-right\">");
            sb.Append("<ul id='pager' class='pagination'>");
            // TotalCount
            //sb.Append("<span totailItemCount='" + totalItemCount + "'>共" + totalItemCount + "筆</span> ");

            // Previous
            if (this.currentPage > 1)
            { sb.Append(GeneratePageLink(txtPrev, this.currentPage - 1)); }
            else
            { sb.Append("<li class=\"disabled\"><a  href=\"#\">" + txtPrev + "</a></li>"); }

            sb.Append(txtSign);

            int start = 1;
            int end = pageCount;

            if (pageCount > pagesDisplayNumber)
            {
                int middle = (int)Math.Ceiling(pagesDisplayNumber / 2d) - 1;
                int below = (this.currentPage - middle);
                int above = (this.currentPage + middle);

                if (below < 4)
                {
                    above = pagesDisplayNumber;
                    below = 1;
                }
                else if (above > (pageCount - 4))
                {
                    above = pageCount;
                    below = (pageCount - pagesDisplayNumber);
                }

                start = below;
                end = above;
            }
            if (start > 3)
            {
                sb.Append(GeneratePageLink("1", 1));
                sb.Append(GeneratePageLink("2", 2));
                sb.Append("<li class=\"disabled\"><a  href=\"#\">...</a></li>");
            }
            for (int i = start; i <= end; i++)
            {
                if (i == this.currentPage)
                { sb.AppendFormat("<li class=\"active\"><a href=\"#\" class=\"current\">{0}</a></li>", i); } //改用a
                else
                { sb.Append(GeneratePageLink(i.ToString(), i)); }
            }
            if (end < (pageCount - 3))
            {
                sb.Append("<li class=\"disabled\"><a  href=\"#\">...</a></li>");
                sb.Append(GeneratePageLink((pageCount - 1).ToString(), pageCount - 1));
                sb.Append(GeneratePageLink(pageCount.ToString(), pageCount));
            }

            sb.Append(txtSign);

            // Next
            if (this.currentPage < pageCount)
            { sb.Append(GeneratePageLink(txtNext, (this.currentPage + 1))); }
            else
            { sb.Append("<li class=\"disabled\"><a href=\"#\">" + txtNext + "</a></li>"); }
            sb.Append("</ul>");
            sb.Append("</div>");
            return sb.ToString();
        }

        private string GeneratePageLink(string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);
            pageLinkValueDictionary.Add("page", pageNumber);
            var virtualPathData = RouteTable.Routes.GetVirtualPathForArea(this.viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathData != null)
            {
                string linkFormat = "<li><a href=\"{0}\">{1}</a></li>";
                return String.Format(linkFormat, virtualPathData.VirtualPath, linkText);
            }
            else
            {
                return null;
            }
        }
    }
}