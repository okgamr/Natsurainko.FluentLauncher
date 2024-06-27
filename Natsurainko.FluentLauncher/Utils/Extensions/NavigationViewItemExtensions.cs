using Microsoft.UI.Xaml.Controls;
using System;

namespace Natsurainko.FluentLauncher.Utils.Extensions;

internal static class NavigationViewItemExtensions
{
    public static string GetTag(this NavigationViewItem item)
        => item.Tag?.ToString()?? throw new ArgumentNullException("E006", "Tag");
}
