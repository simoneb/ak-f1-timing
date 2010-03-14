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

using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Serialization
{
    public class TypeDescriptorTest : TestBase
    {
        [Fact]
        public void implements_equality_contract() {

            
        }

        [Fact]
        public void for_throws_if_type_is_null() {

            Assert.Throws<ArgumentNullException>(() => {
                TypeDescriptor.For(null);
            });
        }

        [Fact]
        public void for_throws_if_type_is_not_decorated_with_type_id_attribute() {

            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(string));
            });
        }

        [Fact]
        public void for_throws_if_type_is_decorated_but_one_or_properties_properties_are_not() {

            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(DecoratedTypeWithAnUndecoratedProperty));
            });
            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(DecoratedTypeWithAnUndecoratedAndDecoratedProperty));
            });
        }

        [Fact(Skip = "Need to change exception type in collection.")]
        public void for_throws_if_type_has_duplicate_properties() {

            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(DecoratedTypeWithDuplicateProperties));
            });
            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(DecoratedChildTypeWithSamePropertyAsParent));
            });
        }

        [Fact]
        public void for_throws_if_base_type_is_decorated_but_type_is_not() {

            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(UndecoratedChildOfDecoratedType));
            });
        }

        [Fact]
        public void for_throws_if_type_has_already_been_registered_with_the_same_id() {

            TypeDescriptor.For(typeof(DecoratedTypeWithDecoratedProperty));
            Assert.Throws<SerializationException>(() => {
                TypeDescriptor.For(typeof(DecoratedTypeWithDuplicateTypeId));
            });
        }

        [TypeId(-2694475)]
        private sealed class DecoratedTypeWithAnUndecoratedProperty
        {
            public int Property0 { get; set; }
        }

        [TypeId(-9215470)]
        private sealed class DecoratedTypeWithAnUndecoratedAndDecoratedProperty
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            public int Property1 { get; set; }
        }

        [TypeId(-7507244)]
        private sealed class DecoratedTypeWithDuplicateProperties
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            [PropertyId(0)]
            public int Property1 { get; set; }
        }

        [TypeId(1685648)]
        private class DecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property0 { get; set; }
        }

        [TypeId(1685648)]
        private class DecoratedTypeWithDuplicateTypeId
        {
            [PropertyId(0)]
            public int Property0 { get; set; }
        }

        [TypeId(-7211818)]
        private class DecoratedChildTypeWithSamePropertyAsParent : DecoratedTypeWithDecoratedProperty
        {
            [PropertyId(0)]
            public int Property1 { get; set; }
        }

        private sealed class UndecoratedChildOfDecoratedType : DecoratedTypeWithDecoratedProperty
        {
        }
    }
}