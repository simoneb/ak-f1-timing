// Copyright 2011 Andy Kernahan
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Model.Driver;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Model.Session
{
    public class SpeedCapturesModelTest : TestClass
    {
        [Fact]
        public void can_create()
        {
            var model = CreateModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void ctor_throws_if_driver_model_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new SpeedCapturesModel(null));
        }

        [Fact]
        public void can_reset()
        {
            var model = CreateModel(
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 300),
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S2, 300),
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S3, 300),
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.Straight, 300));

            model.Reset();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void can_get_a_capture_collection()
        {
            var model = CreateModel();

            Assert.Same(model.S1, model.GetCollection(SpeedCaptureLocation.S1));
            Assert.Same(model.S2, model.GetCollection(SpeedCaptureLocation.S2));
            Assert.Same(model.S3, model.GetCollection(SpeedCaptureLocation.S3));
            Assert.Same(model.Straight, model.GetCollection(SpeedCaptureLocation.Straight));
        }

        [Fact]
        public void get_capture_collection_throws_if_location_is_not_valid()
        {
            var model = CreateModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => model.GetCollection(SpeedCaptureLocation.Straight + 1));
        }

        [Theory]
        [ClassData(typeof(AllSpeedCaptureLocations))]
        public void processing_a_set_driver_speed_message_updates_the_correct_collection(
            SpeedCaptureLocation location)
        {
            var message = new SetDriverSpeedMessage(1, location, 300);
            var model = CreateModel(message);
            var collection = model.GetCollection(location);

            Assert.Equal(1, collection.Count);
            Assert.NotNull(collection[0].Driver);
            Assert.Equal(message.DriverId, collection[0].Driver.Id);
            Assert.Equal(message.Location, collection[0].Location);
            Assert.Equal(message.Speed, collection[0].Speed);
        }

        [Fact]
        public void capture_collections_maintain_the_fastest_speed_for_a_driver()
        {
            var model = CreateModel(
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 100),
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 300),
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 200));

            Assert.Equal(1, model.S1.Count);
            Assert.Equal(300, model.S1[0].Speed);
        }

        [Fact]
        public void capture_collections_are_sorted_by_speed_descending()
        {
            var model = CreateModel(
                new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 100),
                new SetDriverSpeedMessage(3, SpeedCaptureLocation.S1, 300),
                new SetDriverSpeedMessage(4, SpeedCaptureLocation.S1, 400),
                new SetDriverSpeedMessage(2, SpeedCaptureLocation.S1, 200));

            Assert.Equal(400, model.S1[0].Speed);
            Assert.Equal(300, model.S1[1].Speed);
            Assert.Equal(200, model.S1[2].Speed);
            Assert.Equal(100, model.S1[3].Speed);
        }

        [Fact]
        public void only_the_fastest_six_speeds_are_maintained()
        {
            Assert.Equal(6, SpeedCapturesModel.MaxCollectionSize);

            var model = CreateModel(
                new SetDriverSpeedMessage(2, SpeedCaptureLocation.S1, 200),
                new SetDriverSpeedMessage(3, SpeedCaptureLocation.S1, 300),
                new SetDriverSpeedMessage(4, SpeedCaptureLocation.S1, 400),
                new SetDriverSpeedMessage(5, SpeedCaptureLocation.S1, 500),
                new SetDriverSpeedMessage(6, SpeedCaptureLocation.S1, 600),
                new SetDriverSpeedMessage(7, SpeedCaptureLocation.S1, 700));

            model.Process(new SetDriverSpeedMessage(1, SpeedCaptureLocation.S1, 100));
            Assert.Equal(6, model.S1.Count);

            // Replace top.
            model.Process(new SetDriverSpeedMessage(8, SpeedCaptureLocation.S1, 800));
            Assert.Equal(6, model.S1.Count);
            Assert.Equal(800, model.S1[0].Speed);
            Assert.Equal(700, model.S1[1].Speed);
            Assert.Equal(600, model.S1[2].Speed);
            Assert.Equal(500, model.S1[3].Speed);
            Assert.Equal(400, model.S1[4].Speed);
            Assert.Equal(300, model.S1[5].Speed);

            // Insert midpoint.
            model.Process(new SetDriverSpeedMessage(9, SpeedCaptureLocation.S1, 550));
            Assert.Equal(6, model.S1.Count);
            Assert.Equal(800, model.S1[0].Speed);
            Assert.Equal(700, model.S1[1].Speed);
            Assert.Equal(600, model.S1[2].Speed);
            Assert.Equal(550, model.S1[3].Speed);
            Assert.Equal(500, model.S1[4].Speed);
            Assert.Equal(400, model.S1[5].Speed);

            // Replace bottom.
            model.Process(new SetDriverSpeedMessage(10, SpeedCaptureLocation.S1, 450));
            Assert.Equal(6, model.S1.Count);
            Assert.Equal(800, model.S1[0].Speed);
            Assert.Equal(700, model.S1[1].Speed);
            Assert.Equal(600, model.S1[2].Speed);
            Assert.Equal(550, model.S1[3].Speed);
            Assert.Equal(500, model.S1[4].Speed);
            Assert.Equal(450, model.S1[5].Speed);
        }

        [Fact]
        public void process_throws_when_message_is_null()
        {
            var model = CreateModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        private void assert_properties_have_default_values(SpeedCapturesModel model)
        {
            Assert.True(model.IsEmpty);
            Assert.Empty(model.S1);
            Assert.Empty(model.S2);
            Assert.Empty(model.S3);
            Assert.Empty(model.Straight);
        }

        private SpeedCapturesModel CreateModel(params Message[] messagesToProcess)
        {
            var model = new SpeedCapturesModel(new StubDriverModelLocator());
            foreach(var message in messagesToProcess)
            {
                model.Process(message);
            }
            return model;
        }

        private sealed class StubDriverModelLocator : IDriverModelLocator
        {
            public DriverModel GetDriver(int id)
            {
                return new DriverModel(id);
            }
        }

        private sealed class AllSpeedCaptureLocations : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return Enum.GetValues(typeof(SpeedCaptureLocation)).Cast<object>()
                    .Select(x => A(x)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(params object[] args)
            {
                return args;
            }
        }
    }
}