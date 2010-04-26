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

namespace AK.F1.Timing.Model.Session
{
    public partial class WeatherModel
    {
        /// <summary>
        /// An <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
        /// <see cref="AK.F1.Timing.Model.Session.WeatherModel"/> as it processes
        /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
        /// </summary>
        [Serializable]
        private sealed class WeatherModelBuilder : MessageVisitorBase, IMessageProcessor
        {
            #region Public Interface.

            /// <summary>
            /// Initialises a new instance of the <see cref="WeatherModelBuilder"/> class and specifies
            /// the <paramref name="model"/> to build.
            /// </summary>
            /// <param name="model">The model to build.</param>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="model"/> is <see langword="null"/>.
            /// </exception>
            public WeatherModelBuilder(WeatherModel model) {

                Guard.NotNull(model, "model");

                Model = model;
            }

            /// <inheritdoc/>
            public void Process(Message message) {

                message.Accept(this);
            }

            /// <inheritdoc/>
            public override void Visit(SetAirTemperatureMessage message) {

                Model.AirTemperature.Add(message.Temperature);
            }

            /// <inheritdoc/>
            public override void Visit(SetTrackTemperatureMessage message) {

                Model.TrackTemperature.Add(message.Temperature);
            }

            /// <inheritdoc/>
            public override void Visit(SetAtmosphericPressureMessage message) {

                Model.Pressure.Add(message.Pressure);
            }

            /// <inheritdoc/>
            public override void Visit(SetHumidityMessage message) {

                Model.Humidity.Add(message.Humidity);
            }

            /// <inheritdoc/>
            public override void Visit(SetWindSpeedMessage message) {

                Model.WindSpeed.Add(message.Speed);
            }

            /// <inheritdoc/>
            public override void Visit(SetWindAngleMessage message) {

                Model.WindAngle.Add(message.Angle);
            }

            /// <inheritdoc/>
            public override void Visit(SetIsWetMessage message) {

                Model.IsWet = message.IsWet;
            }

            #endregion

            #region Private Impl.

            private WeatherModel Model { get; set; }

            #endregion
        }
    }
}