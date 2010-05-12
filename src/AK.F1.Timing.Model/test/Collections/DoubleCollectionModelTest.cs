// Copyright 2010 Andy Kernahan
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
using System.Linq.Expressions;
using Xunit;

namespace AK.F1.Timing.Model.Collections
{
    public class DoubleCollectionModelTest
    {
        [Fact]
        public void can_create()
        {
            var model = new DoubleCollectionModel();

            assert_has_default_property_values(model);
        }

        [Fact]
        public void can_reset()
        {
            var model = new DoubleCollectionModel();

            model.Add(1d);
            model.Add(2d);
            model.Reset();
            assert_has_default_property_values(model);
        }

        [Fact]
        public void reset_raises_begin_and_end_events()
        {
            var resetBeginRaised = false;
            var resetCompleteRaised = false;
            var model = new DoubleCollectionModel();

            model.ResetBegin += delegate
            {
                Assert.False(resetBeginRaised);
                Assert.False(resetCompleteRaised);
                resetBeginRaised = true;
            };
            model.ResetComplete += delegate
            {
                Assert.True(resetBeginRaised);
                Assert.False(resetCompleteRaised);
                resetCompleteRaised = true;
            };
            model.Reset();
            Assert.True(resetBeginRaised && resetCompleteRaised);
        }

        [Fact]
        public void add_throws_if_item_a_not_a_number()
        {
            var model = new DoubleCollectionModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => model.Add(Double.NaN));
        }

        private static void assert_has_default_property_values(DoubleCollectionModel model)
        {
            Assert.Equal(0, model.Count);
            Assert.Null(model.Current);
            Assert.Equal(DeltaType.None, model.CurrentDeltaType);
            Assert.Null(model.Maximum);
            Assert.Null(model.Mean);
            Assert.Equal(DeltaType.None, model.MeanDeltaType);
            Assert.Null(model.Minimum);
            Assert.Null(model.Range);
            Assert.Null(model.StandardDeviation);
            Assert.Empty(model.Items);
        }

        [Fact]
        public void adding_an_item_updates_the_count()
        {
            var context = new TestContext();
            var model = context.Model;

            Assert.Equal(0, model.Count);
            model.Add(1d);
            Assert.Equal(1, model.Count);
            model.Add(1d);
            Assert.Equal(2, model.Count);

            Assert.Equal(2, context.GetChangeCount(x => x.Count));
        }

        [Fact]
        public void adding_an_item_updates_the_current()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(1d, model.Current);
            model.Add(2d);
            Assert.Equal(2d, model.Current);

            Assert.Equal(2, context.GetChangeCount(x => x.Current));
        }

        [Fact]
        public void adding_an_item_updates_the_current_delta_type_if_it_has_changed()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(DeltaType.None, model.CurrentDeltaType);
            model.Add(2d);
            Assert.Equal(DeltaType.Increase, model.CurrentDeltaType);
            model.Add(1d);
            Assert.Equal(DeltaType.Decrease, model.CurrentDeltaType);
            model.Add(1d);
            Assert.Equal(DeltaType.None, model.CurrentDeltaType);

            Assert.Equal(3, context.GetChangeCount(x => x.CurrentDeltaType));
        }

        [Fact]
        public void adding_an_item_updates_the_maximum_if_it_has_changed()
        {
            var context = new TestContext();
            var model = context.Model;

            for(double item = -5d; item < 5d; ++item)
            {
                model.Add(item);
                Assert.Equal(item, model.Maximum);
                model.Add(item - 1);
                Assert.Equal(item, model.Maximum);
            }

            Assert.Equal(10, context.GetChangeCount(x => x.Maximum));
        }

        [Fact]
        public void adding_an_item_updates_the_minimum_if_it_has_changed()
        {
            var context = new TestContext();
            var model = context.Model;

            for(double item = 5d; item > -5d; --item)
            {
                model.Add(item);
                Assert.Equal(item, model.Minimum);
                model.Add(item + 1);
                Assert.Equal(item, model.Minimum);
            }

            Assert.Equal(10, context.GetChangeCount(x => x.Minimum));
        }

        [Fact]
        public void adding_an_item_updates_the_mean()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(1d, model.Mean);
            model.Add(1d);
            Assert.Equal(1d, model.Mean);
            model.Add(4d);
            Assert.Equal(2d, model.Mean);

            Assert.Equal(2, context.GetChangeCount(x => x.Mean));
        }

        [Fact]
        public void adding_an_item_updates_the_mean_delta_type_if_it_has_changed()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(10d);
            Assert.Equal(DeltaType.None, model.MeanDeltaType);
            model.Add(10d);
            Assert.Equal(DeltaType.None, model.MeanDeltaType);
            model.Add(20d);
            Assert.Equal(DeltaType.Increase, model.MeanDeltaType);
            model.Add(5d);
            Assert.Equal(DeltaType.Decrease, model.MeanDeltaType);

            Assert.Equal(2, context.GetChangeCount(x => x.MeanDeltaType));
        }

        [Fact]
        public void adding_an_item_updates_the_range()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(1d);
            Assert.Equal(0d, model.Range);
            model.Add(10d);
            Assert.Equal(9d, model.Range);

            Assert.Equal(2, context.GetChangeCount(x => x.Range));

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

            Assert.Equal(4, context.GetChangeCount(x => x.Range));
        }

        [Fact]
        public void adding_an_item_updates_the_standard_deviation()
        {
            var context = new TestContext();
            var model = context.Model;

            for(double item = 0d; item < 10d; ++item)
            {
                context.Model.Add(item);
            }

            Assert.Equal(2.8722813232690143, model.StandardDeviation);
            Assert.Equal(10, context.GetChangeCount(x => x.StandardDeviation));
        }

        [Fact]
        public void adding_an_item_adds_it_to_the_items()
        {
            var model = new DoubleCollectionModel();

            model.Add(1d);
            Assert.Equal(1, model.Items.Count);
            Assert.Equal(1d, model.Items[0]);
        }

        private sealed class TestContext
        {
            public readonly DoubleCollectionModel Model;
            public readonly PropertyChangeObserver<DoubleCollectionModel> Observer;

            public TestContext()
            {
                Model = new DoubleCollectionModel();
                Observer = new PropertyChangeObserver<DoubleCollectionModel>(Model);
            }

            public int GetChangeCount<TResult>(Expression<Func<DoubleCollectionModel, TResult>> expression)
            {
                return Observer.GetChangeCount(expression);
            }
        }
    }
}