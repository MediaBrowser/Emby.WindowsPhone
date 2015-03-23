using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using MediaBrowser.Model.Sync;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class SyncOptionsControl
    {
        public static readonly DependencyProperty SyncUnwatchedProperty = DependencyProperty.Register(
            "SyncUnwatched", typeof (bool), typeof (SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncUnwatched
        {
            get { return (bool) GetValue(SyncUnwatchedProperty); }
            set { SetValue(SyncUnwatchedProperty, value); }
        }

        public static readonly DependencyProperty SyncNewContentProperty = DependencyProperty.Register(
            "SyncNewContent", typeof (bool), typeof (SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncNewContent
        {
            get { return (bool) GetValue(SyncNewContentProperty); }
            set { SetValue(SyncNewContentProperty, value); }
        }

        public static readonly DependencyProperty ItemLimitProperty = DependencyProperty.Register(
            "ItemLimit", typeof (string), typeof (SyncOptionsControl), new PropertyMetadata(default(string)));

        public string ItemLimit
        {
            get { return (string) GetValue(ItemLimitProperty); }
            set { SetValue(ItemLimitProperty, value); }
        }

        public SyncOptionsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SyncButton_OnTap(object sender, GestureEventArgs e)
        {
            if (MessagePrompt != null)
            {
                MessagePrompt.OnCompleted(new PopUpEventArgs<string, PopUpResult>{PopUpResult = PopUpResult.Ok});
            }
        }

        public MessagePrompt MessagePrompt { get; set; }

        public void SetOptions(SyncDialogOptions options)
        {
            var unwatched = options.Options.Contains(SyncJobOption.UnwatchedOnly);
            var itemLimit = options.Options.Contains(SyncJobOption.ItemLimit);
            var autoSync = options.Options.Contains(SyncJobOption.SyncNewContent);

            OptionsPicker.ItemsSource = options.QualityOptions;
            OptionsPicker.SelectedItem = OptionsPicker.Items.FirstOrDefault(x => (x as SyncQualityOption).IsDefault);
            ItemLimitBox.Visibility = itemLimit ? Visibility.Visible : Visibility.Collapsed;
            AutoSyncVideos.Visibility = autoSync ? Visibility.Visible : Visibility.Collapsed;
            UnwatchedVideos.Visibility = unwatched ? Visibility.Visible : Visibility.Collapsed;
        }

        public SyncQualityOption GetSelectedOption()
        {
            var item = OptionsPicker.SelectedItem as SyncQualityOption;
            return item;
        }
    }
}
