using Microsoft.UI.Xaml.Data;
using Nrk.FluentCore.Management.Downloader.Data;
using Nrk.FluentCore.Management;
using System;
using System.Collections.Generic;
using Natsurainko.FluentLauncher.Utils;
using System.Linq;

namespace Natsurainko.FluentLauncher.XamlHelpers.Converters;

internal class GameInfoConverter : IValueConverter
{
    public bool EnableShowModLoaderType { get; set; } = false;

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is GameInfo game)
        {
            var strings = new List<string>
            {
                game.AbsoluteVersion ?? "Unknown Version",
                LocalizationResourcesUtils.GetValue("Converters", "_" + game.Type switch
                {
                    "release" => "Release",
                    "snapshot" => "Snapshot",
                    "old_beta" => "Old Beta",
                    "old_alpha" => "Old Alpha",
                    _ => "Unknown Type"
                }),
            };

            if (EnableShowModLoaderType)
                strings.AddRange(game.GetModLoaders().Select(x => $"{x.LoaderType} {x.Version}"));

            return strings;
        }

        if (value is VersionManifestItem manifestItem)
            return LocalizationResourcesUtils.GetValue("Converters", "_" + manifestItem.Type switch
            {
                "release" => "Release",
                "snapshot" => "Snapshot",
                "old_beta" => "Old Beta",
                "old_alpha" => "Old Alpha",
                _ => "Unknown Type"
            });

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
