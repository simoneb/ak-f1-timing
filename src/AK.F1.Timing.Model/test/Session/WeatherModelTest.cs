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
using AK.F1.Timing.Messages.Weather;
using Xunit;

namespace AK.F1.Timing.Model.Session
{
    public class WeatherModelTest
    {
        [Fact]
        public void can_create()
        {
            var model = new WeatherModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void can_reset()
        {
            var model = new WeatherModel();

            model.AirTemperature.Add(1d);
            model.Humidity.Add(1d);
            model.Process(new SetIsWetMessage(true));
            model.Pressure.Add(1d);
            model.TrackTemperature.Add(1d);
            model.WindAngle.Add(1d);
            model.WindSpeed.Add(1d);

            model.Reset();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void process_throws_when_message_is_null()
        {
            var model = new WeatherModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Fact]
        public void processing_a_set_air_temperature_message_updates_the_air_temperature_collection()
        {
            var temperature = 21.6;
            var model = CreateModel(new SetAirTemperatureMessage(temperature));

            Assert.NotEmpty(model.AirTemperature.Items);
            Assert.Equal(temperature, model.AirTemperature.Items[0]);
        }

        [Fact]
        public void processing_a_set_track_temperature_message_updates_the_track_temperature_collection()
        {
            var temperature = 26.6;
            var model = CreateModel(new SetTrackTemperatureMessage(temperature));

            Assert.NotEmpty(model.TrackTemperature.Items);
            Assert.Equal(temperature, model.TrackTemperature.Items[0]);
        }

        [Fact]
        public void processing_a_set_humidity_message_updates_the_humidity_collection()
        {
            var humidity = 32.5;
            var model = CreateModel(new SetHumidityMessage(humidity));

            Assert.NotEmpty(model.Humidity.Items);
            Assert.Equal(humidity, model.Humidity.Items[0]);
        }

        [Fact]
        public void processing_a_set_atmospheric_pressure_message_updates_the_atmospheric_pressure_collection()
        {
            var pressure = 1018.8;
            var model = CreateModel(new SetAtmosphericPressureMessage(pressure));

            Assert.NotEmpty(model.Pressure.Items);
            Assert.Equal(pressure, model.Pressure.Items[0]);
        }

        [Fact]
        public void processing_a_set_wind_speed_message_updates_the_wind_speed_collection()
        {
            var speed = 4.75;
            var model = CreateModel(new SetWindSpeedMessage(speed));

            Assert.NotEmpty(model.WindSpeed.Items);
            Assert.Equal(speed, model.WindSpeed.Items[0]);
        }

        [Fact]
        public void processing_a_set_wind_angle_message_updates_the_wind_angle_collection()
        {
            var angle = 180;
            var model = CreateModel(new SetWindAngleMessage(angle));

            Assert.NotEmpty(model.WindAngle.Items);
            Assert.Equal(angle, model.WindAngle.Items[0]);
        }

        [Fact]
        public void processing_a_set_is_wet_message_updates_the_is_wet_property()
        {
            var isWet = true;
            var model = CreateModel(new SetIsWetMessage(isWet));

            Assert.Equal(isWet, model.IsWet);
        }

        [Fact]
        public void changes_to_the_is_wet_property_raise_the_property_changed_event()
        {
            var model = new WeatherModel();
            var observer = new PropertyChangeObserver<WeatherModel>(model);

            model.Process(new SetIsWetMessage(true));
            Assert.True(observer.HasChanged(x => x.IsWet));
        }

        private static WeatherModel CreateModel(Message messageToProcess)
        {
            var model = new WeatherModel();

            model.Process(messageToProcess);

            return model;
        }

        private void assert_properties_have_default_values(WeatherModel model)
        {
            Assert.Empty(model.AirTemperature.Items);
            Assert.Empty(model.Humidity.Items);
            Assert.False(model.IsWet);
            Assert.Empty(model.Pressure.Items);
            Assert.Empty(model.TrackTemperature.Items);
            Assert.Empty(model.WindAngle.Items);
            Assert.Empty(model.WindSpeed.Items);
        }
    }
}