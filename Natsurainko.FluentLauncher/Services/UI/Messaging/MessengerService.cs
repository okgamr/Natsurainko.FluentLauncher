using CommunityToolkit.Mvvm.Messaging;
using Natsurainko.FluentLauncher.Services.Accounts;
using Natsurainko.FluentLauncher.Services.Settings;

namespace Natsurainko.FluentLauncher.Services.UI.Messaging;

/// <summary>
/// 用于管理其他组件的事件订阅和全局消息传递的单例服务
/// </summary>
internal class MessengerService
{
    private readonly AccountService _accountService;
    private readonly SettingsService _settingsService;

    public MessengerService(AccountService accountService, SettingsService settingsService)
    {
        _accountService = accountService;
        _settingsService = settingsService;
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    public void SubscribeEvents()
    {
        _accountService.ActiveAccountChanged += AccountService_ActiveAccountChanged;

        _settingsService.ActiveMinecraftFolderChanged += SettingsService_SettingsStringValueChanged;
        _settingsService.ActiveJavaChanged += SettingsService_SettingsStringValueChanged;
    }

    private void SettingsService_SettingsStringValueChanged(AppSettingsManagement.SettingsContainer sender, AppSettingsManagement.SettingChangedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new SettingsStringValueChangedMessage(
            e.NewValue == null ? string.Empty : e.NewValue.ToString()!,
            e.Path));
    }

    private void AccountService_ActiveAccountChanged(object? sender, Nrk.FluentCore.Authentication.Account? e)
    {
        if (e == null)
            return;

        WeakReferenceMessenger.Default.Send(new ActiveAccountChangedMessage(e));
    }
}
