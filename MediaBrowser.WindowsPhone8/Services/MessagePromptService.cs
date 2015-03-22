using System.Collections.Generic;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Controls;
using MediaBrowser.WindowsPhone.Interfaces;

namespace MediaBrowser.WindowsPhone.Services
{
    public class MessagePromptService : IMessagePromptService
    {
        public async Task<SyncQualityOption> RequestSyncOption(List<SyncQualityOption> options)
        {
            var tcs = new TaskCompletionSource<SyncQualityOption>();

            var optionsControl = new SyncOptionsControl();
            var prompt = new MessagePrompt
            {
                Body = optionsControl
            };

            optionsControl.MessagePrompt = prompt;
            optionsControl.SetOptions(options);

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
                var result = await tcs.Task;
                
                return result;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}
