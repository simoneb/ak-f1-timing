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
using System.Collections.Generic;
using Xunit;

namespace AK.F1.Timing.Messaging
{
    public class MockMessageReader : IMessageReader
    {
        public MockMessageReader(params Message[] messages) {

            this.Messages = new Queue<Message>(messages);
        }

        public Message Read() {

            if(this.IsDisposed) {
                throw new ObjectDisposedException(typeof(MockMessageReader).FullName);
            }

            return this.Messages.Count > 0 ? this.Messages.Dequeue() : null;
        }

        public void Dispose() {

            this.IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

        private Queue<Message> Messages { get; set; }
    }
}
