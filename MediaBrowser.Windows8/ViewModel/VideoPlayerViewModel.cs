using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
using MediaBrowser.Windows8.Model;
using MyToolkit.Multimedia;
using ReflectionIT.Windows8.Helpers;
using Windows.UI.Xaml;
using System;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class VideoPlayerViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly NavigationService NavigationService;
        private bool isResume;
        private bool isTrailer;
        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            ApiClient = apiClient;
            NavigationService = navigationService;
            if (IsInDesignMode)
            {
                SelectedItem = new BaseItemDto
                                   {
                                       Name = "Jurassic Park"
                                   };
            }
            else
            {
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.PlayVideoItemMsg))
                {
                    SelectedItem = (BaseItemDto)m.Sender;
                    if(m.Target != null)
                        isResume = (bool) m.Target;
                    isTrailer = false;
                }
                if (m.Notification.Equals(Constants.PlayTrailerMsg))
                {
                    SelectedItem = (BaseItemDto) m.Sender;
                    isTrailer = true;
                }
                if (m.Notification.Equals(Constants.VideoPlayerLoadedMsg))
                {
                    if (isTrailer)
                    {
                        var url = SelectedItem.TrailerUrls[0];
                        if (url.ToLower().Contains("youtube."))
                        {
                            var hadError = false;
                            try
                            {
                                VideoUrl = await ParseYoutubeLink(url);
                            }
                            catch
                            {
                                hadError = true;
                            }
                            if (hadError)
                            {
                                await MessageBox.ShowAsync("Unfortunately there was an error trying to retrieve the trailer, it could have been removed from the interwebs. Sorry about that.", "Error", MessageBoxButton.OK);
                                NavigationService.GoBack();
                            }
                        }
                        else
                        {
                            VideoUrl = url;
                        }
                    }
                    else
                    {
                        long ticks = 0;
                        if (SelectedItem.UserData != null && isResume)
                        {
                            ticks = SelectedItem.UserData.PlaybackPositionTicks;
                        }
                        var bounds = Window.Current.Bounds;
                        var query = new VideoStreamOptions
                        {
                            ItemId = SelectedItem.Id,
                            VideoCodec = VideoCodecs.H264,
                            OutputFileExtension = "ts",
                            AudioCodec = AudioCodecs.Mp3,
                            StartTimeTicks = ticks,
                            MaxHeight = (int)bounds.Height,
                            MaxWidth = (int)bounds.Width
                        };
                        VideoUrl = ApiClient.GetVideoStreamUrl(query);
                        System.Diagnostics.Debug.WriteLine(VideoUrl);
                        await ApiClient.ReportPlaybackStartAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id).ConfigureAwait(true);
                    }
                }
                if(m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                        await ApiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                    }
                    catch
                    {
                        string v = "v";
                    }
                }
            });
        }

        private static async Task<string> ParseYoutubeLink(string link)
        {
            var ytLink = new Uri(link);
            var queries = ParseQueryString(ytLink.Query);
            var youtubeId = queries["v"];

            var result = await YouTube.GetVideoUriAsync(youtubeId);
            return result.Uri.ToString();
        }

        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var nameValueCollection = new Dictionary<string, string>();
            string[] items = queryString.Split('&');

            foreach (string item in items)
            {
                if (item.Contains("="))
                {
                    string[] nameValue = item.Split('=');
                    if (nameValue[0].Contains("?"))
                        nameValue[0] = nameValue[0].Replace("?", "");
                    nameValueCollection.Add(nameValue[0], (nameValue[1]));
                }
            }

            return nameValueCollection;
        }

        public BaseItemDto SelectedItem { get; set; }
        public TimeSpan PlayedVideoDuration { get; set; }
        public TimeSpan? StartTime { get; set; }
        public string VideoUrl { get; set; }
    }
}