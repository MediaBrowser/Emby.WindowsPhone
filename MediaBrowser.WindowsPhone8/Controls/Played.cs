using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;

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
            DependencyProperty.Register("Data", typeof (object), typeof (Played), new PropertyMetadata(default(BaseItemDto), OnDataChanged));

        public object Data
        {
            get { return GetValue(DataProperty); }
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

            UserItemDataDto userData = null;
            if (played.Data is BaseItemDto)
            {
                var item = played.Data as BaseItemDto;
                var type = item.Type;
                if (type != "Season" && type != "Series" && type != "BoxSet" && item.MediaType != "Video" && item.Type != "MusicAlbum" && item.Type != "MusicArtist")
                {
                    if (played._unwatchedGrid != null && played._theEllipse != null && played._watchedPath != null)
                    {
                        played._unwatchedGrid.Visibility = item.RecursiveUnplayedItemCount > 0 ? Visibility.Visible : Visibility.Collapsed;
                        played._theEllipse.Visibility = played._unwatchedGrid.Visibility;

                        if (item.UserData != null)
                        {
                            played._watchedPath.Visibility = item.UserData.Played ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else
                        {
                            played._watchedPath.Visibility = Visibility.Collapsed;
                        }
                    }
                    return;
                }

                if (item.MediaType == null || item.MediaType != "Video")
                {
                    if (played._unwatchedGrid != null && played._watchedPath != null && played._theEllipse != null)
                    {
                        played._unwatchedGrid.Visibility = item.RecursiveUnplayedItemCount.HasValue && item.RecursiveUnplayedItemCount.Value > 0
                            ? Visibility.Visible
                            : Visibility.Collapsed;

                        played._watchedPath.Visibility = item.UserData.Played
                            ? Visibility.Visible
                            : Visibility.Collapsed;

                        played._theEllipse.Visibility = (played._unwatchedGrid.Visibility == Visibility.Visible
                                                         || played._watchedPath.Visibility == Visibility.Visible)
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                    }

                    return;
                }
                userData = item.UserData;
            }

            if (played.Data is RecordingInfoDto)
            {
                var item = played.Data as RecordingInfoDto;
                userData = item.UserData;
            }

            if (played._unwatchedGrid != null && played._theEllipse != null)
            {
                played._unwatchedGrid.Visibility = Visibility.Collapsed;
                played._theEllipse.Visibility = Visibility.Collapsed;
            }

            if (played._watchedPath != null && played._theEllipse != null && userData != null)
            {
                played._watchedPath.Visibility = userData.Played ? Visibility.Visible : Visibility.Collapsed;
                played._theEllipse.Visibility = played._watchedPath.Visibility;
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
