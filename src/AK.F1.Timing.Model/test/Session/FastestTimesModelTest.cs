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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Model.Driver;

namespace AK.F1.Timing.Model.Session
{
    public class FastestTimesModelTest
    {
        [Fact]
        public void can_create() {

            var model = CreateModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void ctor_throws_if_driver_model_provider_is_null() {

            Assert.Throws<ArgumentNullException>(() => new FastestTimesModel(null));
        }

        [Fact(Skip = "Needs to be implemented.")]
        public void can_reset() {
        }

        [Fact]
        public void process_throws_when_message_is_null() {

            var model = CreateModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Fact]
        public void processing_a_set_driver_sector_1_time_messages_updates_the_sector_1_property() {

            var driverId = 1;
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(35.4), PostedTimeType.SessionBest, 14);
            var model = CreateModel(new SetDriverSectorTimeMessage(driverId, 1, sectorTime));            

            Assert.NotNull(model.S1);
            Assert.Null(model.S1.Delta);
            Assert.NotNull(model.S1.Driver);
            Assert.Equal(driverId, model.S1.Driver.Id);
            Assert.Equal(sectorTime.Time, model.S1.Time);
            Assert.Equal(sectorTime.LapNumber, model.S1.LapNumber);
        }

        [Fact]
        public void processing_another_set_driver_sector_1_time_message_updates_the_sector_1_property_and_computes_the_delta() {

            var driverId = 2;
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(25.4), PostedTimeType.SessionBest, 24);
            var model = CreateModel(
                new SetDriverSectorTimeMessage(1, 1, new PostedTime(TimeSpan.FromSeconds(35.4), PostedTimeType.SessionBest, 14)),
                new SetDriverSectorTimeMessage(driverId, 1, sectorTime)
            );

            Assert.NotNull(model.S1);
            Assert.NotNull(model.S1.Delta);
            Assert.Equal(TimeSpan.FromSeconds(-10D), model.S1.Delta);
            Assert.NotNull(model.S1.Driver);
            Assert.Equal(driverId, model.S1.Driver.Id);
            Assert.Equal(sectorTime.Time, model.S1.Time);
            Assert.Equal(sectorTime.LapNumber, model.S1.LapNumber);
        }

        private void assert_properties_have_default_values(FastestTimesModel model) {

            Assert.True(model.IsEmpty);
            Assert.Null(model.Lap);
            Assert.Null(model.Possible);
            Assert.Null(model.S1);
            Assert.Null(model.S2);
            Assert.Null(model.S3);
        }

        private FastestTimesModel CreateModel(params Message[] messagesToProcess) {

            var model = new FastestTimesModel(new StubDriverModelLocator());

            foreach(var message in messagesToProcess) {
                model.Process(message);
            }

            return model;
        }

        private sealed class StubDriverModelLocator : IDriverModelLocator
        {
            public DriverModel GetDriver(int id) {

                return new DriverModel(id);
            }
        }
    }
}