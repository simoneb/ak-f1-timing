// Copyright 2009 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Messaging.Live.IO
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Messaging.Live.IO.IMessageStreamEndpoint"/> implementation that
    /// opens message streams provided by the F1 live timing server. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    public class LiveMessageStreamEndpoint : IMessageStreamEndpoint
    {
        #region Private Fields.
        
        private const int STREAM_PORT = 4321;
        private const string STREAM_HOST = "live-timing.formula1.com";
        private const string KEYFRAME_URL = "http://live-timing.formula1.com/keyframe";
        private const string KEYFRAME_EXT = ".bin";
        private static readonly CultureInfo INV_CULTURE = CultureInfo.InvariantCulture;
        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(typeof(LiveMessageStreamEndpoint));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveMessageStreamEndpoint"/> class.
        /// </summary>
        public LiveMessageStreamEndpoint() { }

        /// <inheritdoc />
        public IMessageStream Open() {

            IPEndPoint endpoint = ResolveStreamEndpoint();
            Socket socket = CreateStreamSocket();

            // TODO the socket throws socket exceptions, check to ensure the networkstream wraps these
            // in IOExceptions.            

            _log.InfoFormat("connecting to {0}", endpoint);            

            try {
                socket.Connect(endpoint);
                _log.Info("connected");
            } catch(SocketException exc) {                
                _log.ErrorFormat("unable to connect to {0}: {1}", endpoint, exc.Message);
                ((IDisposable)socket).Dispose();
                throw Guard.LiveMessageStreamEndpoint_FailedToOpenStream(exc);
            }

            return new LiveSocketMessageStream(socket);
        }

        /// <inheritdoc />
        public IMessageStream OpenKeyframe(int keyframe) {

            Guard.InRange(keyframe >= 0, "keyframe");

            Stream stream;
            Uri uri = BuildKeyframeUri(keyframe);

            _log.InfoFormat("opening keyframe: {0}", uri);
            try {
                stream = uri.GetResponseStream(HttpMethod.Get);
                _log.InfoFormat("opened keyframe, length: {0}", stream.Length);
                return new MessageStreamDelegate(stream);
            } catch(IOException exc) {
                _log.Error(exc);
                throw Guard.LiveMessageStreamEndpoint_FailedToOpenKeyframe(exc);
            }
        }

        #endregion

        #region Private Impl.

        private static IPEndPoint ResolveStreamEndpoint() {

            IPHostEntry entry;

            try {
                entry = Dns.GetHostEntry(STREAM_HOST);
                if(entry.AddressList.Length > 0) {
                    return new IPEndPoint(entry.AddressList[0], STREAM_PORT);
                }
                throw Guard.LiveMessageStreamEndpoint_FailedToResolveStreamHost(STREAM_HOST);
            } catch(SocketException exc) {
                throw Guard.LiveMessageStreamEndpoint_FailedToResolveStreamHost(STREAM_HOST, exc);
            }
        }

        private static Socket CreateStreamSocket() {

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.NoDelay = false;

            return socket;
        }

        private Uri BuildKeyframeUri(int keyframe) {

            StringBuilder sb = new StringBuilder();

            sb.Append(KEYFRAME_URL);
            if(keyframe == 0) {
                sb.Append(KEYFRAME_EXT);
                if(this.KeyframeCount > 0)
                    sb.AppendFormat(INV_CULTURE, "?{0}", this.KeyframeCount);
                ++this.KeyframeCount;
            } else {
                sb.Append("_").AppendFormat(INV_CULTURE, "{0:00000}", keyframe).Append(KEYFRAME_EXT);
                this.KeyframeCount = 0;
            }

            return new Uri(sb.ToString(), UriKind.Absolute);
        }

        private int KeyframeCount { get; set; }

        #endregion
    }
}
