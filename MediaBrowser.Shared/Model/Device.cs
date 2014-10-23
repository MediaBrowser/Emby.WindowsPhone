using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Devices;
using Microsoft.Xna.Framework.Media;

namespace MediaBrowser.WindowsPhone.Model
{
    public class Device : IDevice
    {
        public IEnumerable<LocalFileInfo> GetLocalPhotos()
        {
            using (var mediaLibrary = new MediaLibrary())
            {
                var pictures = mediaLibrary.Pictures.Select(x => new LocalFileInfo
                {
                    Name = x.Name,
                    Album = x.Album != null ? x.Album.Name : string.Empty,
                    FullPath = string.Format("{0}//{1}", x.Album != null ? x.Album.Name : string.Empty, x.Name),
                    MimeType = "image/jpeg"
                }).ToList();

                return pictures;
            }
        }

        public IEnumerable<LocalFileInfo> GetLocalVideos()
        {
            return new List<LocalFileInfo>();
        }

        public async Task UploadFile(LocalFileInfo file, IApiClient apiClient, CancellationToken cancellationToken)
        {
            using (var mediaLibrary = new MediaLibrary())
            {
                var picture = mediaLibrary.Pictures.FirstOrDefault(x => (x.Album == null || x.Album.Name == file.Album) && x.Name == file.Name);

                if (picture != null)
                {
                    using (var stream = picture.GetImage())
                    {
                        await apiClient.UploadFile(stream, file, cancellationToken);
                    }
                }
            }
        }

        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public event EventHandler<EventArgs> ResumeFromSleep;
    }
}
