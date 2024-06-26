using CommunityToolkit.Mvvm.Messaging;

namespace Natsurainko.FluentLauncher.Services.UI.Messaging;

/// <summary>
/// 用于管理其他组件的事件订阅和全局消息传递的单例服务
/// </summary>
internal class MessengerService
{/*
    private readonly AccountService _accountService;

    public MessengerService(AccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    public void SubscribeEvents()
    {
        _accountService.ActiveAccountChanged += AccountService_ActiveAccountChanged;
    }

    private void AccountService_ActiveAccountChanged(object? sender, Nrk.FluentCore.Authentication.Account? e)
    {
        if (e == null)
            return;

        WeakReferenceMessenger.Default.Send(new ActiveAccountChangedMessage(e));
    }*/
}
