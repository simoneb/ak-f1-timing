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
using System.Reflection;
using Xunit;

using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Serialization
{
    public class PropertyDescriptorCollectionTest : TestBase
    {
        [Fact]
        public void can_create() {

            var property0 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property0"));
            var property1 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property1"));
            var collection = new PropertyDescriptorCollection(new[] { property0, property1 });

            Assert.True(collection.Contains(property0));
            Assert.True(collection.Contains(property1));
            Assert.Equal(2, collection.Count);
            Assert.Equal(0, collection.IndexOf(property0));
            Assert.Equal(1, collection.IndexOf(property1));
        }

        [Fact]
        public void can_get_a_property_by_its_id() {

            var property0 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property0"));
            var property1 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property1"));
            var collection = new PropertyDescriptorCollection(new[] { property0, property1 });

            Assert.Equal(property0, collection.GetById(0));
            Assert.Equal(property1, collection.GetById(1));
        }

        [Fact]
        public void get_by_id_returns_null_if_no_property_exists_with_specified_id() {

            var collection = new PropertyDescriptorCollection(new PropertyDescriptor[0]);

            Assert.Null(collection.GetById(5));
        }

        [Fact]
        public void collection_cannot_be_mutated() {
            
            var property0 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property0"));
            var property1 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property1"));
            IList<PropertyDescriptor> collection = new PropertyDescriptorCollection(new[] { property0, property1 });
            
            Assert.Throws<NotSupportedException>(() => collection.Add(property1));
            Assert.Throws<NotSupportedException>(() => collection.Clear());
            Assert.Throws<NotSupportedException>(() => collection.Insert(0, property1));
            Assert.Throws<NotSupportedException>(() => collection.Remove(property0));
            Assert.Throws<NotSupportedException>(() => collection.RemoveAt(0));
        }

        [TypeId(7013143)]
        private sealed class DecoratedType
        {
            [PropertyId(0)]
            public int Property0 { get; set; }

            [PropertyId(1)]
            public int Property1 { get; set; }
        }
    }
}
