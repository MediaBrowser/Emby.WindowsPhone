using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MetroLog;
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
        private readonly ExtendedApiClient _apiClient;
        private readonly NavigationService _navigationService;
        private readonly ILogger _logger;

        private bool _isResume;
        private bool _isTrailer;
        /// <summary>
        /// Initializes a new instance of the VideoPlayerViewModel class.
        /// </summary>
        public VideoPlayerViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _logger = LogManagerFactory.DefaultLogManager.GetLogger<VideoPlayerViewModel>();

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
                        _isResume = (bool) m.Target;
                    _isTrailer = false;
                }
                if (m.Notification.Equals(Constants.PlayTrailerMsg))
                {
                    SelectedItem = (BaseItemDto) m.Sender;
                    _isTrailer = true;
                }
                if (m.Notification.Equals(Constants.VideoPlayerLoadedMsg))
                {
                    if (_isTrailer)
                    {
                        var url = SelectedItem.TrailerUrls[0];
                        if (url.ToLower().Contains("youtube."))
                        {
                            _logger.Info("Trying to play YouTube trailer");
                            var hadError = false;
                            try
                            {
                                VideoUrl = await ParseYoutubeLink(url);
                                _logger.Debug(VideoUrl);
                            }
                            catch(Exception ex)
                            {
                                _logger.Error(ex.Message, ex);
                                hadError = true;
                            }
                            if (hadError)
                            {
                                await MessageBox.ShowAsync("Unfortunately there was an error trying to retrieve the trailer, it could have been removed from the interwebs. Sorry about that.", "Error", MessageBoxButton.OK);
                                _navigationService.GoBack();
                            }
                        }
                        else
                        {
                            _logger.Info("Trying to play trailer");

                            VideoUrl = url;

                            _logger.Debug(VideoUrl);
                        }
                    }
                    else
                    {
                        long ticks = 0;
                        if (SelectedItem.UserData != null && _isResume)
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

                        VideoUrl = _apiClient.GetVideoStreamUrl(query);

                        Debug.WriteLine(VideoUrl);

                        _logger.Info("Playing {0} [{1}] ({2})", SelectedItem.Type, SelectedItem.Name, SelectedItem.Id);
                        _logger.Debug(VideoUrl);

                        try
                        {
                            await _apiClient.ReportPlaybackStartAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id).ConfigureAwait(true);
                        }
                        catch (HttpException ex)
                        {
                            _logger.Error(ex.Message, ex);
                        }
                    }
                }
                if(m.Notification.Equals(Constants.SendVideoTimeToServerMsg))
                {
                    try
                    {
                        var totalTicks = StartTime.HasValue ? StartTime.Value.Ticks + PlayedVideoDuration.Ticks : PlayedVideoDuration.Ticks;
                        
                        await _apiClient.ReportPlaybackStoppedAsync(SelectedItem.Id, App.Settings.LoggedInUser.Id, totalTicks);
                        
                        SelectedItem.UserData.PlaybackPositionTicks = totalTicks;
                    }
                    catch (HttpException ex)
                    {
                        _logger.Error(ex.Message, ex);
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