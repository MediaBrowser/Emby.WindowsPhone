using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class WideTileControl
    {
        private readonly Image[] _imageItems;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Stream>), typeof(WideTileControl), new PropertyMetadata(default(IEnumerable<Stream>), OnItemsSourceChanged));

        public IEnumerable<Stream> ItemsSource
        {
            get { return (IEnumerable<Stream>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public WideTileControl()
        {
            InitializeComponent();
            _imageItems = new[] { ImageOne, ImageTwo, ImageThree, ImageFour, ImageFive, ImageSix, ImageSeven, ImageEight, ImageNine};
        }

        public void UpdateBackground()
        {
            LayoutRoot.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("/Assets/Tiles/WideTileBackground.png", UriKind.Relative))
            };
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var wtc = sender as WideTileControl;
            var list = e.NewValue as IEnumerable<Stream>;
            if (wtc == null || list == null)
            {
                return;
            }

            var bmpList = list.ToList();

            if (bmpList.Count < 9)
            {
                var difference = 9 - bmpList.Count;

                if (difference > bmpList.Count)
                {
                    while (bmpList.Count < 9)
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
                if (i >= wtc._imageItems.Length)
                {
                    break;
                }

                var bitmap = new BitmapImage();
                bitmap.SetSource(image);
                wtc._imageItems[i].Source = bitmap;
                i++;
            }
        }
    }
}
