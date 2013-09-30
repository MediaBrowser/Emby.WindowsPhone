using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class LockScreenCollage
    {
        private readonly Image[] _imageItems;
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Stream>), typeof(LockScreenCollage), new PropertyMetadata(default(IEnumerable<Stream>), OnItemsSourceChanged));

        public IEnumerable<Stream> ItemsSource
        {
            get { return (IEnumerable<Stream>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public LockScreenCollage()
        {
            InitializeComponent();
            _imageItems = new[] { ImageOne, ImageTwo, ImageThree, ImageFour, ImageFive, ImageSix, ImageSeven, ImageEight, ImageNine, ImageTen, ImageEleven, ImageTwelve };
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var lsmi = sender as LockScreenCollage;
            var list = e.NewValue as IEnumerable<Stream>;
            if (lsmi == null || list == null)
            {
                return;
            }

            var bmpList = list.ToList();

            if (bmpList.Count < 12)
            {
                var difference = 12 - bmpList.Count;

                if (difference > bmpList.Count)
                {
                    while (bmpList.Count < 12)
                    {
                        var extra = bmpList;
                        bmpList.AddRange(extra);
                    }
                }
                else
                {
                    var extra = bmpList.Take(difference).ToList();
                    bmpList.AddRange(extra);
                }
            }

            var i = 0;
            foreach (var image in bmpList)
            {
                if (i >= lsmi._imageItems.Length)
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
