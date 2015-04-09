using System;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using WebSocket4Net;
using WebSocketState = MediaBrowser.Model.Net.WebSocketState;

namespace MediaBrowser.Model
{
    public class WebSocketClient : IClientWebSocket
    {
        /// <summary>
        ///     The _socket
        /// </summary>
        private WebSocket _socket;

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public WebSocketState State
        {
            get
            {
                switch (_socket.State)
                {
                    case WebSocket4Net.WebSocketState.Closed:
                        return WebSocketState.Closed;
                    case WebSocket4Net.WebSocketState.Closing:
                        return WebSocketState.Closed;
                    case WebSocket4Net.WebSocketState.Connecting:
                        return WebSocketState.Connecting;
                    case WebSocket4Net.WebSocketState.None:
                        return WebSocketState.None;
                    case WebSocket4Net.WebSocketState.Open:
                        return WebSocketState.Open;
                    default:
                        return WebSocketState.None;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the receive action.
        /// </summary>
        /// <value>The receive action.</value>
        public Action<byte[]> OnReceiveBytes { get; set; }

        /// <summary>
        ///     Gets or sets the on receive.
        /// </summary>
        /// <value>The on receive.</value>
        public Action<string> OnReceive { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_socket != null)
            {
                WebSocketState state = State;

                if (state == WebSocketState.Open || state == WebSocketState.Connecting)
                {
                    _socket.Close();
                }

                _socket = null;
            }
        }

        /// <summary>
        ///     Occurs when [closed].
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        ///     Connects the async.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task ConnectAsync(string url, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            try
            {
                _socket = new WebSocket(url);

                _socket.MessageReceived += websocket_MessageReceived;

                _socket.Open();

                _socket.Opened += (sender, args) => taskCompletionSource.TrySetResult(true);
                _socket.Closed += _socket_Closed;
            }
            catch (Exception ex)
            {
                _socket = null;

                taskCompletionSource.TrySetException(ex);
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        ///     Handles the Closed event of the _socket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void _socket_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Handles the MessageReceived event of the websocket control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessageReceivedEventArgs" /> instance containing the event data.</param>
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (OnReceive != null)
            {
                OnReceive(e.Message);
            }
        }

        /// <summary>
        ///     Sends the async.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The type.</param>
        /// <param name="endOfMessage">if set to <c>true</c> [end of message].</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task SendAsync(byte[] bytes, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => _socket.Send(bytes, 0, bytes.Length), cancellationToken);
        }
    }
}