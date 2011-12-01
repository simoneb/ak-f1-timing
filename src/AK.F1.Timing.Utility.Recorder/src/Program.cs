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
using System.IO;
using System.Net;
using System.Threading;
using AK.CmdLine;
using AK.CmdLine.Impl;
using AK.F1.Timing.Proxy;
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Utility.Recorder
{
    /// <summary>
    /// F1 Live-Timing Recorder for .NET 4.
    /// </summary>
    public class Program
    {
        #region Fields.

        private const string TmsExtension = ".tms";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            var program = new Program();
            var driver = new CmdLineDriver(program, Console.Error);
            driver.TryProcess(args);
        }

        /// <summary>
        /// Reads and records the live-timing message stream.
        /// </summary>
        /// <param name="username">A valid www.formula1.com username.</param>
        /// <param name="password">The corresponding password.</param>
        /// <param name="session">The name of the session to record. Can contain path information.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/>, <paramref name="password"/> or 
        /// <paramref name="session"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/>, <paramref name="password"/> or 
        /// <paramref name="session"/> is empty.
        /// </exception>
        public void Live(string username, string password, string session)
        {
            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");
            Guard.NotNullOrEmpty(session, "session");

            string path;
            if(MakeRecordPath(session, out path))
            {
                DoIOBoundOperation(() =>
                {
                    AuthenticationToken token;
                    if(TryAuthenticate(username, password, out token))
                    {
                        Read(F1Timing.Live.ReadAndRecord(token, path));
                    }
                });
            }
        }

        /// <summary>
        /// Reads and records a proxied live-timing message stream.
        /// </summary>
        /// <param name="endpoint">The proxy address or endpoint.</param>
        /// <param name="session">The name of the session to record. Can contain path information.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="endpoint"/> or <paramref name="session"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="session"/> is empty.
        /// </exception>
        public void Proxy(IPEndPoint endpoint, string session)
        {
            Guard.NotNull(endpoint, "username");
            Guard.NotNullOrEmpty(session, "session");

            string path;
            if(MakeRecordPath(session, out path))
            {
                DoIOBoundOperation(() =>
                {
                    Read(F1Timing.Proxy.ReadAndRecord(endpoint, path));
                });
            }
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
            RegisterValueConverters();
        }

        private bool MakeRecordPath(string session, out string path)
        {
            path = session;
            if(!path.EndsWith(TmsExtension, StringComparison.OrdinalIgnoreCase))
            {
                path += TmsExtension;
            }
            if(!Path.IsPathRooted(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, path);
            }
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                return true;
            }
            catch(IOException exc)
            {
                Log.Error(exc);
                return false;
            }
        }

        private static void DoIOBoundOperation(Action operation)
        {
            int attempt = 1;
            const int maxAttempts = 6;
            TimeSpan backoff = TimeSpan.FromSeconds(5D);

            while(attempt <= maxAttempts)
            {
                try
                {
                    operation();
                    break;
                }
                catch(IOException exc)
                {
                    Log.Error(exc);
                }
                ++attempt;
                Log.InfoFormat("backing off for {0}s before attempt {1}/{2}",
                    backoff.TotalSeconds, attempt, maxAttempts);
                Thread.Sleep(backoff);
                backoff += backoff;
            }
        }

        private bool TryAuthenticate(string username, string password, out AuthenticationToken token)
        {
            Log.Info("authenticating...");
            try
            {
                token = F1Timing.Live.Login(username, password);
                Log.InfoFormat("authenticated: {0}", username);
                return true;
            }
            catch(Exception exc)
            {
                Log.Error(exc);
                token = null;
                return false;
            }
        }

        private static void Read(IMessageReader reader)
        {
            Log.Info("connecting...");
            try
            {
                using(reader)
                {
                    Message message;
                    while((message = reader.Read()) != null)
                    {
                        Log.Info(message.ToString());
                    }
                }
                Log.Info("disconnected");
            }
            catch(Exception exc)
            {
                Log.Error(exc);
            }
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

        #endregion
    }
}