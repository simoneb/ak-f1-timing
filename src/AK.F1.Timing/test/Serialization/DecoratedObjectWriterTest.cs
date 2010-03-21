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
using System.IO;
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectWriterTest
    {
        [Fact]
        public void ctor_throws_if_output_is_null() {

            Assert.Throws<ArgumentNullException>(() => new DecoratedObjectWriter(null));
        }

        [Fact]
        public void output_is_not_closed_when_writer_is_disposed() {

            var output = new MemoryStream();

            using(var writer = new DecoratedObjectWriter(output)) { }

            Assert.DoesNotThrow(() => output.Position = 0);
            output.Dispose();
            Assert.Throws<ObjectDisposedException>(() => output.Position = 0);
        }

        [Fact]
        public void can_write_a_null_graph() {

            using(var writer = CreateMemoryBackedWriter()) {
                Assert.DoesNotThrow(() => writer.Write(null));
            }
        }

        [Theory]
        [ClassData(typeof(PrimitiveDataProvider))]
        public void can_write_all_clr_primitives(object primitive) {

            using(var writer = CreateMemoryBackedWriter()) {
                Assert.DoesNotThrow(() => writer.Write(primitive));
            }
        }

        [Fact]
        public void write_throws_if_graph_is_not_a_primitive_and_has_not_decorated_with_a_type_id() {

            using(var writer = CreateMemoryBackedWriter()) {
                Assert.Throws<SerializationException>(() => writer.Write(new object()));
            }
        }

        [Fact]
        public void write_throws_when_writer_has_been_disposed() {

            var writer = CreateMemoryBackedWriter();

            ((IDisposable)writer).Dispose();

            Assert.Throws<ObjectDisposedException>(() => writer.Write(null));
        }

        private static DecoratedObjectWriter CreateMemoryBackedWriter() {

            return new DecoratedObjectWriter(new MemoryStream());
        }

        public class PrimitiveDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return new object[] { 's' };
                yield return new object[] { "s" };
                yield return new object[] { true };
                yield return new object[] { (Byte)0 };
                yield return new object[] { (SByte)0 };
                yield return new object[] { (Int16)0 };
                yield return new object[] { (UInt16)0 };
                yield return new object[] { (Int32)0 };
                yield return new object[] { (UInt32)0 };
                yield return new object[] { (Int64)0 };
                yield return new object[] { (UInt64)0 };
                yield return new object[] { (Single)0 };
                yield return new object[] { (Double)0 };
                yield return new object[] { (Decimal)0 };
                yield return new object[] { DBNull.Value };
                yield return new object[] { TimeSpan.Zero };
                yield return new object[] { DateTime.MinValue };
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {

                return GetEnumerator();
            }
        }
    }
}
