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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using AK.F1.Timing.Extensions;
using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Session;
using AK.F1.Timing.Model.Collections;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;

namespace AK.F1.Timing.Model.Session
{
    public class SessionModel : ModelBase, IMessageProcessor, IDriverModelProvider
    {
        #region Private Fields.

        private SessionStatus _sessionStatus;
        private SessionType _sessionType;
        private TimeSpan _elapsedSessionTime;
        private TimeSpan _remainingSessionTime;        
        private int _raceLapNumber;
        private GridModelBase _grid;
        private SessionModelBuilder _builder;

        private static readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1d);

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SessionModel"/> class.
        /// </summary>
        public SessionModel() {

            this.Builder = new SessionModelBuilder(this);
            this.Drivers = new SortableObservableCollection<DriverModel>((x, y) => {
                return x.Position.CompareTo(y.Position);
            });
            this.DriversById = new List<DriverModel>(25);
            this.Feed = new FeedModel();
            this.Grid = GridModelBase.Create(SessionType.None);
            this.FastestTimes = new FastestTimesModel();
            this.Messages = new MessageModel();
            this.OneSecondTimer = new DispatcherTimer(DispatcherPriority.Normal);
            this.OneSecondTimer.Interval = ONE_SECOND;
            this.OneSecondTimer.Tick += OnOneSecondElapsed;
            this.SessionStatus = SessionStatus.Finished;
            this.Weather = new WeatherModel();
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public void Process(Message message) {

            this.Builder.Process(message);            
        }

        /// <summary>
        /// Resets this session.
        /// </summary>
        public void Reset() {

            this.OneSecondTimer.Stop();            
            this.DecrementRemainingSessionTime = false;
            this.Drivers.Clear();
            this.DriversById.Clear();
            this.ElapsedSessionTime = TimeSpan.Zero;
            this.FastestTimes.Reset();
            this.Feed.Reset();
            this.Grid = GridModelBase.Create(SessionType.None);            
            this.Messages.Reset();
            this.RaceLapNumber = 0;
            this.RemainingSessionTime = TimeSpan.Zero;
            this.SessionStatus = SessionStatus.Finished;
            this.SessionType = SessionType.None;            
            this.Weather.Reset();
        }

        /// <inheritdoc/>
        public DriverModel GetDriver(int id) {

            Guard.InRange(id > 0, "id");

            EnsureDriversByIdCount(id);

            int index = id - 1;
            DriverModel driver = this.DriversById[index];

            if(driver == null) {
                driver = new DriverModel(id);
                this.DriversById[index] = driver;
                this.Drivers.Add(driver);
            }

            return driver;
        }        

        /// <summary>
        /// Gets the collection of drivers participating in this session.
        /// </summary>
        public SortableObservableCollection<DriverModel> Drivers { get; private set; }

        /// <summary>
        /// Gets the weather model.
        /// </summary>
        public WeatherModel Weather { get; private set; }

        /// <summary>
        /// Gets the message model.
        /// </summary>
        public MessageModel Messages { get; private set; }

        /// <summary>
        /// Gets the feed model.
        /// </summary>
        public FeedModel Feed { get; private set; }

        /// <summary>
        /// Gets the current session type.
        /// </summary>
        public SessionType SessionType {

            get { return _sessionType; }
            protected internal set { SetProperty("SessionType", ref _sessionType, value); }
        }

        /// <summary>
        /// Gets the current sesion status.
        /// </summary>
        public SessionStatus SessionStatus {

            get { return _sessionStatus; }
            set { SetProperty("SessionStatus", ref _sessionStatus, value); }
        }

        /// <summary>
        /// Gets the race lap number.
        /// </summary>
        public int RaceLapNumber {

            get { return _raceLapNumber; }
            protected internal set { SetProperty("RaceLapNumber", ref _raceLapNumber, value); }
        }

        /// <summary>
        /// Gets the amount of session time elapsed.
        /// </summary>
        public TimeSpan ElapsedSessionTime {

            get { return _elapsedSessionTime; }
            protected internal set { SetProperty("ElapsedSessionTime", ref _elapsedSessionTime, value); }
        }

        /// <summary>
        /// Gets the amount of sesion time remaining.
        /// </summary>
        public TimeSpan RemainingSessionTime {

            get { return _remainingSessionTime; }
            protected internal set { SetProperty("RemainingSessionTime", ref _remainingSessionTime, value); }
        }

        /// <summary>
        /// Gets the fastest time information for this session.
        /// </summary>
        public FastestTimesModel FastestTimes { get; private set; }

        /// <summary>
        /// Gets the grid model for session.
        /// </summary>
        public GridModelBase Grid {

            get { return _grid; }
            set { SetProperty("Grid", ref _grid, value); }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Event handler invoked when a second has elapsed. This callback handler is only invoked
        /// when the session has started.
        /// </summary>
        protected virtual void OnOneSecondElapsed() {

            this.ElapsedSessionTime += ONE_SECOND;

            if(!this.DecrementRemainingSessionTime) {
                return;
            }

            TimeSpan remaining = this.RemainingSessionTime - ONE_SECOND;

            this.RemainingSessionTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets the instance which builds this session.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        protected SessionModelBuilder Builder {

            get { return _builder; }
            set {
                Guard.NotNull(value, "value");
                _builder = value;
            }
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Gets the one second dispatch timer.
        /// </summary>
        internal DispatcherTimer OneSecondTimer { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating if the
        /// <see cref="P:SessionModel.RemainingSessionTime"/> should be decremented when the
        /// <see cref="P:SessionModel.OneSecondTimer"/> ticks.
        /// </summary>
        internal bool DecrementRemainingSessionTime { get; set; }

        #endregion

        #region Private Impl.

        private void OnOneSecondElapsed(object sender, EventArgs e) {

            OnOneSecondElapsed();
        }

        private void EnsureDriversByIdCount(int count) {

            while(this.DriversById.Count < count) {
                this.DriversById.Add(null);
            }
        } 

        private List<DriverModel> DriversById { get; set; }

        #endregion
    }
}
