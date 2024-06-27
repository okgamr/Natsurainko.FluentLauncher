using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace Natsurainko.FluentLauncher.Utils.Extensions;

internal static class StoryboardExtensions
{
    public static Task BeginAsync(this Storyboard storyboard)
    {
        ArgumentNullException.ThrowIfNull(storyboard);

        var taskCompletionSource = new TaskCompletionSource();

        void onComplete(object? s, object e)
        {
            //storyboard.Stop();
            storyboard.Completed -= onComplete;
            taskCompletionSource.SetResult();
        }

        storyboard.Completed += onComplete;
        storyboard.Begin();

        return taskCompletionSource.Task;
    }
}
