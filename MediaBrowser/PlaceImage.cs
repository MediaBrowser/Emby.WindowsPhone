// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Delay
{
    /// <summary>
    /// Class that makes it easy to display an image placeholder while a desired image loads.
    /// </summary>
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
                            "<Image x:Name=\"BackImage\"/>" +
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
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(PlaceImage), new PropertyMetadata(OnSourcePropertyChanged));
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
#if SILVERLIGHT
            // Avoid warning about unused parameters
            oldValue = newValue;
            newValue = oldValue;
#else
            // Unhook from old element
            var oldBitmapSource = oldValue as BitmapSource;
            if (null != oldBitmapSource)
            {
                oldBitmapSource.DownloadCompleted -= new EventHandler(ImageOpenedOrDownloadCompleted);
            }
            // Hook up to new element
            var frontBitmapImage = newValue as BitmapSource;
            if (null != frontBitmapImage)
            {
                frontBitmapImage.DownloadCompleted += new EventHandler(ImageOpenedOrDownloadCompleted);
            }
#endif
            // Update display
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

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the StretchDirection for the image.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }
        /// <summary>
        /// Identifies the StretchDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(PlaceImage), new PropertyMetadata(StretchDirection.Both));
#endif

        /// <summary>
        /// Initializes a new instance of the PlaceImage class.
        /// </summary>
        public PlaceImage()
        {
            // Load the control template
#if SILVERLIGHT
            Template = (ControlTemplate)XamlReader.Load(TemplateString);
#else
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(TemplateString)))
            {
                Template = (ControlTemplate)XamlReader.Load(stream);
            }
#endif
        }

        /// <summary>
        /// Invoked when a new Template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Unhook from old elements
#if SILVERLIGHT
            if (null != _frontImage)
            {
                _frontImage.ImageOpened -= new EventHandler<RoutedEventArgs>(ImageOpenedOrDownloadCompleted);
            }
#endif

            // Get template parts
            _backImage = GetTemplateChild("BackImage") as Image;
            _frontImage = GetTemplateChild("FrontImage") as Image;
            _frontImageLoaded = false;

            // Set Bindings and hook up to new elements
            if (null != _backImage)
            {
                _backImage.SetBinding(Image.SourceProperty, new Binding("PlaceholderSource") { Source = this });
                _backImage.SetBinding(Image.StretchProperty, new Binding("Stretch") { Source = this });
#if !SILVERLIGHT
                _backImage.SetBinding(Image.StretchDirectionProperty, new Binding("StretchDirection") { Source = this });
#endif
            }
            if (null != _frontImage)
            {
                if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                {
                    _frontImage.SetBinding(Image.SourceProperty, new Binding("Source") { Source = this });
                }
                _frontImage.SetBinding(Image.StretchProperty, new Binding("Stretch") { Source = this });
#if !SILVERLIGHT
                _frontImage.SetBinding(Image.StretchDirectionProperty, new Binding("StretchDirection") { Source = this });
#else
                _frontImage.ImageOpened += new EventHandler<RoutedEventArgs>(ImageOpenedOrDownloadCompleted);
#endif
            }

            // Update display
            UpdateBackImageVisibility();
        }

        /// <summary>
        /// Handles the ImageOpened or DownloadCompleted event for the front image.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="e">Event arguments.</param>
        private void ImageOpenedOrDownloadCompleted(object sender, EventArgs e)
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
                _backImage.Visibility = _frontImageLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
