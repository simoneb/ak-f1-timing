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
using System.Runtime.Serialization;
using Xunit;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectWriterTest
    {
        [Fact]
        public void can_write_a_null_graph()
        {
            Assert.DoesNotThrow(() => Write(null));
        }

        [Fact]
        public void only_graphs_decorated_with_a_type_id_can_be_written()
        {
            Assert.Throws<SerializationException>(() => Write(new object()));
            Assert.Throws<SerializationException>(() => Write('s'));
            Assert.Throws<SerializationException>(() => Write("s"));
            Assert.Throws<SerializationException>(() => Write(true));
            Assert.Throws<SerializationException>(() => Write((Byte)0));
            Assert.Throws<SerializationException>(() => Write((SByte)0));
            Assert.Throws<SerializationException>(() => Write((Int16)0));
            Assert.Throws<SerializationException>(() => Write((UInt16)0));
            Assert.Throws<SerializationException>(() => Write(0));
            Assert.Throws<SerializationException>(() => Write((UInt32)0));
            Assert.Throws<SerializationException>(() => Write((Int64)0));
            Assert.Throws<SerializationException>(() => Write((UInt64)0));
            Assert.Throws<SerializationException>(() => Write((Single)0));
            Assert.Throws<SerializationException>(() => Write((Double)0));
            Assert.Throws<SerializationException>(() => Write((Decimal)0));
            Assert.Throws<SerializationException>(() => Write(DBNull.Value));
            Assert.Throws<SerializationException>(() => Write(TimeSpan.Zero));
            Assert.Throws<SerializationException>(() => Write(DateTime.MinValue));
        }

        [Fact]
        public void graphs_which_contain_circular_references_are_not_supported()
        {
            var graph = new SimpleType();
            // Root to self.
            graph.Value = graph;
            Assert.Throws<SerializationException>(() => Write(graph));
            // Descendent to root.
            graph.Value = new SimpleType {Value = graph};
            Assert.Throws<SerializationException>(() => Write(graph));
            // Descendent to self.
            graph.Value = new SimpleType();
            graph.Value.Value = graph.Value;
            Assert.Throws<SerializationException>(() => Write(graph));
            // Descendent to parent.
            graph.Value = new SimpleType {Value = graph.Value};
            Assert.Throws<SerializationException>(() => Write(graph));
        }

        [Fact]
        public void the_same_graph_written_on_the_same_stream_is_not_detected_as_a_circular_reference()
        {
            var graph = new SimpleType();

            using(var writer = CreateWriter())
            {
                writer.Write(graph);
                writer.Write(graph);
            }
        }

        [Fact]
        public void ctor_throws_if_output_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DecoratedObjectWriter(null));
        }

        [Fact]
        public void output_is_not_closed_when_writer_is_disposed()
        {
            var output = new MemoryStream();

            using(var writer = new DecoratedObjectWriter(output)) {}

            Assert.DoesNotThrow(() => output.Position = 0);
            output.Dispose();
            Assert.Throws<ObjectDisposedException>(() => output.Position = 0);
        }

        [Fact]
        public void write_throws_when_writer_has_been_disposed()
        {
            var writer = CreateWriter();

            ((IDisposable)writer).Dispose();

            Assert.Throws<ObjectDisposedException>(() => writer.Write(null));
        }

        private static void Write(object graph)
        {
            using(var writer = CreateWriter())
            {
                writer.Write(graph);
            }
        }

        private static DecoratedObjectWriter CreateWriter()
        {
            return new DecoratedObjectWriter(new MemoryStream());
        }

        [TypeId(42871340)]
        private class SimpleType
        {
            [PropertyId(0)]
            public SimpleType Value { get; set; }
        }
    }
}