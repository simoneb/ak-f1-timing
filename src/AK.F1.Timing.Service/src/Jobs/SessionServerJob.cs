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

using System.Diagnostics;
using AK.F1.Timing.Server;
using AK.F1.Timing.Server.Proxy;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Service.Jobs
{
    /// <summary>
    /// An <see cref="Quartz.IJob"/> which serves the current session. This class cannot be inherited.
    /// </summary>
    public sealed class SessionServerJob : SessionJobBase
    {
        #region Fields.

        private static volatile TcpServer _server;
        private static readonly object _syncRoot = new object();

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void ExecuteCore()
        {
            lock(_syncRoot)
            {
                Stop();
                Start();
            }
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Stops the server.
        /// </summary>
        internal static void Stop()
        {
            lock(_syncRoot)
            {
                DisposableBase.DisposeOf(_server);
                _server = null;
            }
        }

        #endregion

        #region Private Impl.

        private void Start()
        {
            Debug.Assert(_server == null);
            _server = new TcpServer(
                endpoint: ServiceConfiguration.Server.Endpoint,
                handler: new ProxySessionManager(CreateReader()),
                backlog: ServiceConfiguration.Server.ConnectionBacklog);
            _server.Start();
        }

        #endregion
    }
}
