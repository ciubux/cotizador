using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Models
{
    public static class Util
    {
        public static MvcHtmlString DisableIf(this MvcHtmlString htmlString, Func<bool> expression)
        {
            if (expression.Invoke())
            {
                var html = htmlString.ToString();
                const string disabled = "\"disabled\"";
                html = html.Insert(html.IndexOf(">",
                    StringComparison.Ordinal), " disabled= " + disabled);
                return new MvcHtmlString(html);
            }
            return htmlString;
        }
    }
}