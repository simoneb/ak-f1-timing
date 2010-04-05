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
        public void can_round_trip_primitives(object primitive) {

            RoundTripAndAssertEqual(primitive);
        }

        [Fact]
        public void can_read_and_write_null() {

            Assert.Null(RoundTrip((object)null));
        }

        [Fact]
        public void can_round_trip_multiple_primitives() {

            RoundTripAndAssertEqual(PrimitiveDataProvider.GetData());
        }

        [Theory]
        [ClassData(typeof(ComplexDataProvider))]
        public void can_round_trip_complex_types(object complex) {

            RoundTripAndAssertEqual(complex);
        }

        [Fact]
        public void can_round_trip_multiple_complex_types() {

            RoundTripAndAssertEqual(ComplexDataProvider.GetData());
        }

        [Fact]
        public void can_round_trip_interleaved_complex_and_primitive_types() {

            var graphs = new List<object>();            

            foreach(var primtive in PrimitiveDataProvider.GetData()) {
                foreach(var complex in ComplexDataProvider.GetData()) {
                    graphs.Add(primtive);
                    graphs.Add(complex);
                }
            }

            RoundTripAndAssertEqual(graphs);
        }

        [Fact]
        public void ignored_properties_are_not_serialized() {

            var expected = new TypeWithIgnoredProperty { IgnoredProperty = "Ignored" };
            var actual = RoundTrip(expected);

            Assert.Null(actual.IgnoredProperty);
        }

        [Fact]
        public void graphs_which_implement_object_reference_are_respected() {

            Assert.Same(ObjectReference.Instance, RoundTrip(ObjectReference.Instance));
        }

        public void RoundTripAndAssertEqual(IEnumerable<object> graphs) {

            var graphsCopy = graphs.ToArray();

            using(var stream = new MemoryStream()) {
                using(var writer = new DecoratedObjectWriter(stream)) {
                    foreach(var graph in graphsCopy) {
                        writer.Write(graph);
                    }
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream)) {
                    foreach(var graph in graphsCopy) {
                        var actual = reader.Read();
                        Assert.IsType(graph.GetType(), actual);
                        Assert.Equal(graph, actual);
                    }
                }
            }
        }

        private static void RoundTripAndAssertEqual(object graph) {

            object actual = RoundTrip(graph);

            if(graph == null) {
                Assert.Null(actual);
            } else {
                Assert.IsType(graph.GetType(), actual);
                Assert.Equal(graph, actual);
            }
        }

        private static T RoundTrip<T>(T graph) {

            object actual;

            using(var stream = new MemoryStream()) {
                using(var writer = new DecoratedObjectWriter(stream)) {
                    writer.Write(graph);
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream)) {
                    actual = reader.Read();
                }
            }

            if(graph != null) {
                Assert.IsType(graph.GetType(), actual);
            }

            return (T)actual;
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

            public static IEnumerable<object> GetData() {

                return new ComplexDataProvider().Select(x => x[0]);
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
                    object.Equals(other.EmptyType, EmptyType) &&
                    object.Equals(other.TypeWithPrivateCtor, TypeWithPrivateCtor);
            }

            public override int GetHashCode() {

                throw new NotImplementedException();
            }
        }

        [TypeId(36551276)]
        public class TypeWithIgnoredProperty
        {
            [IgnoreProperty]
            public string IgnoredProperty { get; set; }

            public override bool Equals(object obj) {

                throw new NotImplementedException();
            }

            public override int GetHashCode() {

                throw new NotImplementedException();
            }
        }

        public class PrimitiveDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

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

                yield break;

                // ByteEnum
                yield return A(ByteEnum.Default);
                yield return A(ByteEnum.Min);
                yield return A(ByteEnum.Max);
                // SByteEnum
                yield return A(SByteEnum.Default);
                yield return A(SByteEnum.Min);
                yield return A(SByteEnum.Max);
                // Int16Enum
                yield return A(Int16Enum.Default);
                yield return A(Int16Enum.Min);
                yield return A(Int16Enum.Max);
                // UInt16Enum
                yield return A(UInt16Enum.Default);
                yield return A(UInt16Enum.Min);
                yield return A(UInt16Enum.Max);
                // Int32Enum
                yield return A(Int32Enum.Default);
                yield return A(Int32Enum.Min);
                yield return A(Int32Enum.Max);
                // UInt32Enum
                yield return A(UInt32Enum.Default);
                yield return A(UInt32Enum.Min);
                yield return A(UInt32Enum.Max);
                // Int64Enum
                yield return A(Int64Enum.Default);
                yield return A(Int64Enum.Min);
                yield return A(Int64Enum.Max);
                // UInt64Enum
                yield return A(UInt64Enum.Default);
                yield return A(UInt64Enum.Min);
                yield return A(UInt64Enum.Max);
            }

            public static IEnumerable<object> GetData() {

                return new PrimitiveDataProvider().Select(x => x[0]);
            }

            private static object[] A(object value) {

                return new object[] { value };
            }

            IEnumerator IEnumerable.GetEnumerator() {
                throw new NotImplementedException();
            }
        }

        private enum ByteEnum : byte
        {
            Default = 0,
            Min = Byte.MinValue,
            Max = Byte.MaxValue
        }

        private enum SByteEnum : sbyte
        {
            Default = 0,
            Min = SByte.MinValue,
            Max = SByte.MaxValue
        }

        private enum Int16Enum : short
        {
            Default = 0,
            Min = Int16.MinValue,
            Max = Int16.MaxValue
        }

        private enum UInt16Enum : ushort
        {
            Default = 0,
            Min = UInt16.MinValue,
            Max = UInt16.MaxValue
        }

        private enum Int32Enum : int
        {
            Default = 0,
            Min = Int32.MinValue,
            Max = Int32.MaxValue
        }

        private enum UInt32Enum : uint
        {
            Default = 0,
            Min = UInt32.MinValue,
            Max = UInt32.MaxValue
        }

        private enum Int64Enum : long
        {
            Default = 0,
            Min = Int64.MinValue,
            Max = Int64.MaxValue
        }

        private enum UInt64Enum : ulong
        {
            Default = 0,
            Min = UInt64.MinValue,
            Max = UInt64.MaxValue
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
