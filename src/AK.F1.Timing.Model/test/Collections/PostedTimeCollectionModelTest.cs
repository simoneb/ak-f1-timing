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
using AK.F1.Timing.Messages.Driver;
using Xunit;

namespace AK.F1.Timing.Model.Collections
{
    public class PostedTimeCollectionModelTest
    {
        [Fact]
        public void can_create()
        {
            var model = new PostedTimeCollectionModel();

            assert_has_default_property_values(model);
        }

        [Fact]
        public void can_reset()
        {
            var model = new PostedTimeCollectionModel();

            model.Add(PT(65d, PostedTimeType.PersonalBest, 5));
            model.Reset();
            assert_has_default_property_values(model);
        }

        [Fact]
        public void add_throws_if_item_is_null()
        {
            var model = new PostedTimeCollectionModel();

            Assert.Throws<ArgumentNullException>(() => model.Add(null));
        }

        [Fact]
        public void replace_current_throws_if_replacement_is_null()
        {
            var model = new PostedTimeCollectionModel();

            Assert.Throws<ArgumentNullException>(() => model.ReplaceCurrent(null));
        }

        [Fact]
        public void replace_current_throws_if_current_has_not_been_set()
        {
            var model = new PostedTimeCollectionModel();

            Assert.Throws<InvalidOperationException>(() => model.ReplaceCurrent(PT(10, PostedTimeType.Normal, 1)));
        }

        private static void assert_has_default_property_values(PostedTimeCollectionModel model)
        {
            Assert.Equal(0, model.Count);
            Assert.Null(model.Current);
            Assert.Null(model.CurrentDelta);
            Assert.Null(model.Maximum);
            Assert.Null(model.Mean);
            Assert.Null(model.Minimum);
            Assert.Equal(0, model.PersonalBestCount);
            Assert.Null(model.Previous);
            Assert.Null(model.Range);
            Assert.Equal(0, model.SessionBestCount);
            Assert.Empty(model.Items);
        }

        [Fact]
        public void adding_an_item_updates_the_count()
        {
            var context = new TestContext();
            var model = context.Model;

            Assert.Equal(0, model.Count);
            model.Add(PT(35d, PostedTimeType.Normal, 5));
            Assert.Equal(1, model.Count);
            model.Add(PT(35d, PostedTimeType.Normal, 5));
            Assert.Equal(2, model.Count);

            Assert.Equal(2, context.GetChangeCount(x => x.Count));
        }

        [Fact]
        public void replacing_an_item_does_not_update_the_count()
        {
            var context = CreateReplaceTestContext();

            context.Model.ReplaceCurrent(PT(10, PostedTimeType.Normal, 1));

            Assert.Equal(1, context.Model.Count);

            Assert.Equal(0, context.GetChangeCount(x => x.Count));
        }

        [Fact]
        public void adding_an_item_updates_the_current()
        {
            var context = new TestContext();
            var model = context.Model;
            var current = PT(35d, PostedTimeType.Normal, 5);

            model.Add(current);
            Assert.Equal(current, model.Current);
            current = PT(65d, PostedTimeType.Normal, 5);
            model.Add(current);
            Assert.Equal(current, model.Current);

            Assert.Equal(2, context.GetChangeCount(x => x.Current));
        }

        [Fact]
        public void replacing_an_item_updates_the_current()
        {
            var context = CreateReplaceTestContext();
            var replacement = PT(10, PostedTimeType.Normal, 1);

            context.Model.Add(replacement);
            Assert.Equal(replacement, context.Model.Current);

            Assert.Equal(1, context.GetChangeCount(x => x.Current));
        }

        [Fact]
        public void adding_an_item_updates_the_maximum_if_it_has_changed()
        {
            PostedTime item;
            TestContext context = new TestContext();
            PostedTimeCollectionModel model = context.Model;

            for(double seconds = 1d; seconds < 6d; ++seconds)
            {
                item = PT(seconds, PostedTimeType.Normal, 5);
                model.Add(item);
                Assert.Equal(item, model.Maximum);
                model.Add(PT(seconds - 1, PostedTimeType.Normal, 5));
                Assert.Equal(item, model.Maximum);
            }

            Assert.Equal(5, context.GetChangeCount(x => x.Maximum));
        }

        [Fact]
        public void replacing_an_item_updates_the_maximum_if_it_has_changed()
        {
            var context = CreateReplaceTestContext();
            var replacement = PT(0, PostedTimeType.Normal, 1);

            context.Model.ReplaceCurrent(replacement);
            Assert.NotEqual(replacement, context.Model.Maximum);

            replacement = PT(60, PostedTimeType.Normal, 1);
            context.Model.ReplaceCurrent(replacement);
            Assert.Equal(replacement, context.Model.Maximum);

            Assert.Equal(1, context.GetChangeCount(x => x.Maximum));
        }

        [Fact]
        public void adding_an_item_updates_the_minimum_if_it_has_changed()
        {
            PostedTime item;
            TestContext context = new TestContext();
            PostedTimeCollectionModel model = context.Model;

            for(double seconds = 5d; seconds >= 0d; --seconds)
            {
                item = PT(seconds, PostedTimeType.Normal, 5);
                model.Add(item);
                Assert.Equal(item, model.Minimum);
                model.Add(PT(seconds + 1, PostedTimeType.Normal, 5));
                Assert.Equal(item, model.Minimum);
            }

            Assert.Equal(6, context.GetChangeCount(x => x.Minimum));
        }

