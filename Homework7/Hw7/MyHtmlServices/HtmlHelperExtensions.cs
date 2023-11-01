using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Hw7.MyHtmlServices;

public static class HtmlHelperExtensions
{
    public static IHtmlContent MyEditorForModel<TModel>(this IHtmlHelper<TModel> helper)
    {
        var modelProperties = helper.ViewData.ModelExplorer.ModelType.GetProperties();
        var model = helper.ViewData.Model;
        var htmlContent = new HtmlContentBuilder();

        foreach (var property in modelProperties)
        {
            htmlContent.AppendHtml(Div(property, model));
        }

        return htmlContent;
    }

    private static IHtmlContent Div<TModel>(PropertyInfo property, TModel model)
    {
        var htmlContent = new TagBuilder("div");
        htmlContent.InnerHtml.AppendHtml(GetLabel(property)); 

        if (!property.PropertyType.IsEnum)
        {
            htmlContent.InnerHtml.AppendHtml(GetInput(property));
        }
        else
        {
            htmlContent.InnerHtml.AppendHtml(GetSelect(property));
        }

        htmlContent.InnerHtml.AppendHtml(Validate(property, model));
        return htmlContent;
    }

    private static IHtmlContent GetLabel(PropertyInfo property)
    {
        var html = new TagBuilder("label");
        var display = property.GetCustomAttribute(typeof(DisplayAttribute)) != null
            ? property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute
            : null;
        html.InnerHtml.AppendHtml(display?.Name ?? SeparateName(property.Name));
        html.Attributes.Add("for",property.Name);
        return html;
    }

    private static IHtmlContent GetInput(PropertyInfo property)
    {
        var html = new TagBuilder("input");
        html.Attributes.Add("type",property.PropertyType == typeof(string) ? "text" : "number");
        html.Attributes.Add("id", property.Name);
        html.Attributes.Add("class", property.Name);
        return html;
    }

    private static IHtmlContent GetSelect(PropertyInfo property)
    {
        var html = new TagBuilder("select");
        var values = property.PropertyType.GetEnumValues();
        html.Attributes.Add("id",property.Name);
        foreach (var value in values)
        {
            html.InnerHtml.AppendHtml($"<option value = \"{value}\">{value}<option/>");
        }

        return html;
    }

    private static IHtmlContent Validate<TModel>(PropertyInfo property, TModel model)
    {
        var htmlContent = new TagBuilder("span");

        if (model == null)
            return htmlContent;

        var validationAttributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

        foreach (ValidationAttribute validationAttribute in validationAttributes)
        {
            if (validationAttribute.IsValid(property.GetValue(model))) continue;
            htmlContent.InnerHtml.AppendHtml(validationAttribute.ErrorMessage!); 
        }            
        return htmlContent;
    }
    
    private static string SeparateName(string name) => Regex.Replace(name,"(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
} 





























