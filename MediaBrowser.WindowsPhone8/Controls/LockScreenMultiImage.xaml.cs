using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class LockScreenMultiImage
    {
        private readonly Image[] _imageItems;
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Stream>), typeof(LockScreenMultiImage), new PropertyMetadata(default(IEnumerable<Stream>), OnItemsSourceChanged));

        public IEnumerable<Stream> ItemsSource
        {
            get { return (IEnumerable<Stream>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public LockScreenMultiImage()
        {
            InitializeComponent();
            _imageItems = new[] {ImageOne, ImageTwo, ImageThree, ImageFour, ImageFive};
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var lsmi = sender as LockScreenMultiImage;
            var list = e.NewValue as IEnumerable<Stream>;
            if (lsmi == null || list == null)
            {
                return;
            }

            var i = 0;
            foreach (var image in list)
            {
                if (i > lsmi._imageItems.Length)
                {
                    break;
                }

                var bitmap = new BitmapImage();
                bitmap.SetSource(image);
                lsmi._imageItems[i].Source = bitmap;
                i++;
            }
        }
    }
}
