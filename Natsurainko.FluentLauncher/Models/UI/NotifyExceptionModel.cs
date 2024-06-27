using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text;

#nullable disable

namespace Natsurainko.FluentLauncher.Models.UI;

internal partial class NotifyExceptionModel : ObservableObject
{
    public Exception Exception { get; set; }

    [ObservableProperty]
    private string description;

    public string ExceptionMessage
    {
        get
        {
            var @string = new StringBuilder();
            @string.AppendLine((Exception.InnerException ?? Exception).GetType().ToString());

            if (!string.IsNullOrEmpty((Exception.InnerException ?? Exception).Message))
                Description += "...\r\n" + (Exception.InnerException ?? Exception).Message;

            if (!string.IsNullOrEmpty((Exception.InnerException ?? Exception).HelpLink))
                Description += "...\r\n" + (Exception.InnerException ?? Exception).HelpLink;

            @string.AppendLine((Exception.InnerException ?? Exception).StackTrace);

            return @string.ToString();
        }
    }

    [RelayCommand]
    public void Copy()
    {

    }
}
