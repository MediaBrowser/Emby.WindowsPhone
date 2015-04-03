using System;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Controls;
using MediaBrowser.WindowsPhone.Interfaces;
using MediaBrowser.WindowsPhone.Model.Sync;

namespace MediaBrowser.WindowsPhone.Services
{
    public class MessagePromptService : IMessagePromptService
    {
        public async Task<SyncOption> RequestSyncOption(SyncJobRequest request)
        {
            var tcs = new TaskCompletionSource<SyncOption>();

            var optionsControl = new SyncOptionsControl();
            var prompt = new MessagePrompt
            {
                Body = optionsControl
            };

            optionsControl.MessagePrompt = prompt;
            optionsControl.SetOptions(request);

            prompt.Completed += (sender, args) =>
            {
                if (args.PopUpResult == PopUpResult.Ok)
                {
                    var item = optionsControl.GetSelectedOption();
                    tcs.SetResult(item);
                }
                else
                {
                    tcs.SetCanceled();
                }
            };

            prompt.Show();

            try
            {
                var qualityResult = await tcs.Task;

                return qualityResult;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public void ShowMessage(string message, string title = "", Action action = null)
        {
            App.ShowMessage(message, title, action);
        }
    }
}
