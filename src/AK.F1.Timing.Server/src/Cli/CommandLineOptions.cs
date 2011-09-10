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

using System.Net;
using AK.F1.Timing.Proxy;
using Genghis;

// Disable warning indicating fields are never assigned.
#pragma warning disable 0649

namespace AK.F1.Timing.Server.Cli
{
    internal sealed class CommandLineOptions : CommandLineParser
    {
        #region Public Interface.

        [ValueUsage("A valid www.formula1.com username.",
            Name = "username")]
        public string Username;

        [ValueUsage("The corresponding password.",
            Name = "password")]
        public string Password;

        [ValueUsage("The IP to bind to.",
            Name = "ip",
            Optional = true)]
        public string BindIP = "0.0.0.0";

        [ValueUsage("The port to bind to.",
            Name = "port",
            Optional = true)]
        public int BindPort = ProxyMessageReader.DefaultPort;

        [ValueUsage("The maximum incoming connection backlog.",
            Name = "connectionBacklog",
            Optional = true)]
        public int ConnectionBacklog = 100;

        [NoUsage]
        public IPEndPoint BindEndpoint
        {
            get { return new IPEndPoint(IPAddress.Parse(BindIP), BindPort); }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void Parse(string[] args, bool ignoreFirstArg)
        {
            base.Parse(args, ignoreFirstArg);
            ValidateBindIP();
            ValidateBindPort();
        }

        #endregion

        #region Private Impl.

        private void ValidateBindIP()
        {
            IPAddress _;
            if(!IPAddress.TryParse(BindIP, out _))
            {
                throw Usage("ip", "'{0}' is not a valid IP address", BindIP);
            }
        }

        private void ValidateBindPort()
        {
            if(BindPort < IPEndPoint.MinPort || BindPort > IPEndPoint.MaxPort)
            {
                throw Usage("port", "{0} is not a valid port", BindPort);
            }
        }

        private static UsageException Usage(string argName, string format, params object[] args)
        {
            return new UsageException(argName, string.Format(format, args));
        }

        #endregion
    }
}