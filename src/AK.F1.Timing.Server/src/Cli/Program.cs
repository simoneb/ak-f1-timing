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
using System.Threading;
using AK.F1.Timing.Server.Proxy;
using log4net;
using log4net.Config;

namespace AK.F1.Timing.Server.Cli
{
    /// <summary>
    /// Application entry point container. This class is <see langword="static"/>.
    /// </summary>
    public class Program
    {
        #region Fields.

        private readonly CommandLineOptions _options;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Interface.

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            var options = new CommandLineOptions();
            if(options.ParseAndContinue(args))
            {
                new Program(options).Run();
            }
        }

        #endregion

        #region Private Impl.

        static Program()
        {
            XmlConfigurator.Configure();
        }

        private Program(CommandLineOptions options)
        {
            _options = options;
        }

        private void Run()
        {
            try
            {
                RunCore();
            }
            catch(Exception exc)
            {
                Log.Fatal(exc);
            }
        }

        private void RunCore()
        {
            var exitEvent = new ManualResetEventSlim();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                exitEvent.Set();
            };
            using(var server = new TcpServer(
                endpoint: _options.BindEndpoint,
                handler: CreateSocketHandler(),
                backlog: _options.ConnectionBacklog))
            {
                server.Start();
                exitEvent.Wait();
            }
        }

        private ISocketHandler CreateSocketHandler()
        {
            return new ProxySessionManager(_options.Username, _options.Password);
        }

        #endregion
    }
}