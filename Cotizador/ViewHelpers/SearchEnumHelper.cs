using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc.Html;
using System.Web.Mvc;

namespace Cotizador.ViewHelpers
{
    public static class SearchEnumHelper 
    {
        public static IList<SelectListItem> GetSelectList(Type type, Enum value)
        {
            IList<SelectListItem> list = EnumHelper.GetSelectList(type, value);

            SelectListItem defaultItem = new SelectListItem();
            defaultItem.Text = "Todos";
            defaultItem.Value = "";

            list.Insert(0, defaultItem);
            return list;
        }

        public static IList<SelectListItem> GetSelectList(Type type, Enum value, String defaultText, String defaultValue, String selectedValue)
        {
            IList<SelectListItem> list = EnumHelper.GetSelectList(type, value);

            SelectListItem defaultItem = new SelectListItem();
            defaultItem.Text = defaultText;
            defaultItem.Value = defaultValue;
            if (selectedValue.Equals(defaultValue))
            {
                defaultItem.Selected = true;
                foreach (SelectListItem sel in list)
                {
                    sel.Selected = false;
                }
            }

            list.Insert(0, defaultItem);
            return list;
        }
    }
}