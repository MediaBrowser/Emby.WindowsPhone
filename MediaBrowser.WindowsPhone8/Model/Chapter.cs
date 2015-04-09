using MediaBrowser.Model.Dto;
using PropertyChanged;

namespace Emby.WindowsPhone.Model
{
    [ImplementPropertyChanged]
    public class Chapter : ChapterInfoDto
    {
        public Chapter(ChapterInfoDto info)
        {
            ImageTag = info.ImageTag;
            Name = info.Name;
            StartPositionTicks = info.StartPositionTicks;
        }

        public string ImageUrl { get; set; }
    }
}
