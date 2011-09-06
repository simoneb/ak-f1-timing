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

using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server.Proxy
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Server.ISocketHandler"/> which creates and manages a
    /// <see cref="AK.F1.Timing.Server.Proxy.ProxySession"/> for each connected client.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ProxySessionManager : DisposableBase, ISocketHandler
    {
        #region Fields.

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<int, Task> _sessions = new ConcurrentDictionary<int, Task>();

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxySessionManager));

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public void Handle(Socket client)
        {
            Guard.NotNull(client, "client");
            CheckDisposed();

            StartSessionTask(client);
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            CancelSessions();
        }

        #endregion

        #region Private Impl.

        private void StartSessionTask(Socket client)
        {
            var session = new ProxySession(client, _cancellationTokenSource.Token);
            var task = new Task(session.Run, _cancellationTokenSource.Token);
            task.ContinueWith(OnSessionTaskCompleted);
            Guard.Assert(_sessions.TryAdd(task.Id, task));
            Log.InfoFormat("starting session, id={0}, endpoint={1}, open={2}", task.Id,
                client.RemoteEndPoint, _sessions.Count);
            task.Start();
        }

        private void OnSessionTaskCompleted(Task task)
        {
            Task _;
            _sessions.TryRemove(task.Id, out _);
            Log.InfoFormat("removed session, id={0}, open={1}", task.Id, _sessions.Count);
        }

        private void CancelSessions()
        {
            var sessions = _sessions.Select(x => x.Value).ToArray();
            Log.InfoFormat("cancelling sessions, open={0}", sessions.Length);
            _cancellationTokenSource.Cancel();
            Task.WaitAll(sessions);
        }

        #endregion
    }
}
