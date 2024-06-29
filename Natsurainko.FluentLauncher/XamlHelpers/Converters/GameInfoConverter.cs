using Microsoft.UI.Xaml.Data;
using Nrk.FluentCore.Management.Downloader.Data;
using Nrk.FluentCore.Management;
using System;
using System.Collections.Generic;
using Natsurainko.FluentLauncher.Utils;

namespace Natsurainko.FluentLauncher.XamlHelpers.Converters;

internal class GameInfoConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is GameInfo game)
        {
            var strings = new List<string>
            {
                LocalizationResourcesUtils.GetValue("Converters", "_" + game.Type switch
                {
                    "release" => "Release",
                    "snapshot" => "Snapshot",
                    "old_beta" => "Old Beta",
                    "old_alpha" => "Old Alpha",
                    _ => "Unknown"
                })
            };

            if (game.IsInheritedFrom)
                strings.Add(LocalizationResourcesUtils.GetValue("Converters", "_Inherited From"));

            strings.Add(game.AbsoluteVersion ?? string.Empty);

            return string.Join(" ", strings);
        }


        if (value is VersionManifestItem manifestItem)
            return LocalizationResourcesUtils.GetValue("Converters", "_" + manifestItem.Type switch
            {
                "release" => "Release",
                "snapshot" => "Snapshot",
                "old_beta" => "Old Beta",
                "old_alpha" => "Old Alpha",
                _ => "Unknown"
            });

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
