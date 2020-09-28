using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CMSShoppingCart.Infrastructure
{
    // Displaying Movies 1 - 25 of 500
    public class PaginationTagHelper : TagHelper
    {

        /// <summary>
        ///     The current page number.
        ///     (Should start from <strike>1</strike> <see cref="PageRange"/>)
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        ///     The number of items per page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        ///     Total number of pages.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        ///     The starting index for pages; defaults to <c>1</c>.
        /// </summary>
        public int PageRange { get; set; }
        /// <summary>
        ///     Text to display fo the <em>first page</em> link.
        /// </summary>
        public string PageFirst { get; set; }
        /// <summary>
        ///     Text to display fo the <em>last page</em> link.
        /// </summary>
        public string PageLast { get; set; }
        /// <summary>
        ///     Base URL for the page links.
        /// </summary>
        public string PageTarget { get; set; }

        public override void Process(TagHelperContext _context, TagHelperOutput output)
        {
            if (PageRange == 0) {
                PageRange = 1;
            }
            if (PageCount < PageRange) {
                PageRange = PageCount;
            }
            output.TagName = "nav";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("aria-label", "Page navigation");
            output.Content.SetHtmlContent(Content());
        }

        private string buttonContent(int currentPage)
        {
            var active = currentPage == PageNumber ? "active" : "";
            return $@"
                <li class='page-item {active}'>
                    <a class='page-link'href='{PageTarget}?p={currentPage}'>
                        {currentPage}
                    </a>
                </li>
            ";
        }

        private string Content()
        {

            if (string.IsNullOrEmpty(PageFirst)) {
                PageFirst = "First";
            }

            if (string.IsNullOrEmpty(PageLast)) {
                PageLast = "Last";
            }

            var content = new StringBuilder();
            content.Append("<ul class='pagination'>");

            // Display the *first page* link on every page but the first.
            if (PageNumber != 1) {
                content.Append($@"
                    <li class='page-item'>
                        <a class='page-link' href='{PageTarget}?p={PageRange}'>
                            {PageFirst}
                        </a>
                    </li>
                ");
            }

            if (PageNumber <= PageRange) {
                for (int currentPage = 1; currentPage < 2 * PageRange + 1; currentPage++) {
                    if (currentPage < 1 || currentPage > PageCount) {
                        continue;
                    }
                    content.Append(buttonContent(currentPage));
                }
            } else if (PageNumber > PageRange && PageNumber < PageCount - PageRange) {
                for (int currentPage = PageNumber - PageRange; currentPage < PageNumber + PageRange; currentPage++) {
                    if (currentPage < 1 || currentPage > PageCount) {
                        continue;
                    }
                    content.Append(buttonContent(currentPage));
                }
            } else {
                for (int currentPage = PageCount - (2 * PageRange); currentPage < PageCount + 1; currentPage++) {
                    if (currentPage < 1 || currentPage > PageCount) {
                        continue;
                    }
                    content.Append(buttonContent(currentPage));
                }
            }

            // Display the *last page* link on every page but the last.
            if (PageNumber != PageCount) {
                content.Append($@"
                    <li class='page-item'>
                        <a class='page-link' href='{PageTarget}?p={PageCount}'>
                            {PageLast}
                        </a>
                    </li>
                ");
            }

            content.Append("</ul>");
            return content.ToString();
        }

    }
}
