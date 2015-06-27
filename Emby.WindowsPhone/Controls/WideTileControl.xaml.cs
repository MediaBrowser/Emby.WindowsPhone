using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Emby.WindowsPhone.Controls
{
    public partial class WideTileControl
    {
        private readonly Image[] _imageItems;

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Stream>), typeof(WideTileControl), new PropertyMetadata(default(IEnumerable<Stream>)));

        public IEnumerable<Stream> ItemsSource
        {
            get { return (IEnumerable<Stream>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty UseTransparentTileProperty = DependencyProperty.Register(
            "UseTransparentTile", typeof(bool), typeof(WideTileControl), new PropertyMetadata(default(bool)));

        public bool UseTransparentTile
        {
            get { return (bool)GetValue(UseTransparentTileProperty); }
            set { SetValue(UseTransparentTileProperty, value); }
        }

        public WideTileControl()
        {
            InitializeComponent();
            _imageItems = new[] { ImageOne, ImageTwo, ImageThree, ImageFour, ImageFive, ImageSix, ImageSeven, ImageEight, ImageNine };
        }

        public void UpdateBackground()
        {
            if (!UseTransparentTile)
            {
                LayoutRoot.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("/Assets/Tiles/WideTileBackground.png", UriKind.Relative))
                };
            }

            NormalImage.Visibility = UseTransparentTile ? Visibility.Collapsed : Visibility.Visible;
            BackgroundImage.Visibility = UseTransparentTile ? Visibility.Collapsed : Visibility.Visible;
            TransparentImage.Visibility = UseTransparentTile ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task SetImages()
        {
            var bmpList = ItemsSource.ToList();

            await Task.Factory.StartNew(() =>
            {
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

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var i = 0;
                    foreach (var image in bmpList)
                    {
                        if (i >= _imageItems.Length)
                        {
                            break;
                        }

                        var bitmap = new BitmapImage();
                        bitmap.SetSource(image);
                        _imageItems[i].Source = bitmap;
                        i++;
                    }
                });
            });
        }
    }
}
