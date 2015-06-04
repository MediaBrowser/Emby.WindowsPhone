using System.Collections.Generic;
using Emby.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Messaging
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

        public VideoMessage(BaseItemDto item, bool isResume, PlayerSourceType playerSourceType, long? resumeTicks = null)
        {
            VideoItem = item;
            IsResume = isResume;
            PlayerSourceType = playerSourceType;
            
            if (VideoItem != null && resumeTicks.HasValue)
            {
                if (VideoItem.UserData == null)
                {
                    VideoItem.UserData = new UserItemDataDto();
                }

                VideoItem.UserData.PlaybackPositionTicks = resumeTicks.Value;
            }
        }
        
        public PlayerSourceType PlayerSourceType { get; set; }
        public BaseItemDto VideoItem { get; set; }
        public IList<BaseItemDto> VideoPlaylists { get; set; }
        public bool IsResume { get; set; }
        public long? ResumeTicks { get; set; }
    }
}
