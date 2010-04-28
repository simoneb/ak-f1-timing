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
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Model.Driver
{
    public class PitTimesModelTest : TestClass
    {
        [Fact]
        public void can_create() {

            var model = new PitTimesModel();

            Assert.Empty(model);
        }

        [Fact]
        public void can_add_pit_time_to_items() {

            var time = new PitTimeModel(TimeSpan.FromSeconds(35.6), 3);
            var model = new PitTimesModel();

            model.Add(time);
            Assert.NotEmpty(model.Items);
            Assert.Same(time, model.Items[0]);            
        }

        [Fact]
        public void add_throws_if_time_is_null() {

            var model = new PitTimesModel();

            Assert.Throws<ArgumentNullException>(() => model.Add(null));
        }

        [Fact]
        public void adding_an_item_updates_the_count_property_if_it_is_less_than_the_number_of_items() {

            var model = new PitTimesModel();
            var time = new PitTimeModel(TimeSpan.FromSeconds(28.9), 14);

            model.Add(time);

            Assert.Equal(1, model.Count);

            model.Count = 3;
            model.Add(time);

            Assert.Equal(3, model.Count);

            model.Add(time);
            model.Add(time);

            Assert.Equal(4, model.Count);
        }

        [Fact]
        public void changes_to_the_count_property_raise_the_property_changed_event() {

            var model = new PitTimesModel();
            var observer = new PropertyChangeObserver<PitTimesModel>(model);

            model.Count = 1;
            Assert.True(observer.HasChanged(x => x.Count));
        }
    }
}