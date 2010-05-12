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
using System.Runtime.Serialization;
using Xunit;

namespace AK.F1.Timing.Serialization
{
    public class TypeDescriptorTest : TestBase
    {
        [Fact]
        public void implements_equality_contract()
        {
            Assert.EqualityContract(
                new[] {TypeDescriptor.For(typeof(TypeWithTwoProperties)), TypeDescriptor.For(typeof(TypeWithTwoProperties))},
                new[] {TypeDescriptor.For(typeof(TypeWithProperty)), TypeDescriptor.For(typeof(TypeWithTwoProperties))}
                );
        }

        [Fact]
        public void can_get_the_descriptor_for_a_type()
        {
            var type = typeof(TypeWithTwoProperties);
            var descriptor = TypeDescriptor.For(type);

            Assert.NotNull(descriptor);
            Assert.Equal(2, descriptor.Properties.Count);
            Assert.Equal(type, descriptor.Type);
            Assert.Equal(57287559, descriptor.TypeId);
        }

        [Fact]
        public void for_returns_the_same_descriptor_for_the_same_type()
        {
            var type = typeof(TypeWithTwoProperties);

            Assert.Same(TypeDescriptor.For(type), TypeDescriptor.For(type));
        }

        [Fact]
        public void properties_decorated_with_ignore_property_are_ignored()
        {
            var descriptor = TypeDescriptor.For(typeof(TypeWithIgnoredProperty));

            Assert.Empty(descriptor.Properties);
        }

        [Fact]
        public void for_throws_if_type_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => { TypeDescriptor.For(null); });
        }

        [Fact]
        public void for_throws_if_type_is_not_decorated()
        {
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(UndecoratedType)); });
        }

        [Fact]
        public void for_throws_if_type_is_not_decorated_with_type_id_attribute()
        {
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(string)); });
        }

        [Fact]
        public void for_throws_if_type_is_decorated_but_one_or_more_properties_are_not()
        {
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(TypeWithAnUndecoratedProperty)); });
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(TypeWithAnUndecoratedAndDecoratedProperty)); });
        }

        [Fact]
        public void for_throws_if_type_has_duplicate_properties()
        {
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(TypeWithDuplicateProperties)); });
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(ChildTypeWithSamePropertyAsParent)); });
        }

        [Fact]
        public void for_throws_if_base_type_is_decorated_but_type_is_not()
        {
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(UndecoratedChildOfDecoratedType)); });
        }

        [Fact]
        public void for_throws_if_type_has_already_been_registered_with_the_same_id()
        {
            TypeDescriptor.For(typeof(TypeWithProperty));
            Assert.Throws<SerializationException>(() => { TypeDescriptor.For(typeof(TypeWithDuplicateTypeId)); });
        }

        private sealed class UndecoratedType
        {
            [PropertyId(0)]
            public int Property0 { get; set; }
        }

        [TypeId(-2694475)]
        private sealed class TypeWithAnUndecoratedProperty
        {
            public int Property0 { get; set; }
        }

        [TypeId(-9215470)]
        private sealed class TypeWithAnUndecoratedAndDecoratedProperty
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            public int Property1 { get; set; }
        }

        [TypeId(-7507244)]
        private sealed class TypeWithDuplicateProperties
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            [PropertyId(0)]
            public int Property1 { get; set; }
        }

        [TypeId(1685648)]
        private class TypeWithProperty
        {
            [PropertyId(0)]
            public int Property0 { get; set; }
        }

        [TypeId(57287559)]
        private class TypeWithTwoProperties
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            [PropertyId(1)]
            public int Property1 { get; set; }
        }

        [TypeId(-75030885)]
        private class TypeWithIgnoredProperty
        {
            [IgnoreProperty]
            public int Property1 { get; set; }
        }

        [TypeId(1685648)]
        private class TypeWithDuplicateTypeId
        {
            [PropertyId(0)]
            public int Property0 { get; set; }
        }

        [TypeId(-7211818)]
        private class ChildTypeWithSamePropertyAsParent : TypeWithProperty
        {
            [PropertyId(0)]
            public int Property1 { get; set; }
        }

        private sealed class UndecoratedChildOfDecoratedType : TypeWithProperty {}
    }
}