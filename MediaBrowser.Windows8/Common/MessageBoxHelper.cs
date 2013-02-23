using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace ReflectionIT.Windows8.Helpers
{

    class MessageBox
    {

        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText,
                                                             string caption,
                                                             MessageBoxButton button)
        {

            MessageDialog md = new MessageDialog(messageBoxText, caption);
            MessageBoxResult result = MessageBoxResult.None;
            if (button.HasFlag(MessageBoxButton.OK))
            {
                md.Commands.Add(new UICommand("OK",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.OK)));
            }
            if (button.HasFlag(MessageBoxButton.Yes))
            {
                md.Commands.Add(new UICommand("Yes",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.Yes)));
            }
            if (button.HasFlag(MessageBoxButton.No))
            {
                md.Commands.Add(new UICommand("No",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.No)));
            }
            if (button.HasFlag(MessageBoxButton.Cancel))
            {
                md.Commands.Add(new UICommand("Cancel",
                    new UICommandInvokedHandler((cmd) => result = MessageBoxResult.Cancel)));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }
            var op = await md.ShowAsync();
            return result;
        }

        public static async Task<MessageBoxResult> ShowAsync(string messageBoxText)
        {
            return await MessageBox.ShowAsync(messageBoxText, null, MessageBoxButton.OK);
        }
    }

    // Summary:
    //     Specifies the buttons to include when you display a message box.
    [Flags]
    public enum MessageBoxButton
    {
        // Summary:
        //     Displays only the OK button.
        OK = 1,
        // Summary:
        //     Displays only the Cancel button.
        Cancel = 2,
        //
        // Summary:
        //     Displays both the OK and Cancel buttons.
        OKCancel = OK | Cancel,
        // Summary:
        //     Displays only the OK button.
        Yes = 4,
        // Summary:
        //     Displays only the Cancel button.
        No = 8,
        //
        // Summary:
        //     Displays both the OK and Cancel buttons.
        YesNo = Yes | No,
    }

    // Summary:
    //     Represents a user's response to a message box.
    public enum MessageBoxResult
    {
        // Summary:
        //     This value is not currently used.
        None = 0,
        //
        // Summary:
        //     The user clicked the OK button.
        OK = 1,
        //
        // Summary:
        //     The user clicked the Cancel button or pressed ESC.
        Cancel = 2,
        //
        // Summary:
        //     This value is not currently used.
        Yes = 6,
        //
        // Summary:
        //     This value is not currently used.
        No = 7,
    }
}

