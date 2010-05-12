// Copyright 2010 Andy Kernahan
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

namespace AK.F1.Timing
{
    public class StubMessageReader : IMessageReader
    {
        public StubMessageReader(params Message[] messages)
        {
            MessageQueue = new Queue<Message>(messages);
        }

        public Message Read()
        {
            if(IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return MessageQueue.Count > 0 ? MessageQueue.Dequeue() : null;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

        public Queue<Message> MessageQueue { get; set; }
    }
}