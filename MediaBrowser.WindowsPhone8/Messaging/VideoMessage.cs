using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Messaging
{
    public class VideoMessage : MessageBase
    {
        public VideoMessage(BaseItemDto item, bool isResume, long? resumeTicks = null)
        {
            VideoItem = item;
            IsResume = isResume;
            if (VideoItem != null && resumeTicks.HasValue)
            {
                if (VideoItem.UserData == null)
                {
                    VideoItem.UserData = new UserItemDataDto();
                }

                VideoItem.UserData.PlaybackPositionTicks = resumeTicks.Value;
            }
        }

        public BaseItemDto VideoItem { get; set; }
        public bool IsResume { get; set; }
        public long? ResumeTicks { get; set; }
    }

    public class RemoteMessage : MessageBase
    {
        public RemoteMessage(string itemId, long? startPositionTicks)
        {
            ItemId = itemId;
            StartPositionTicks = startPositionTicks;
        }

        public string ItemId { get; set; }
        public long? StartPositionTicks { get; set; }
    }
}
