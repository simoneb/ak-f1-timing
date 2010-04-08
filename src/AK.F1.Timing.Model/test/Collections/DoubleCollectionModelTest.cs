﻿// Copyright 2010 Andy Kernahan
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
using Xunit;

namespace AK.F1.Timing.Model.Collections
{
    public class DoubleCollectionModelTest
    {
        [Fact]
        public void can_create() {

            var model = new DoubleCollectionModel();

            assert_has_default_property_values(model);
        }

        [Fact]
        public void can_reset() {

            var model = new DoubleCollectionModel();

            model.Add(5d);            
            model.Reset();
            assert_has_default_property_values(model);
        }

        [Fact]
        public void add_throws_if_item_a_not_a_number() {

            var model = new DoubleCollectionModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => model.Add(Double.NaN));
        }

        private static void assert_has_default_property_values(DoubleCollectionModel model) {

            Assert.Equal(0, model.Count);
            Assert.Null(model.Current);
            Assert.Null(model.Maximum);
            Assert.Null(model.Mean);
            Assert.Null(model.Minimum);
            Assert.Null(model.Range);
            Assert.Null(model.StandardDeviation);
            Assert.Empty(model.Values);
        }

        [Fact]
        public void adding_an_item_updates_the_count() {

            var context = new TestContext();
            var model = context.Model;

            Assert.Equal(0, model.Count);
            model.Add(1d);
            Assert.Equal(1, model.Count);
            model.Add(1d);
            Assert.Equal(2, model.Count);

            Assert.Equal(2, context.Changes["Count"]);
        }

        [Fact]
        public void adding_an_item_updates_the_current() {

            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(1d, model.Current);
            model.Add(2d);
            Assert.Equal(2d, model.Current);

            Assert.Equal(2, context.Changes["Current"]);
        }

        [Fact]
        public void adding_an_item_updates_the_maximum_if_it_has_changed() {

            var context = new TestContext();
            var model = context.Model;

            for(double item = -5d; item < 5d; ++item) {
                model.Add(item);
                Assert.Equal(item, model.Maximum);
                model.Add(item - 1);
                Assert.Equal(item, model.Maximum);
            }

            Assert.Equal(10, context.Changes["Maximum"]);
        }

        [Fact]
        public void adding_an_item_updates_the_minimum_if_it_has_changed() {

            var context = new TestContext();
            var model = context.Model;

            for(double item = 5d; item > -5d; --item) {
                model.Add(item);
                Assert.Equal(item, model.Minimum);
                model.Add(item + 1);
                Assert.Equal(item, model.Minimum);
            }

            Assert.Equal(10, context.Changes["Minimum"]);
        }

        [Fact]
        public void adding_an_item_updates_the_mean() {

            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(1d, model.Mean);
            model.Add(1d);
            Assert.Equal(1d, model.Mean);
            model.Add(4d);
            Assert.Equal(2d, model.Mean);

            Assert.Equal(2, context.Changes["Mean"]);
        }

        [Fact]
        public void adding_an_item_updates_the_range() {

            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(0d, model.Range);
            model.Add(10d);
            Assert.Equal(9d, model.Range);

            Assert.Equal(2, context.Changes["Range"]);

            context = new TestContext();
            model = context.Model;

            model.Add(-20);
            Assert.Equal(0d, model.Range);
            model.Add(-10);
            Assert.Equal(10d, model.Range);
            model.Add(0d);
            Assert.Equal(20d, model.Range);
            model.Add(10d);
            Assert.Equal(30d, model.Range);

            Assert.Equal(4, context.Changes["Range"]);
        }

        [Fact]
        public void adding_an_item_updates_the_standard_deviation() {

            var context = new TestContext();
            var model = context.Model;

            for(double item = 0d; item < 10d; ++item) {
                context.Model.Add(item);
            }

            Assert.Equal(2.8722813232690143, model.StandardDeviation);
            Assert.Equal(10, context.Changes["StandardDeviation"]);
        }

        [Fact]
        public void adding_an_item_adds_it_to_the_values() {

            var model = new DoubleCollectionModel();

            model.Add(1d);
            Assert.Equal(1, model.Values.Count);
            Assert.Equal(1d, model.Values[0]);
        }

        private sealed class TestContext
        {
            public readonly DoubleCollectionModel Model = new DoubleCollectionModel();
            public readonly IDictionary<string, int> Changes = new Dictionary<string, int>(StringComparer.Ordinal);

            public TestContext() {

                Model.PropertyChanged += (s, e) => {
                    int count;
                    Changes.TryGetValue(e.PropertyName, out count);
                    Changes[e.PropertyName] = count + 1;
                };
            }
        }
    }
}