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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectReaderWriterTest
    {
        [Theory]
        [ClassData(typeof(PrimitiveDataProvider))]
        public void can_write_and_read_primitives(object primitive) {

            WriteAndRead(primitive);
        }

        [Fact]
        public void can_write_and_read_multiple_primitives() {

            object[] graphs = new PrimitiveDataProvider().Select(x => x[0]).ToArray();

            using(var stream = new MemoryStream()) {
                WriteGraphs(stream, graphs);
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream)) {
                    foreach(var graph in graphs) {
                        Assert.Equal(graph, reader.Read());
                    }
                }
            }
        }

        [Theory]
        [ClassData(typeof(ComplexDataProvider))]
        public void can_write_and_read_complex_types(object complex) {

            WriteAndRead(complex);
        }

        [Fact]
        public void graphs_which_implement_object_reference_are_respected() {

            Assert.Same(ObjectReference.Instance, WriteAndRead(ObjectReference.Instance));
        }

        private static void WriteGraphs(Stream stream, params object[] graphs) {

            using(var writer = new DecoratedObjectWriter(stream)) {
                foreach(var graph in graphs) {
                    writer.Write(graph);
                }
            }
        }

        private static object WriteAndRead(object graph) {

            object actual;

            using(var stream = new MemoryStream()) {
                WriteGraphs(stream, graph);
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

        public class ComplexDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return A(new EmptyType());
                yield return A(TypeWithPrivateCtor.New());
                yield return A(new TypeWithComplexProperties());
                yield return A(new TypeWithComplexProperties {
                    EmptyType = new EmptyType(),
                    TypeWithPrivateCtor = TypeWithPrivateCtor.New()
                });
            }

            private static object[] A(object value) {

                return new object[] { value };
            }

            IEnumerator IEnumerable.GetEnumerator() {

                return GetEnumerator();
            }
        }

        [TypeId(23409283)]
        public class EmptyType
        {
            public override bool Equals(object obj) {

                return obj != null && obj.GetType() == GetType();
            }

            public override int GetHashCode() {

                throw new NotImplementedException();
            }
        }

        [TypeId(654645645)]
        public class TypeWithPrivateCtor : EmptyType
        {
            private TypeWithPrivateCtor() { }

            public static TypeWithPrivateCtor New() {

                return new TypeWithPrivateCtor();
            }
        }

        [TypeId(5528731)]
        public class TypeWithComplexProperties
        {
            [PropertyId(0)]
            public EmptyType EmptyType { get; set; }

            [PropertyId(1)]
            public TypeWithPrivateCtor TypeWithPrivateCtor { get; set; }

            public override bool Equals(object obj) {

                if(obj == null || obj.GetType() != GetType()) {
                    return false;
                }

                var other = (TypeWithComplexProperties)obj;

                return
                    object.Equals(other.EmptyType, this.EmptyType) &&
                    object.Equals(other.TypeWithPrivateCtor, this.TypeWithPrivateCtor);
            }

            public override int GetHashCode() {

                throw new NotImplementedException();
            }
        }

        public class PrimitiveDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                // Empty
                yield return A(null);
                // String
                yield return A("s");                
                // Boolean
                yield return A(true);
                yield return A(false);
                // Char
                yield return A('c');
                yield return A(char.MinValue);
                yield return A(char.MaxValue);
                // Byte
                yield return A((Byte)0);
                yield return A(Byte.MinValue);
                yield return A(Byte.MaxValue);
                // SByte
                yield return A((SByte)0);
                yield return A(SByte.MinValue);
                yield return A(SByte.MaxValue);
                // Int16
                yield return A((Int16)0);
                yield return A(Int16.MinValue);
                yield return A(Int16.MaxValue);
                // UInt16
                yield return A((UInt16)0);
                yield return A(UInt16.MinValue);
                yield return A(UInt16.MaxValue);
                // Int32
                yield return A((Int32)0);
                yield return A(Int32.MinValue);
                yield return A(Int32.MaxValue);
                // UInt32
                yield return A((UInt32)0);
                yield return A(UInt32.MinValue);
                yield return A(UInt32.MaxValue);
                // Int64
                yield return A((Int64)0);
                yield return A(Int64.MinValue);
                yield return A(Int64.MaxValue);
                // UInt64
                yield return A((UInt64)0);
                yield return A(UInt64.MinValue);
                yield return A(UInt64.MaxValue);
                //  Single
                yield return A((Single)0);
                yield return A(Single.MinValue);
                yield return A(Single.MaxValue);
                yield return A(Single.Epsilon);
                yield return A(Single.NaN);
                yield return A(Single.NegativeInfinity);
                yield return A(Single.PositiveInfinity);
                // Double
                yield return A((Double)0);
                yield return A(Double.MinValue);
                yield return A(Double.MaxValue);
                yield return A(Double.Epsilon);
                yield return A(Double.NaN);
                yield return A(Double.NegativeInfinity);
                yield return A(Double.PositiveInfinity);
                // Decimal
                yield return A((Decimal)0);
                yield return A(Decimal.MinValue);
                yield return A(Decimal.MaxValue);
                // DBNull
                yield return A(DBNull.Value);
                // TimeSpan
                yield return A(TimeSpan.Zero);
                yield return A(TimeSpan.MinValue);
                yield return A(TimeSpan.MaxValue);
                // DateTime
                yield return A(DateTime.Now);
                yield return A(DateTime.MinValue);
                yield return A(DateTime.MaxValue);
            }

            private static object[] A(object value) {

                return new object[] { value };
            }

            IEnumerator IEnumerable.GetEnumerator() {
                throw new NotImplementedException();
            }
        }

        [TypeId(123456789)]
        private sealed class ObjectReference : IObjectReference
        {
            public static readonly ObjectReference Instance = new ObjectReference();

            public object GetRealObject(StreamingContext context) {

                return Instance;
            }
        }
    }
}
