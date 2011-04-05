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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectReaderWriterTest
    {
        [Fact]
        public void can_round_trip_null()
        {
            Assert.Null(RoundTrip((object)null));
        }

        [Fact]
        public void ignored_properties_are_not_serialized()
        {
            var expected = new TypeWithIgnoredProperty { IgnoredProperty = "Ignored" };
            var actual = RoundTrip(expected);

            Assert.Null(actual.IgnoredProperty);
        }

        [Fact]
        public void object_references_can_be_round_tripped_and_the_real_object_is_returned()
        {
            Assert.Same(ObjectReference.Instance, RoundTrip(ObjectReference.Instance));
        }

        [Fact]
        public void can_round_trip_types_with_private_ctors()
        {
            Assert.NotNull(RoundTrip(TypeWithPrivateCtor.New()));
        }

        [Fact]
        public void can_round_trip_types_with_protected_ctors()
        {
            Assert.NotNull(RoundTrip(TypeWithPrivateCtor.New()));
        }

        [Fact]
        public void can_round_trip_empty_types()
        {
            Assert.NotNull(RoundTrip(new EmptyType()));
        }

        [Theory]
        [ClassData(typeof(PrimitiveDataProvider))]
        public void can_round_trip_types_with_primitive_properties(object graph)
        {
            var actual = RoundTrip(graph);

            Assert.Equal(graph, actual);
        }

        [Theory]
        [ClassData(typeof(ComplexDataProvider))]
        public void can_round_trip_types_with_complex_properties(object graph)
        {
            var actual = RoundTrip(graph);

            Assert.Equal(graph, actual);
        }

        [Fact]
        public void can_round_trip_multiple_types_on_one_stream()
        {
            var graphs = PrimitiveDataProvider.GetData().ToList();

            using(var stream = new MemoryStream())
            {
                using(var writer = new DecoratedObjectWriter(stream))
                {
                    foreach(var graph in graphs)
                    {
                        writer.Write(graph);
                    }
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream))
                {
                    foreach(var graph in graphs)
                    {
                        Assert.Equal(graph, reader.Read());
                    }
                }
            }
        }

        [Fact]
        public void supports_custom_serializable_types()
        {
            using(var stream = new MemoryStream())
            {
                var serializable = new CustomSerializableType();
                using(var writer = new DecoratedObjectWriter(stream))
                {
                    writer.Write(serializable);
                    Assert.Equal(1, serializable.WriteCallCount);
                    Assert.Same(writer, serializable.Writer);
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream))
                {
                    serializable = (CustomSerializableType)reader.Read();
                    Assert.Equal(1, serializable.ReadCallCount);
                    Assert.Same(reader, serializable.Reader);
                }
            }
        }

        private static T RoundTrip<T>(T graph)
        {
            object actual;

            using(var stream = new MemoryStream())
            {
                using(var writer = new DecoratedObjectWriter(stream))
                {
                    writer.Write(graph);
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream))
                {
                    actual = reader.Read();
                }
            }

            if(graph != null)
            {
                Assert.NotNull(actual);
                Assert.IsType(graph.GetType(), actual);
            }
            else
            {
                Assert.Null(actual);
            }

            return (T)actual;
        }

        [TypeId(23409283)]
        public class EmptyType
        {
            public override bool Equals(object obj)
            {
                return obj != null && obj.GetType() == GetType();
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        [TypeId(654645645)]
        public class TypeWithPrivateCtor : EmptyType
        {
            private TypeWithPrivateCtor() { }

            public static TypeWithPrivateCtor New()
            {
                return new TypeWithPrivateCtor();
            }
        }

        [TypeId(4431976)]
        public class TypeWithProtectedCtor : EmptyType
        {
            protected TypeWithProtectedCtor() { }

            public static TypeWithProtectedCtor New()
            {
                return new TypeWithProtectedCtor();
            }
        }

        [TypeId(123456789)]
        private sealed class ObjectReference : IObjectReference
        {
            public static readonly ObjectReference Instance = new ObjectReference();

            public object GetRealObject(StreamingContext context)
            {
                return Instance;
            }
        }

        [TypeId(36551276)]
        public class TypeWithIgnoredProperty
        {
            [IgnoreProperty]
            public string IgnoredProperty { get; set; }

            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        [TypeId(46713346)]
        public class CustomSerializableType : ICustomSerializable
        {
            public int WriteCallCount;
            public IObjectWriter Writer;
            void ICustomSerializable.Write(IObjectWriter writer)
            {
                ++WriteCallCount;
                Writer = writer;
            }

            public int ReadCallCount;
            public IObjectReader Reader;
            void ICustomSerializable.Read(IObjectReader reader)
            {
                ++ReadCallCount;
                Reader = reader;
            }
        }

        [TypeId(48247595)]
        private class BooleanHolder : PrimitiveHolder<Boolean>
        {
            public BooleanHolder()
            {
                Min = false;
                Max = true;
            }
        }

        [TypeId(68102379)]
        private class CharHolder : PrimitiveHolder<Char>
        {
            public CharHolder()
            {
                Min = Char.MinValue;
                Max = Char.MaxValue;
            }
        }

        [TypeId(58738455)]
        private class StringHolder : PrimitiveHolder<String>
        {
            public StringHolder()
            {
                Min = "Min";
                Max = "Max";
            }
        }

        [TypeId(84504291)]
        private class DateTimeHolder : PrimitiveHolder<DateTime>
        {
            public DateTimeHolder()
            {
                Min = DateTime.MinValue;
                Max = DateTime.MaxValue;
            }
        }

        [TypeId(88324618)]
        private class TimeSpanHolder : PrimitiveHolder<TimeSpan>
        {
            public TimeSpanHolder()
            {
                Min = TimeSpan.MinValue;
                Max = TimeSpan.MaxValue;
            }
        }

        [TypeId(56716748)]
        private class ByteHolder : PrimitiveHolder<Byte>
        {
            public ByteHolder()
            {
                Min = Byte.MinValue;
                Max = Byte.MaxValue;
            }
        }

        [TypeId(83053711)]
        private class SByteHolder : PrimitiveHolder<SByte>
        {
            public SByteHolder()
            {
                Min = SByte.MinValue;
                Max = SByte.MaxValue;
            }
        }

        [TypeId(37755692)]
        private class Int16Holder : PrimitiveHolder<Int16>
        {
            public Int16Holder()
            {
                Min = Int16.MinValue;
                Max = Int16.MaxValue;
            }
        }

        [TypeId(28095204)]
        private class UInt16Holder : PrimitiveHolder<UInt16>
        {
            public UInt16Holder()
            {
                Min = UInt16.MinValue;
                Max = UInt16.MaxValue;
            }
        }

        [TypeId(80020408)]
        private class Int32Holder : PrimitiveHolder<Int32>
        {
            public Int32Holder()
            {
                Min = Int32.MinValue;
                Max = Int32.MaxValue;
            }
        }

        [TypeId(73188542)]
        private class UInt32Holder : PrimitiveHolder<UInt32>
        {
            public UInt32Holder()
            {
                Min = UInt32.MinValue;
                Max = UInt32.MaxValue;
            }
        }

        [TypeId(48194370)]
        private class Int64Holder : PrimitiveHolder<Int64>
        {
            public Int64Holder()
            {
                Min = Int64.MinValue;
                Max = Int64.MaxValue;
            }
        }

        [TypeId(95896724)]
        private class UInt64Holder : PrimitiveHolder<UInt64>
        {
            public UInt64Holder()
            {
                Min = UInt64.MinValue;
                Max = UInt64.MaxValue;
            }
        }

        [TypeId(52151731)]
        private class DoubleHolder : PrimitiveHolder<Double>
        {
            public DoubleHolder()
            {
                Min = Double.MinValue;
                Max = Double.MaxValue;
            }
        }

        [TypeId(29802692)]
        private class SingleHolder : PrimitiveHolder<Single>
        {
            public SingleHolder()
            {
                Min = Single.MinValue;
                Max = Single.MaxValue;
            }
        }

        [TypeId(62803598)]
        private class DecimalHolder : PrimitiveHolder<Decimal>
        {
            public DecimalHolder()
            {
                Min = Decimal.MinValue;
                Max = Decimal.MaxValue;
            }
        }

        [TypeId(26740644)]
        private class ByteEnumHolder : PrimitiveHolder<ByteEnum>
        {
            public ByteEnumHolder()
            {
                Min = ByteEnum.Min;
                Max = ByteEnum.Max;
            }
        }

        [TypeId(29618117)]
        private class SByteEnumHolder : PrimitiveHolder<SByteEnum>
        {
            public SByteEnumHolder()
            {
                Min = SByteEnum.Min;
                Max = SByteEnum.Max;
            }
        }

        [TypeId(49263960)]
        private class Int16EnumHolder : PrimitiveHolder<Int16Enum>
        {
            public Int16EnumHolder()
            {
                Min = Int16Enum.Min;
                Max = Int16Enum.Max;
            }
        }

        [TypeId(10754466)]
        private class UInt16EnumHolder : PrimitiveHolder<UInt16Enum>
        {
            public UInt16EnumHolder()
            {
                Min = UInt16Enum.Min;
                Max = UInt16Enum.Max;
            }
        }

        [TypeId(67571400)]
        private class Int32EnumHolder : PrimitiveHolder<Int32Enum>
        {
            public Int32EnumHolder()
            {
                Min = Int32Enum.Min;
                Max = Int32Enum.Max;
            }
        }

        [TypeId(59285730)]
        private class UInt32EnumHolder : PrimitiveHolder<UInt32Enum>
        {
            public UInt32EnumHolder()
            {
                Min = UInt32Enum.Min;
                Max = UInt32Enum.Max;
            }
        }

        [TypeId(70098681)]
        private class Int64EnumHolder : PrimitiveHolder<Int64Enum>
        {
            public Int64EnumHolder()
            {
                Min = Int64Enum.Min;
                Max = Int64Enum.Max;
            }
        }

        [TypeId(58969482)]
        private class UInt64EnumHolder : PrimitiveHolder<UInt64Enum>
        {
            public UInt64EnumHolder()
            {
                Min = UInt64Enum.Min;
                Max = UInt64Enum.Max;
            }
        }

        [TypeId(19310090)]
        private abstract class PrimitiveHolder<T>
        {
            [PropertyId(0)]
            public T Min { get; set; }

            [PropertyId(1)]
            public T Max { get; set; }

            [PropertyId(2)]
            public T Default { get; set; }

            protected PrimitiveHolder()
            {
                Default = default(T);
            }

            public override bool Equals(object obj)
            {
                if(obj == null || obj.GetType() != GetType())
                {
                    return false;
                }
                if(obj == this)
                {
                    return true;
                }
                var other = (PrimitiveHolder<T>)obj;
                return
                    Equals(Default, other.Default) &&
                        Equals(Min, other.Min) &&
                            Equals(Max, other.Max);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        private enum ByteEnum : byte
        {
            Min = Byte.MinValue,
            Max = Byte.MaxValue
        }

        private enum SByteEnum : sbyte
        {
            Min = SByte.MinValue,
            Max = SByte.MaxValue
        }

        private enum Int16Enum : short
        {
            Min = Int16.MinValue,
            Max = Int16.MaxValue
        }

        private enum UInt16Enum : ushort
        {
            Min = UInt16.MinValue,
            Max = UInt16.MaxValue
        }

        private enum Int32Enum
        {
            Min = Int32.MinValue,
            Max = Int32.MaxValue
        }

        private enum UInt32Enum : uint
        {
            Min = UInt32.MinValue,
            Max = UInt32.MaxValue
        }

        private enum Int64Enum : long
        {
            Min = Int64.MinValue,
            Max = Int64.MaxValue
        }

        private enum UInt64Enum : ulong
        {
            Min = UInt64.MinValue,
            Max = UInt64.MaxValue
        }

        public class PrimitiveDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return A(new ByteHolder());
                yield return A(new ByteEnumHolder());
                yield return A(new SByteHolder());
                yield return A(new SByteEnumHolder());
                yield return A(new Int16Holder());
                yield return A(new Int16EnumHolder());
                yield return A(new UInt16Holder());
                yield return A(new UInt16EnumHolder());
                yield return A(new Int32Holder());
                yield return A(new Int32EnumHolder());
                yield return A(new UInt32Holder());
                yield return A(new UInt32EnumHolder());
                yield return A(new Int64Holder());
                yield return A(new Int64EnumHolder());
                yield return A(new UInt64Holder());
                yield return A(new UInt64EnumHolder());
                yield return A(new SingleHolder());
                yield return A(new DoubleHolder());
                yield return A(new DecimalHolder());
                yield return A(new DateTimeHolder());
                yield return A(new TimeSpanHolder());
                yield return A(new CharHolder());
                yield return A(new StringHolder());
                yield return A(new BooleanHolder());
            }

            public static IEnumerable<object> GetData()
            {
                return new PrimitiveDataProvider().Select(x => x[0]);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(object value)
            {
                return new[] { value };
            }
        }

        [TypeId(35928738)]
        private class TypeWithComplexProperties
        {
            public TypeWithComplexProperties()
            {
                P0 = new Int16Holder();
                P1 = new Int32Holder();
            }

            [PropertyId(0)]
            public Int16Holder P0 { get; set; }

            [PropertyId(1)]
            public Int32Holder P1 { get; set; }

            public override bool Equals(object obj)
            {
                if(obj == null || obj.GetType() != GetType())
                {
                    return false;
                }
                if(obj == this)
                {
                    return true;
                }
                var other = (TypeWithComplexProperties)obj;
                return
                    Equals(P0, other.P0) &&
                        Equals(P1, other.P1);
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }

        public class ComplexDataProvider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return A(new TypeWithComplexProperties());
            }

            public static IEnumerable<object> GetData()
            {
                return new PrimitiveDataProvider().Select(x => x[0]);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(object value)
            {
                return new[] { value };
            }
        }
    }
}