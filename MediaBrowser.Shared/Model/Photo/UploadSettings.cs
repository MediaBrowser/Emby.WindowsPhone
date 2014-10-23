using System;
using PropertyChanged;

namespace MediaBrowser.WindowsPhone.Model.Photo
{
    [ImplementPropertyChanged]
    public class UploadSettings
    {
        public UploadSettings()
        {
            UploadAllPhotos = true;
            UploadAfterDateTime = DateTime.Now;
        }

        public bool IsPhotoUploadsEnabled { get; set; }
        public bool UploadAllPhotos { get; set; }
        public DateTime UploadAfterDateTime { get; set; }
    }
}
