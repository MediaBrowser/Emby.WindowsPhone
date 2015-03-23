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
        public async Task<SyncOption> RequestSyncOption(SyncDialogOptions options)
        {
            var tcs = new TaskCompletionSource<SyncQualityOption>();

            var optionsControl = new SyncOptionsControl();
            var prompt = new MessagePrompt
            {
                Body = optionsControl
            };

            var unwatched = options.Options.Contains(SyncJobOption.UnwatchedOnly);
            var itemLimit = options.Options.Contains(SyncJobOption.ItemLimit);
            var autoSync = options.Options.Contains(SyncJobOption.SyncNewContent);

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
                var qualityResult = await tcs.Task;

                var option = new SyncOption
                {
                    Quality = qualityResult,
                    AutoSyncNewItems = autoSync && optionsControl.SyncNewContent,
                    UnwatchedItems = unwatched && optionsControl.SyncUnwatched
                };


                if (itemLimit && !string.IsNullOrEmpty(optionsControl.ItemLimit))
                {
                    option.ItemLimit = int.Parse(optionsControl.ItemLimit);
                }

                return option;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }
    }
}
