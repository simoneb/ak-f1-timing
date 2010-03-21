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
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectReaderWriterTest
    {
        [Theory(Skip = "BUG: writer should not compress root primitives")]
        [ClassData(typeof(PrimitiveDataProvider))]
        public void can_write_and_read_primitives(object primitive) {

            WriteAndRead(primitive);
        }

        [Fact]
        public void graphs_which_implement_object_reference_are_respected() {

            Assert.Same(DBNull.Value, WriteAndRead(DBNull.Value));
        }

        private static object WriteAndRead(object graph) {

            object actual;

            using(var stream = new MemoryStream()) {
                using(var writer = new DecoratedObjectWriter(stream)) {
                    writer.Write(graph);
                }
                Assert.NotEqual(0L, stream.Position);
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream)) {
                    actual = reader.Read();
                }
            }

            if(graph == null) {
                Assert.Null(actual);
            } else {
                Assert.IsType(graph.GetType(), actual);
                Assert.Equal(graph, actual);
            }

            return actual;
        }

        public class PrimitiveDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return V(null);
                yield return V('c');
                yield return V("s");
                yield return V(true);
                // Byte
                yield return V((Byte)0);
                yield return V(Byte.MinValue);
                yield return V(Byte.MaxValue);
                // SByte
                yield return V((SByte)0);
                yield return V(SByte.MinValue);
                yield return V(SByte.MaxValue);
                // Int16
                yield return V((Int16)0);
                yield return V(Int16.MinValue);
                yield return V(Int16.MaxValue);
                // UInt16
                yield return V((UInt16)0);
                yield return V(UInt16.MinValue);
                yield return V(UInt16.MaxValue);
                // Int32
                yield return V((Int32)0);
                yield return V(Int32.MinValue);
                yield return V(Int32.MaxValue);
                // UInt32
                yield return V((UInt32)0);
                yield return V(UInt32.MinValue);
                yield return V(UInt32.MaxValue);
                // Int64
                yield return V((Int64)0);
                yield return V(Int64.MinValue);
                yield return V(Int64.MaxValue);
                // UInt64
                yield return V((UInt64)0);
                yield return V(UInt64.MinValue);
                yield return V(UInt64.MaxValue);
                //  Single
                yield return V((Single)0);
                yield return V(Single.MinValue);
                yield return V(Single.MaxValue);
                yield return V(Single.Epsilon);
                yield return V(Single.NaN);
                yield return V(Single.NegativeInfinity);
                yield return V(Single.PositiveInfinity);
                // Double
                yield return V((Double)0);
                yield return V(Double.MinValue);
                yield return V(Double.MaxValue);
                yield return V(Double.Epsilon);
                yield return V(Double.NaN);
                yield return V(Double.NegativeInfinity);
                yield return V(Double.PositiveInfinity);
                // Decimal
                yield return V((Decimal)0);
                yield return V(Decimal.MinValue);
                yield return V(Decimal.MaxValue);
                // DBNull
                yield return V(DBNull.Value);
                // TimeSpan
                yield return V(TimeSpan.Zero);
                yield return V(TimeSpan.MinValue);
                yield return V(TimeSpan.MaxValue);
                // DateTime
                yield return V(DateTime.Now);
                yield return V(DateTime.MinValue);
                yield return V(DateTime.MaxValue);
            }

            private static object[] V(object value) {

                return new object[] { value };
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                throw new NotImplementedException();
            }
        }
    }
}
