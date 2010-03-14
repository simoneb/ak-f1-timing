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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Serialization
{
    public class PropertyDescriptorTest : TestBase
    {
        [Fact]
        public void for_throws_if_property_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                PropertyDescriptor.For(null);
            });
        }

        [Fact]
        public void for_throws_if_property_is_not_decorated_with_property_id_attribute() {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(string).GetProperty("Length"));
            });
            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(DecoratedTypeWithUndecoratedProperty).GetProperty("Property"));
            });
        }

        [Fact(Skip = "Needs to pass")]
        public void for_throws_if_property_is_decorated_but_declaring_type_is_not() {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(UndecoratedTypeWithDecoratedProperty).GetProperty("Property"));
            });
        }

        [Theory]
        [InlineData("PropertyWithNoGet")]
        [InlineData("PropertyWithNoSet")]
        public void for_throws_if_property_does_not_have_a_get_and_set(string propertyName) {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(DecoratedType).GetProperty(propertyName));
            });
        }

        [Theory]
        [InlineData("PropertyWithPublicGetAndPublicSet")]
        [InlineData("PropertyWithPrivateGetAndPublicSet")]
        [InlineData("PropertyWithPublicGetAndPrivateSet")]
        public void for_does_not_throw_if_property_has_get_and_set(string propertyName) {

            Assert.DoesNotThrow(() => {
                Assert.NotNull(PropertyDescriptor.For(typeof(DecoratedType).GetProperty(propertyName)));
            });
        }

        [Fact]
        public void can_serialize_descriptor() {

            var property = typeof(DecoratedType).GetProperty("PropertyWithPublicGetAndPublicSet");
            PropertyDescriptor expected = PropertyDescriptor.For(property);
            PropertyDescriptor actual = null;

            Assert.DoesNotThrow(() => {
                actual = expected.DeepClone();
            });

            Assert.NotNull(actual);
            Assert.Equal(expected.Property, actual.Property);
            Assert.Equal(expected.PropertyId, actual.PropertyId);
        }

        [Fact]
        public void implements_equality_contract() {

            Assert.EqualityContract(GetEquivalentInstances(), GetDistinctInstances());
        }

        private static IEnumerable<PropertyDescriptor> GetEquivalentInstances() {

            var scope = typeof(DecoratedType);

            yield return PropertyDescriptor.For(scope.GetProperty("PropertyWithPublicGetAndPublicSet"));
            yield return PropertyDescriptor.For(scope.GetProperty("PropertyWithPublicGetAndPublicSet"));
            yield return PropertyDescriptor.For(scope.GetProperty("PropertyWithPublicGetAndPublicSet")).DeepClone();
        }

        private IEnumerable<PropertyDescriptor> GetDistinctInstances() {

            foreach(var property in typeof(DecoratedTypeWithSameProperties1).GetProperties()) {
                yield return PropertyDescriptor.For(property);
            }
            foreach(var property in typeof(DecoratedTypeWithSameProperties2).GetProperties()) {
                yield return PropertyDescriptor.For(property);
            }
        }

        private sealed class UndecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property { get; set; }
        }

        [TypeId(59414808)]
        private sealed class DecoratedTypeWithUndecoratedProperty
        {
            public int Property { get; set; }
        }

        [TypeId(31739516)]
        private sealed class DecoratedType
        {
            [PropertyId(0)]
            public int PropertyWithPublicGetAndPublicSet { get; set; }

            [PropertyId(1)]
            public int PropertyWithNoGet { get { return 0; } }

            [PropertyId(2)]
            public int PropertyWithNoSet { set { } }

            [PropertyId(3)]
            public int PropertyWithPrivateGetAndPublicSet { private set; get; }

            [PropertyId(4)]
            public int PropertyWithPublicGetAndPrivateSet { set; private get; }
        }

        [TypeId(61112946)]
        private sealed class DecoratedTypeWithSameProperties1
        {
            [PropertyId(0)]
            public int Property1 { get; set; }

            [PropertyId(1)]
            public int Property2 { get; set; }

            [PropertyId(2)]
            public int Property3 { get; set; }

            [PropertyId(3)]
            public int Property4 { get; set; }

            [PropertyId(4)]
            public int Property5 { get; set; }
        }

        [TypeId(32378927)]
        private sealed class DecoratedTypeWithSameProperties2
        {
            [PropertyId(0)]
            public int Property1 { get; set; }

            [PropertyId(1)]
            public int Property2 { get; set; }

            [PropertyId(2)]
            public int Property3 { get; set; }

            [PropertyId(3)]
            public int Property4 { get; set; }

            [PropertyId(4)]
            public int Property5 { get; set; }
        }
    }
}
