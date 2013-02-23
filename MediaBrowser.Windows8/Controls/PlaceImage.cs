using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace MediaBrowser.Windows8.Controls
{
    public class PlaceImage : Control
    {
        /// <summary>
        /// Gets the ControlTemplate string for the control.
        /// </summary>
        /// <remarks>
        /// Not in generic.xaml so the implementation of PlaceImage can be entirely in this file.
        /// </remarks>
        private static string TemplateString
        {
            get
            {
                return
                    "<ControlTemplate " +
                        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
                        "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                            "<Grid>" +
                                "<Border x:Name=\"BackImageHolder\" Background=\"{TemplateBinding Background}\">" +
                                    "<Image x:Name=\"BackImage\"/>" +
                                "</Border>" +
                                "<Image x:Name=\"FrontImage\"/>" +
                            "</Grid>" +
                    "</ControlTemplate>";
            }
        }

        /// <summary>
        /// Stores a reference to the back image (placeholder image).
        /// </summary>
        private Image _backImage;

        /// <summary>
        /// Stores a reference to the front image (desired image).
        /// </summary>
        private Image _frontImage;

        /// <summary>
        /// The _border
        /// </summary>
        private Border _border;

        /// <summary>
        /// Stores a value indicating whether the front image is loaded.
        /// </summary>
        private bool _frontImageLoaded;

        /// <summary>
        /// Gets or sets the ImageSource for the placeholder image.
        /// </summary>
        public ImageSource PlaceholderSource
        {
            get { return (ImageSource)GetValue(PlaceholderSourceProperty); }
            set { SetValue(PlaceholderSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the PlaceholderSource dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderSourceProperty = DependencyProperty.Register("PlaceholderSource", typeof(ImageSource), typeof(PlaceImage), null);

        /// <summary>
        /// Gets or sets the ImageSource for the desired image.
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(PlaceImage), new PropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// Called when the Source dependency property changes.
        /// </summary>
        /// <param name="o">Event object.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((PlaceImage)o).OnSourcePropertyChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
        }

        /// <summary>
        /// Called when the Source dependency property changes.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        private void OnSourcePropertyChanged(ImageSource oldValue, ImageSource newValue)
        {
            oldValue = newValue;
            newValue = oldValue;

            _frontImageLoaded = false;
            UpdateBackImageVisibility();
        }

        /// <summary>
        /// Gets or sets the Stretch for the images.
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        /// <summary>
        /// Identifies the Stretch dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PlaceImage), new PropertyMetadata(Stretch.Uniform));

        public PlaceImage()
        {
            Template = (ControlTemplate) XamlReader.Load(TemplateString);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_frontImage != null)
            {
                _frontImage.ImageOpened -= ImageOpenedOrDownloadCompleted;
            }

            // Get template parts
            _backImage = GetTemplateChild("BackImage") as Image;
            _frontImage = GetTemplateChild("FrontImage") as Image;
            _border = GetTemplateChild("BackImageHolder") as Border;
            _frontImageLoaded = false;

            // Set Bindings and hook up to new elements
            if (null != _backImage)
            {
                _backImage.SetBinding(Image.SourceProperty, new Binding {Path = new PropertyPath("PlaceholderSource"), Source = this});
                _backImage.SetBinding(Image.StretchProperty, new Binding {Path = new PropertyPath("Stretch"), Source = this});
            }
            if (_frontImage != null)
            {
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    _frontImage.SetBinding(Image.SourceProperty, new Binding { Path= new PropertyPath("Source"), Source = this });
                }
                _frontImage.SetBinding(Image.StretchProperty, new Binding { Path = new PropertyPath("Stretch"), Source = this });
                _frontImage.ImageOpened += ImageOpenedOrDownloadCompleted;
            }

            // Update display
            UpdateBackImageVisibility();
        }

        /// <summary>
        /// Handles the ImageOpened or DownloadCompleted event for the front image.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ImageOpenedOrDownloadCompleted(object sender, RoutedEventArgs e)
        {
            _frontImageLoaded = true;
            UpdateBackImageVisibility();
        }

        /// <summary>
        /// Updates the Visibility of the back image according to whether the front image is loaded.
        /// </summary>
        private void UpdateBackImageVisibility()
        {
            if (null != _backImage)
            {
                _border.Visibility = _frontImageLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
