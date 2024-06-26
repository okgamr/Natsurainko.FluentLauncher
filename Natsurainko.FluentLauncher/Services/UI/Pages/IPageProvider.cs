using System;
using System.Collections.Generic;

namespace Natsurainko.FluentLauncher.Services.UI.Pages;

internal interface IPageProvider
{
    IReadOnlyDictionary<string, PageDescriptor> RegisteredPages { get; }

    object GetPage(string key);

    object GetViewModel(string key);
}

public record PageDescriptor(Type PageType, Type? ViewModelType = default);
