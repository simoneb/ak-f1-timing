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
        [Fact(Skip="Need to re-write the collection")]
        public void collection_cannot_be_mutated_once_sealed() {

            var collection = new PropertyDescriptorCollection();
            var property0 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property0"));
            var property1 = PropertyDescriptor.For(typeof(DecoratedType).GetProperty("Property1"));

            collection.Add(property0);
            collection.Seal();
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
