using Markdig;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Security.Claims;

namespace zblesk_web;

public static class Helpers
{
    private static MarkdownPipeline _mdPipeline;

    static Helpers()
    {
        _mdPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    public static string GetUserId(this ClaimsPrincipal userClaims) => userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static string RenderMarkdown(this string md) => string.IsNullOrWhiteSpace(md) ? "" : Markdown.ToHtml(md, _mdPipeline);

    public static string Format(this IStringLocalizer localizer, string name, params object[] args)
    {
        var origCulture = CultureInfo.CurrentCulture;
        // TODO
        //  CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = _config.Current.DefaultCultureInfo;
        var str = localizer[name, args];
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = origCulture;
        return str;
    }

    public static string FormatAndRender(this IStringLocalizer localizer, string name, params object[] args)
    {
        var origCulture = CultureInfo.CurrentCulture;
        // TODO
        //CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = Config.Current.DefaultCultureInfo;
        var str = localizer[name, args]
            .Value
            .RenderMarkdown();
        CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = origCulture;
        return str;
    }

    public static string MakeImageRelativePath(string imageFilename)
        => $"/images/{imageFilename}";
}
