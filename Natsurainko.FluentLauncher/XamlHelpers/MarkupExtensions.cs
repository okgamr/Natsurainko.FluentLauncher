using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Natsurainko.FluentLauncher.Utils;

#nullable disable

namespace Natsurainko.FluentLauncher.XamlHelpers;

[MarkupExtensionReturnType(ReturnType = typeof(Microsoft.UI.Xaml.Controls.FontIcon))]
public class FontIcon : MarkupExtension
{
    public string Glyph { get; set; }

    protected override object ProvideValue()
    {
        return new Microsoft.UI.Xaml.Controls.FontIcon { Glyph = Glyph };
    }
}

[MarkupExtensionReturnType(ReturnType = typeof(Microsoft.UI.Xaml.Controls.ComboBox))]
public class ComboBox : MarkupExtension
{
    public string ResourceKey { get; set; }

    protected override object ProvideValue(IXamlServiceProvider serviceProvider)
    {
        var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

        return LocalizationResourcesUtils.GetItems(
        [
            .. uriContext.BaseUri.AbsolutePath.ToString()
                        .Replace("/Views/", string.Empty)
                        .Replace(".xaml", string.Empty)
                        .Split('/'),
            ResourceKey,
        ]);
    }
}