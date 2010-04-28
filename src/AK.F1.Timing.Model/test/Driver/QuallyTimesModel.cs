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
    public class QuallyTimesModelTest : TestClass
    {
        [Fact]
        public void can_create() {

            var model = new QuallyTimesModel();

            Assert.Empty(model);
        }

        [Fact]
        public void can_set_a_qually_time() {

            TimeSpan time;
            var model = new QuallyTimesModel();

            time = TimeSpan.FromSeconds(1d);
            model.SetTime(1, time);
            Assert.Equal(time, model.Q1);
            model.SetTime(1, null);
            Assert.Null(model.Q1);

            time = TimeSpan.FromSeconds(2d);
            model.SetTime(2, time);
            Assert.Equal(time, model.Q2);
            model.SetTime(2, null);
            Assert.Null(model.Q2);

            time = TimeSpan.FromSeconds(3d);
            model.SetTime(3, time);
            Assert.Equal(time, model.Q3);
            model.SetTime(3, null);
            Assert.Null(model.Q3);
        }

        [Fact]
        public void changes_to_the_qually_time_properties_raise_the_property_changed_event() {

            var model = new QuallyTimesModel();
            var observer = new PropertyChangeObserver<QuallyTimesModel>(model);

            model.SetTime(1, TimeSpan.FromSeconds(1d));
            Assert.True(observer.HasChanged(x => x.Q1));

            model.SetTime(2, TimeSpan.FromSeconds(2d));
            Assert.True(observer.HasChanged(x => x.Q2));

            model.SetTime(3, TimeSpan.FromSeconds(3d));
            Assert.True(observer.HasChanged(x => x.Q3));
        }

        [Fact]
        public void can_get_a_qually_time() {

            TimeSpan time;
            var model = new QuallyTimesModel();

            time = TimeSpan.FromSeconds(1d);
            model.SetTime(1, time);
            Assert.Equal(time, model.GetTime(1));

            time = TimeSpan.FromSeconds(2d);
            model.SetTime(2, time);
            Assert.Equal(time, model.GetTime(2));

            time = TimeSpan.FromSeconds(3d);
            model.SetTime(3, time);
            Assert.Equal(time, model.GetTime(3));  
        }
    }
}