using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Logging;
using SharpGIS;

namespace MediaBrowser.WindowsPhone.Model
{
    public class AsyncHttpClient : IAsyncHttpClient
    {
        private GZipWebClient WebClient { get; set; }

        public AsyncHttpClient()
        {
            WebClient = new GZipWebClient();
        }

        public async Task<Stream> GetStreamAsync(string url, ILogger logger, CancellationToken cancellationToken)
        {
            return await WebClient.OpenReadTaskAsync(url);
        }

        public async Task<Stream> PostAsync(string url, string contentType, string postContent, ILogger logger, CancellationToken cancellationToken)
        {
            WebClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            var returnString = await WebClient.UploadStringTaskAsync(url, postContent);
            return new MemoryStream(Encoding.UTF8.GetBytes(returnString));
        }

        public bool IsBusy
        {
            get { return WebClient.IsBusy; }
        }

        public void Dispose()
        {

        }
    }
}
