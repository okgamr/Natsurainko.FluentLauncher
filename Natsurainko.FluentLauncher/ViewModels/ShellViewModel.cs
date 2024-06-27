using Natsurainko.FluentLauncher.Services.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natsurainko.FluentLauncher.ViewModels;

internal class ShellViewModel : INavigationAware
{
    public INavigationService NavigationService => _shellNavigationService;

    private readonly INavigationService _shellNavigationService;

    public bool _onNavigatedTo = false;

    public ShellViewModel(INavigationService shellNavigationService)
    {
        _shellNavigationService = shellNavigationService;
    }

    void INavigationAware.OnNavigatedTo(object? parameter)
    {/*
        if (parameter is string pageKey)
        {
            _shellNavigationService.NavigateTo(pageKey);
            _onNavigatedTo = true;
        }
        else _shellNavigationService.NavigateTo("HomePage");*/
    }
}
