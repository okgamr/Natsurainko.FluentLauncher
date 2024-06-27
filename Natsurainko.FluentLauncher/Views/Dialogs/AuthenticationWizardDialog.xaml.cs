using Microsoft.UI.Xaml.Controls;
using Natsurainko.FluentLauncher.Services.Accounts;
using Natsurainko.FluentLauncher.Services.UI;
using Natsurainko.FluentLauncher.ViewModels.Dialogs;

namespace Natsurainko.FluentLauncher.Views.Dialogs;

public sealed partial class AuthenticationWizardDialog : ContentDialog
{
    public AuthenticationWizardDialog()
    {
        this.InitializeComponent();

        DataContext = new AuthenticationWizardDialogViewModel(
            App.GetService<AccountService>(),
            App.GetService<NotificationService>(),
            App.GetService<AuthenticationService>());
    }
}
