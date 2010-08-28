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
using log4net;

namespace AK.F1.Timing.Live.IO
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Live.IO.IMessageStreamEndpoint"/> implementation that
    /// opens message streams provided by the F1 live-timing server. This class cannot be inherited.
    /// </summary>
    public class LiveMessageStreamEndpoint : IMessageStreamEndpoint
    {
        #region Private Fields.

        private const int StreamPort = 4321;
        private const string StreamHost = "live-timing.formula1.com";
        private const string KeyframeUrl = "http://live-timing.formula1.com/keyframe";
        private const string KeyframeExt = ".bin";

        private static readonly ILog Log = LogManager.GetLogger(typeof(LiveMessageStreamEndpoint));

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public IMessageStream OpenStream()
        {
            var endpoint = ResolveStreamEndpoint();
            var socket = CreateStreamingSocket();

            Log.InfoFormat("connecting: {0}", endpoint);

            try
            {
                socket.Connect(endpoint);
                Log.Info("connected");
                return new LiveSocketMessageStream(socket);
            }
            catch(SocketException exc)
            {
                ((IDisposable)socket).Dispose();
                Log.ErrorFormat("unable to connect to {0}: {1}", endpoint, exc.Message);
                throw Guard.LiveMessageStreamEndpoint_FailedToOpenStream(exc);
            }
        }

        /// <inheritdoc/>
        public IMessageStream OpenKeyframe(int keyframe)
        {
            Guard.InRange(keyframe >= 0, "keyframe");

            Uri keyframeUri = BuildKeyframeUri(keyframe);

            Log.InfoFormat("opening keyframe: {0}", keyframeUri);
            try
            {
                var stream = keyframeUri.GetResponseStream(HttpMethod.Get);
                Log.InfoFormat("opened keyframe, length: {0} bytes", stream.Length);
                return new MessageStreamDelegate(stream);
            }
            catch(IOException exc)
            {
                Log.Error(exc);
                throw Guard.LiveMessageStreamEndpoint_FailedToOpenKeyframe(exc);
            }
        }

        #endregion

        #region Private Impl.

        private static Socket CreateStreamingSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static IPEndPoint ResolveStreamEndpoint()
        {
            IPHostEntry entry;

            try
            {
                if((entry = Dns.GetHostEntry(StreamHost)).AddressList.Length == 0)
                {
                    throw Guard.LiveMessageStreamEndpoint_FailedToResolveStreamHost(StreamHost);
                }
                return new IPEndPoint(entry.AddressList[0], StreamPort);
            }
            catch(SocketException exc)
            {
                throw Guard.LiveMessageStreamEndpoint_FailedToResolveStreamHost(StreamHost, exc);
            }
        }

        private static Uri BuildKeyframeUri(int keyframe)
        {
            var sb = new StringBuilder();

            sb.Append(KeyframeUrl);
            if(keyframe != 0)
            {
                sb.Append("_");
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0:00000}", keyframe);
            }
            sb.Append(KeyframeExt);

            return new Uri(sb.ToString(), UriKind.Absolute);
        }

        #endregion
    }
}