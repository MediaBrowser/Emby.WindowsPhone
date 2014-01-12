using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Controls
{
    [TemplatePart(Name = PartWatchedPath, Type = typeof(TextBlock))]
    [TemplatePart(Name = PartUnwatchedGrid, Type = typeof(TextBlock))]
    [TemplatePart(Name = PartEllipse, Type = typeof(Ellipse))]
    public class Played : Control
    {
        private const string PartWatchedPath = "WatchedPath";
        private const string PartUnwatchedGrid = "UnwatchedText";
        private const string PartEllipse = "TheEllipse";

        private TextBlock _unwatchedGrid;
        private TextBlock _watchedPath;
        private Ellipse _theEllipse;

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof (BaseItemDto), typeof (Played), new PropertyMetadata(default(BaseItemDto), OnDataChanged));

        public BaseItemDto Data
        {
            get { return (BaseItemDto) GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public Played()
        {
            DefaultStyleKey = typeof (Played);
        }

        private static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var played = sender as Played;
            SetData(played);
        }

        private static void SetData(Played played)
        {
            if (played == null)
            {
                return;
            }

            if (played.Data == null)
            {
                return;
            }

            var type = played.Data.Type;
            if (type != "Season" && type != "Series" && type != "BoxSet" && played.Data.MediaType != "Video")
            {
                return;
            }

            if (played.Data.MediaType == "Video")
            {
                if (played._unwatchedGrid != null && played._theEllipse != null)
                {
                    played._unwatchedGrid.Visibility = Visibility.Collapsed;
                    played._theEllipse.Visibility = Visibility.Collapsed;
                }

                if (played._watchedPath != null && played._theEllipse != null)
                {
                    played._watchedPath.Visibility = played.Data.UserData.Played ? Visibility.Visible : Visibility.Collapsed;
                    played._theEllipse.Visibility = played._watchedPath.Visibility;
                }

                return;
            }

            if (played._unwatchedGrid != null && played._watchedPath != null && played._theEllipse != null)
            {
                played._unwatchedGrid.Visibility = played.Data.RecursiveUnplayedItemCount.HasValue && played.Data.RecursiveUnplayedItemCount.Value > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                played._watchedPath.Visibility = played.Data.RecursiveUnplayedItemCount.HasValue && played.Data.RecursiveUnplayedItemCount.Value == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                played._theEllipse.Visibility = (played._unwatchedGrid.Visibility == Visibility.Visible
                                                 || played._watchedPath.Visibility == Visibility.Visible)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public override void OnApplyTemplate()
        {
            _unwatchedGrid = GetTemplateChild(PartUnwatchedGrid) as TextBlock;
            _watchedPath = GetTemplateChild(PartWatchedPath) as TextBlock;
            _theEllipse = GetTemplateChild(PartEllipse) as Ellipse;

            SetData(this);

            base.OnApplyTemplate();
        }
    }
}