        [Fact]
        public void replacing_an_item_updates_the_minimum_if_it_has_changed()
        {
            var context = CreateReplaceTestContext();
            var replacement = PT(2, PostedTimeType.Normal, 1);

            context.Model.ReplaceCurrent(replacement);
            Assert.NotEqual(replacement, context.Model.Minimum);

            replacement = PT(0, PostedTimeType.Normal, 1);
            context.Model.ReplaceCurrent(replacement);
            Assert.Equal(replacement, context.Model.Minimum);

            Assert.Equal(1, context.GetChangeCount(x => x.Minimum));
        }

        [Fact]
        public void adding_an_item_updates_the_mean()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(1d, PostedTimeType.Normal, 1));
            Assert.Equal(TS(1d), model.Mean);
            model.Add(PT(1d, PostedTimeType.Normal, 1));
            Assert.Equal(TS(1d), model.Mean);
            model.Add(PT(4d, PostedTimeType.Normal, 1));
            Assert.Equal(TS(2d), model.Mean);

            Assert.Equal(2, context.GetChangeCount(x => x.Mean));
        }

        [Fact]
        public void replacing_an_item_updates_the_mean()
        {
            var context = CreateReplaceTestContext();
            var model = context.Model;

            model.ReplaceCurrent(PT(10, PostedTimeType.Normal, 1));
            Assert.Equal(TS(10), model.Mean);
        }

        [Fact]
        public void adding_an_item_updates_the_range()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(1d, PostedTimeType.Normal, 1));
            Assert.Equal(TS(0d), model.Range);
            model.Add(PT(10d, PostedTimeType.Normal, 1));
            Assert.Equal(TS(9d), model.Range);

            Assert.Equal(2, context.GetChangeCount(x => x.Range));
        }

        [Fact]
        public void replacing_an_item_updates_the_range()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(10, PostedTimeType.Normal, 1));
            Assert.Equal(TS(0), model.Range);
            model.ReplaceCurrent(PT(10, PostedTimeType.Normal, 1));
            Assert.Equal(TS(0), model.Range);
            model.Add(PT(20, PostedTimeType.Normal, 1));
            model.ReplaceCurrent(PT(10, PostedTimeType.Normal, 1));
            Assert.Equal(TS(10), model.Range);

            Assert.Equal(2, context.GetChangeCount(x => x.Range));
        }

        [Fact]
        public void adding_an_item_updates_the_personal_best_count()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(15, PostedTimeType.PersonalBest, 1));
            Assert.Equal(1, model.PersonalBestCount);

            Assert.Equal(1, context.GetChangeCount(x => x.PersonalBestCount));
        }

        [Fact]
        public void replacing_an_item_updates_the_personal_best_count()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(15, PostedTimeType.PersonalBest, 1));
            Assert.Equal(1, model.PersonalBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.PersonalBest, 1));
            Assert.Equal(1, model.PersonalBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.Normal, 1));
            Assert.Equal(0, model.PersonalBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.PersonalBest, 1));
            Assert.Equal(1, model.PersonalBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.SessionBest, 1));
            Assert.Equal(0, model.PersonalBestCount);

            Assert.Equal(4, context.GetChangeCount(x => x.PersonalBestCount));
        }

        [Fact]
        public void adding_an_item_updates_the_session_best_count()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(15, PostedTimeType.SessionBest, 1));
            Assert.Equal(1, model.SessionBestCount);

            Assert.Equal(1, context.GetChangeCount(x => x.SessionBestCount));
        }

        [Fact]
        public void replacing_an_item_updates_the_session_best_count()
        {
            var context = new TestContext();
            var model = context.Model;

            model.Add(PT(15, PostedTimeType.SessionBest, 1));
            Assert.Equal(1, model.SessionBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.SessionBest, 1));
            Assert.Equal(1, model.SessionBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.Normal, 1));
            Assert.Equal(0, model.SessionBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.SessionBest, 1));
            Assert.Equal(1, model.SessionBestCount);
            model.ReplaceCurrent(PT(15, PostedTimeType.PersonalBest, 1));
            Assert.Equal(0, model.SessionBestCount);

            Assert.Equal(4, context.GetChangeCount(x => x.SessionBestCount));
        }

        [Fact]
        public void adding_an_item_adds_it_to_the_items()
        {
            var item = PT(32, PostedTimeType.Normal, 5);
            var model = new PostedTimeCollectionModel();

            model.Add(item);
            Assert.Equal(1, model.Items.Count);
            Assert.Equal(item, model.Items[0]);
        }

        [Fact]
        public void replacing_an_item_updates_the_items()
        {
            var context = CreateReplaceTestContext();
            var replacement = PT(50, PostedTimeType.Normal, 1);

            context.Model.ReplaceCurrent(replacement);
            Assert.Equal(replacement, context.Model.Items[0]);
        }

        private static TestContext CreateReplaceTestContext()
        {
            var context = new TestContext();

            context.Model.Add(PT(1, PostedTimeType.Normal, 1));
            context.Observer.ClearChanges();

            return context;
        }

        private static TimeSpan TS(double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        private static PostedTime PT(double seconds, PostedTimeType type, int lapNumber)
        {
            return new PostedTime(TS(seconds), type, lapNumber);
        }

        private sealed class TestContext
        {
            public readonly PostedTimeCollectionModel Model;
            public readonly PropertyChangeObserver<PostedTimeCollectionModel> Observer;

            public TestContext()
            {
                Model = new PostedTimeCollectionModel();
                Observer = new PropertyChangeObserver<PostedTimeCollectionModel>(Model);
            }

            public int GetChangeCount<TResult>(Expression<Func<PostedTimeCollectionModel, TResult>> expression)
            {
                return Observer.GetChangeCount(expression);
            }
        }
    }
}