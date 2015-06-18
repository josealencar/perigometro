using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
namespace System.Web.Mvc
{
    public static class MethodsExtensivos
    {
        public static System.Web.Mvc.MvcHtmlString EnumDropDownListDescriptionFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string label = null, object htmlAttributes = null)
        {
            TModel model = htmlHelper.ViewData.Model;
            TProperty property = default(TProperty);
            if (model != null)
            {
                Func<TModel, TProperty> func = expression.Compile();
                property = func(model);
            }
            TagBuilder select = new TagBuilder("select");
            if (htmlAttributes != null)
            {
                System.Reflection.PropertyInfo[] properties = htmlAttributes.GetType().GetProperties();
                foreach (System.Reflection.PropertyInfo prop in properties)
                {
                    select.MergeAttribute(prop.Name, (String)prop.GetValue(htmlAttributes, null));
                }
            }
            Type type = typeof(TProperty);
            String[] Names = type.GetEnumNames();
            if (label != null)
            {
                TagBuilder option = new TagBuilder("option");
                option.MergeAttribute("value", "0");
                option.InnerHtml = label;
                select.InnerHtml += option.ToString();
            }
            foreach (string Name in Names)
            {
                System.Reflection.MemberInfo info = type.GetMember(Name).FirstOrDefault();
                TagBuilder option = new TagBuilder("option");
                if (property != null && property.ToString().Equals(Name))
                {
                    option.MergeAttribute("selected", "selected");
                }
                option.MergeAttribute("value", ((int)Enum.Parse(typeof(TProperty), Name)).ToString());
                var texto = info.CustomAttributes
                        .Select(x => x.ConstructorArguments.Select(a => a.Value))
                        .FirstOrDefault();
                if (texto != null)
                {
                    option.SetInnerText(texto.FirstOrDefault().ToString());
                }
                else
                {
                    option.SetInnerText(Name);
                }
                select.InnerHtml += option.ToString();
            }
            if (!select.Attributes.Where(x => x.Key.ToLower().Equals("id")).Any())
            {
                select.MergeAttribute("id", type.Name);

            }
            if (!select.Attributes.Where(x => x.Key.ToLower().Equals("name")).Any())
            {
                select.MergeAttribute("name", type.Name);
            }
            return MvcHtmlString.Create(select.ToString());
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}