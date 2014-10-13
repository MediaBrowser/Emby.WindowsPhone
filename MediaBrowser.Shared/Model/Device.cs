using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Devices;

namespace MediaBrowser.WindowsPhone.Model
{
    public class Device : IDevice
    {
        public IEnumerable<LocalFileInfo> GetLocalPhotos()
        {
            return new List<LocalFileInfo>();
        }

        public IEnumerable<LocalFileInfo> GetLocalVideos()
        {
            return new List<LocalFileInfo>();
        }

        public async Task UploadFile(LocalFileInfo file, IApiClient apiClient, CancellationToken cancellationToken)
        {
        }

        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public event EventHandler<EventArgs> ResumeFromSleep;
    }
}
