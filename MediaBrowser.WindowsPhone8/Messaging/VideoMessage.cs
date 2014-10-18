using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone.Messaging
{
    public class VideoMessage : MessageBase
    {
        public VideoMessage(IList<BaseItemDto> playlist, BaseItemDto firstItem, bool isResume)
        {
            VideoPlaylists = playlist;
            VideoItem = firstItem;
            IsResume = isResume;
            PlayerSourceType = PlayerSourceType.Playlist;
        }

        public VideoMessage(BaseItemDto item, bool isResume, long? resumeTicks = null)
        {
            VideoItem = item;
            IsResume = isResume;
            PlayerSourceType = PlayerSourceType.Video;
            
            if (VideoItem != null && resumeTicks.HasValue)
            {
                if (VideoItem.UserData == null)
                {
                    VideoItem.UserData = new UserItemDataDto();
                }

                VideoItem.UserData.PlaybackPositionTicks = resumeTicks.Value;
            }
        }

        public VideoMessage(RecordingInfoDto item, bool isResume, long? resumeTicks = null)
        {
            RecordingItem = item;
            IsResume = isResume;
            PlayerSourceType = PlayerSourceType.Recording;

            if (RecordingItem != null && resumeTicks.HasValue)
            {
                if (RecordingItem.UserData == null)
                {
                    RecordingItem.UserData = new UserItemDataDto();
                }

                RecordingItem.UserData.PlaybackPositionTicks = resumeTicks.Value;
            }
        }

        public VideoMessage(ProgramInfoDto item, bool isResume, long? resumeTicks = null)
        {
            ProgrammeItem = item;
            IsResume = isResume;
            PlayerSourceType = PlayerSourceType.Programme;

            if (ProgrammeItem != null && resumeTicks.HasValue)
            {
                if (ProgrammeItem.UserData == null)
                {
                    ProgrammeItem.UserData = new UserItemDataDto();
                }

                ProgrammeItem.UserData.PlaybackPositionTicks = resumeTicks.Value;
            }
        }


        public PlayerSourceType PlayerSourceType { get; set; }
        public BaseItemDto VideoItem { get; set; }
        public IList<BaseItemDto> VideoPlaylists { get; set; }
        public RecordingInfoDto RecordingItem { get; set; }
        public ProgramInfoDto ProgrammeItem { get; set; }
        public bool IsResume { get; set; }
        public long? ResumeTicks { get; set; }
    }
}
