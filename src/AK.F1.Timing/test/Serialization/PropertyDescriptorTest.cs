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
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Serialization
{
    public class PropertyDescriptorTest : TestBase
    {
        [Fact]
        public void can_get_the_descriptor_for_a_property() {

            var property = typeof(TypeWithPropertyWithPublicGetAndPublicSet).GetProperty("Property");
            var descriptor = PropertyDescriptor.For(property);

            Assert.NotNull(descriptor);
            Assert.Equal(property, descriptor.Property);
            Assert.Equal(1, descriptor.PropertyId);
        }

        [Fact]
        public void for_returns_same_instance_for_the_same_property() {

            var property = typeof(TypeWithPropertyWithPublicGetAndPublicSet).GetProperty("Property");
            var descriptor = PropertyDescriptor.For(property);

            Assert.NotNull(descriptor);
            Assert.Same(descriptor, PropertyDescriptor.For(property));
        }

        [Fact]
        public void can_set_the_value_of_a_property() {

            var component = new TypeWithPropertyWithPublicGetAndPublicSet();
            var descriptor = PropertyDescriptor.For(component.GetType().GetProperty("Property"));

            Assert.Equal(0, component.Property);
            descriptor.SetValue(component, 1);
            Assert.Equal(1, component.Property);
        }

        [Fact]
        public void can_set_the_value_of_an_inherited_property() {

            var component = new ChildType();
            var descriptor = PropertyDescriptor.For(component.GetType().GetProperty("Property"));

            Assert.Equal(0, component.Property);
            descriptor.SetValue(component, 1);
            Assert.Equal(1, component.Property);
        }

        [Fact]
        public void set_value_throws_if_component_is_null() {

            var property = typeof(TypeWithPropertyWithPublicGetAndPublicSet).GetProperty("Property");
            var descriptor = PropertyDescriptor.For(property);

            Assert.Throws<ArgumentNullException>(() => descriptor.SetValue(null, 1));
        }

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
                PropertyDescriptor.For(typeof(TypeWithUndecoratedProperty).GetProperty("Property"));
            });
        }

        [Fact]
        public void for_throws_if_property_is_decorated_but_declaring_type_is_not() {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(typeof(UndecoratedTypeWithDecoratedProperty).GetProperty("Property"));
            });
        }

        [Theory]
        [InlineData(typeof(TypeWithPropertyWithNoGet))]
        [InlineData(typeof(TypeWithPropertyWithNoSet))]
        public void for_throws_if_property_does_not_have_a_get_and_set(Type declaringType) {

            Assert.Throws<SerializationException>(() => {
                PropertyDescriptor.For(declaringType.GetProperty("Property"));
            });
        }

        [Theory]
        [InlineData(typeof(TypeWithPropertyWithPublicGetAndPublicSet))]
        [InlineData(typeof(TypeWithPropertyWithPrivateGetAndPublicSet))]
        [InlineData(typeof(TypeWithPropertyWithPublicGetAndPrivateSet))]
        public void for_does_not_throw_if_property_has_get_and_set(Type declaringType) {

            Assert.DoesNotThrow(() => {
                Assert.NotNull(PropertyDescriptor.For(declaringType.GetProperty("Property")));
            });
        }

        [Fact]
        public void can_serialize_descriptor() {

            var property = typeof(TypeWithPropertyWithPublicGetAndPublicSet).GetProperty("Property");
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

            var scope = typeof(TypeWithPropertyWithPublicGetAndPublicSet);

            yield return PropertyDescriptor.For(scope.GetProperty("Property"));
            yield return PropertyDescriptor.For(scope.GetProperty("Property")).DeepClone();
        }

        private IEnumerable<PropertyDescriptor> GetDistinctInstances() {

            foreach(var property in typeof(TypeWithSameProperties1).GetProperties()) {
                yield return PropertyDescriptor.For(property);
            }
            foreach(var property in typeof(TypeWithSameProperties2).GetProperties()) {
                yield return PropertyDescriptor.For(property);
            }
        }

        private sealed class UndecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property { get; set; }
        }

        [TypeId(59414808)]
        private sealed class TypeWithUndecoratedProperty
        {
            public int Property { get; set; }
        }

        [TypeId(31739516)]
        private sealed class TypeWithPropertyWithPublicGetAndPrivateSet
        {
            [PropertyId(0)]
            public int Property { set; private get; }
        }

        [TypeId(98239245)]
        private sealed class TypeWithPropertyWithPrivateGetAndPublicSet
        {
            [PropertyId(0)]
            public int Property { private set; get; }
        }

        [TypeId(65526584)]
        private sealed class TypeWithPropertyWithPublicGetAndPublicSet
        {
            [PropertyId(1)]
            public int Property { get; set; }
        }

        [TypeId(-80807557)]
        private sealed class TypeWithPropertyWithNoGet
        {
            [PropertyId(0)]
            public int Property { set { } }
        }

        [TypeId(-40412810)]
        private sealed class TypeWithPropertyWithNoSet
        {
            [PropertyId(1)]
            public int Property { get { return 0; } }
        }

        [TypeId(61112946)]
        private sealed class TypeWithSameProperties1
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
        private sealed class TypeWithSameProperties2
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

        [TypeId(-40412810)]
        private class TypeWithProperty
        {
            [PropertyId(0)]
            public int Property { get; set; }
        }

        [TypeId(-98367102)]
        private class ChildType : TypeWithProperty { }
    }
}
