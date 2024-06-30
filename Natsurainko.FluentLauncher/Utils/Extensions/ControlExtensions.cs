using Microsoft.UI.Xaml.Controls;
using System;

namespace Natsurainko.FluentLauncher.Utils.Extensions;

internal static class ControlExtensions
{
    public static string GetTag(this Control control)
        => control.Tag?.ToString() ?? throw new ArgumentNullException("E006", "Tag");
}
