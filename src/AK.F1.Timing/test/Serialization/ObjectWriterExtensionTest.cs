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
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Session;
using Xunit;

namespace AK.F1.Timing.Serialization
{
    public class ObjectWriterExtensionTest
    {
        [Fact]
        public void write_message_throws_if_writer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ObjectWriterExtensions.WriteMessage(null, Message.Empty));
        }

        [Fact]
        public void can_write_a_null_message()
        {
            var writer = new ObjectWriterStub();

            writer.WriteMessage(null);
            Assert.Null(writer.Graphs[0]);
        }

        [Fact]
        public void composite_messages_are_expanded_when_written()
        {
            var writer = new ObjectWriterStub();
            var graph = new CompositeMessage(
                EndOfSessionMessage.Instance,
                new CompositeMessage(
                        StartSessionTimeCountdownMessage.Instance,
                        new CompositeMessage(StopSessionTimeCountdownMessage.Instance)));

            writer.WriteMessage(graph);
            Assert.Equal(3, writer.Graphs.Count);
            Assert.Same(EndOfSessionMessage.Instance, writer.Graphs[0]);
            Assert.Same(StartSessionTimeCountdownMessage.Instance, writer.Graphs[1]);
            Assert.Same(StopSessionTimeCountdownMessage.Instance, writer.Graphs[2]);
        }

        private sealed class ObjectWriterStub : IObjectWriter
        {
            public ObjectWriterStub()
            {
                Graphs = new List<object>();
            }

            public void Write(object graph)
            {
                Graphs.Add(graph);
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public IList<object> Graphs { get; private set; }
        }
    }
}