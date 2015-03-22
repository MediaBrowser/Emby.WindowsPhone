using System.Collections.Generic;
using System.Linq;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.Sync;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class SyncOptionsControl
    {
        public SyncOptionsControl()
        {
            InitializeComponent();
        }

        private void SyncButton_OnTap(object sender, GestureEventArgs e)
        {
            if (MessagePrompt != null)
            {
                MessagePrompt.OnCompleted(new PopUpEventArgs<string, PopUpResult>{PopUpResult = PopUpResult.Ok});
            }
        }

        public MessagePrompt MessagePrompt { get; set; }

        public void SetOptions(List<SyncQualityOption> options)
        {
            OptionsPicker.ItemsSource = options;
            OptionsPicker.SelectedItem = OptionsPicker.Items.FirstOrDefault(x => (x as SyncQualityOption).IsDefault);
        }

        public SyncQualityOption GetSelectedOption()
        {
            var item = OptionsPicker.SelectedItem as SyncQualityOption;
            return item;
        }
    }
}
