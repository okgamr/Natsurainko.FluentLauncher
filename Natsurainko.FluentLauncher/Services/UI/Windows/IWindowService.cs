namespace Natsurainko.FluentLauncher.Services.UI.Windows;

// 提供窗口操作的服务
internal interface IWindowService
{
    /// <summary>
    /// 窗口标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 传入参数
    /// </summary>
    object? ActivatingParameter { get; }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    void Close();

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    void Hide();

    /// <summary>
    /// 激活窗口
    /// </summary>
    void Activate();
}
