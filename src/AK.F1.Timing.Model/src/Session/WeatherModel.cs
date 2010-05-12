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

using AK.F1.Timing.Model.Collections;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Contains all information relating to the current weather condition.
    /// </summary>
    public partial class WeatherModel : ModelBase, IMessageProcessor
    {
        #region Private Fields.

        private bool _isWet;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="WeatherModel"/> class.
        /// </summary>
        public WeatherModel()
        {
            AirTemperature = new DoubleCollectionModel();
            TrackTemperature = new DoubleCollectionModel();
            Humidity = new DoubleCollectionModel();
            WindSpeed = new DoubleCollectionModel();
            Pressure = new DoubleCollectionModel();
            WindAngle = new DoubleCollectionModel();
            Builder = new WeatherModelBuilder(this);
        }

        /// <inheritdoc/>        
        public void Process(Message message)
        {
            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <summary>
        /// Resets this weather model.
        /// </summary>
        public void Reset()
        {
            AirTemperature.Reset();
            Pressure.Reset();
            Humidity.Reset();
            TrackTemperature.Reset();
            WindAngle.Reset();
            WindSpeed.Reset();
            IsWet = false;
        }

        /// <summary>
        /// Gets the track temperature model.
        /// </summary>
        public DoubleCollectionModel TrackTemperature { get; private set; }

        /// <summary>
        /// Gets the air temperature model.
        /// </summary>
        public DoubleCollectionModel AirTemperature { get; private set; }

        /// <summary>
        /// Gets the humidity model.
        /// </summary>
        public DoubleCollectionModel Humidity { get; private set; }

        /// <summary>
        /// Gets the atmospheric pressure model.
        /// </summary>
        public DoubleCollectionModel Pressure { get; private set; }

        /// <summary>
        /// Gets the wind speed model.
        /// </summary>
        public DoubleCollectionModel WindSpeed { get; private set; }

        /// <summary>
        /// Gets the wind angle model.
        /// </summary>
        public DoubleCollectionModel WindAngle { get; private set; }

        /// <summary>
        /// Gets a value indicating if the track is wet.
        /// </summary>      
        public bool IsWet
        {
            get { return _isWet; }
            private set { SetProperty("IsWet", ref _isWet, value); }
        }

        #endregion

        #region Private Impl.

        private IMessageProcessor Builder { get; set; }

        #endregion
    }
}