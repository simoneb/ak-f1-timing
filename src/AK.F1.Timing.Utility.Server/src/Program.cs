// Copyright 2011 Andy Kernahan
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
using System.Net;
using System.Threading;
using AK.CmdLine;
using AK.CmdLine.Impl;
using AK.F1.Timing.Proxy;
using AK.F1.Timing.Server;
using AK.F1.Timing.Server.Proxy;
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Utility.Server
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public class Program
    {
        #region Fields.

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            new CmdLineDriver(new Program(), Console.Error).TryProcess(args);
        }

        /// <summary>
        /// Proxies the live-timing message stream to connected clients.
        /// </summary>
        /// <param name="username">A valid www.formula1.com username.</param>
        /// <param name="password">The corresponding password.</param>
        /// <param name="endpoint">The endpoint to bind to. The default is any on 4532.</param>
        /// <param name="connectionBacklog">The accept connection backlog.</param>     
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="connectionBacklog"/> is not positive.
        /// </exception>
        public void Live(string username, string password, IPEndPoint endpoint = null, int connectionBacklog = 50)
        {
            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");
            Guard.InRange(connectionBacklog > 0, "connectionBacklog");

            try
            {
                var reader = F1Timing.Live.Read(F1Timing.Live.Login(username, password));
                RunCore(reader, endpoint, connectionBacklog);
            }
            catch(Exception exc)
            {
                Log.Fatal(exc);
            }
        }

        /// <summary>
        /// Proxies a recorded live-timing message stream to connected clients.
        /// </summary>
        /// <param name="path">The path of the recorded live-timing message stream.</param>
        /// <param name="speed">The playback speed.</param>
        /// <param name="endpoint">The endpoint to bind to. The default is any on 4532.</param>
        /// <param name="connectionBacklog">The accept connection backlog.</param>  
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is  <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="path"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="speed"/> or <paramref name="connectionBacklog"/> is not positive.
        /// </exception>
        public void Playback(string path, double speed = 1d, IPEndPoint endpoint = null, int connectionBacklog = 50)
        {
            Guard.NotNullOrEmpty(path, "path");
            Guard.InRange(speed > 0d, "speed");
            Guard.InRange(connectionBacklog > 0, "connectionBacklog");

            try
            {
                var reader = F1Timing.Playback.Read(path);
                reader.PlaybackSpeed = speed;
                RunCore(reader, endpoint, connectionBacklog);
            }
            catch(Exception exc)
            {
                Log.Fatal(exc);
            }
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
            RegisterValueConverters();
        }

        private static void RegisterValueConverters()
        {
            DefaultValueConverter.Register(s =>
            {
                int colonIndex = s.IndexOf(':');
                if(colonIndex == -1)
                {
                    return new IPEndPoint(IPAddress.Parse(s), ProxyMessageReader.DefaultPort);
                }
                return new IPEndPoint(
                    IPAddress.Parse(s.Substring(0, colonIndex)),
                    Int32.Parse(s.Substring(colonIndex + 1)));
            });
        }

        private void RunCore(IMessageReader reader, IPEndPoint endpoint, int connectionBacklog)
        {
            var exitEvent = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                exitEvent.Set();
            };
            using(var server = new TcpServer(
                endpoint: endpoint ?? new IPEndPoint(IPAddress.Any, ProxyMessageReader.DefaultPort),
                handler: new ProxySessionManager(reader),
                backlog: connectionBacklog))
            {
                server.Start();
                CmdLineCancelKey.WaitFor().Dispose();
            }
        }

        #endregion
    }
}