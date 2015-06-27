
namespace MediaBrowser.Shared
{
    public class RequestResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RequestResult" /> is success.
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Success { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }
    }
}
