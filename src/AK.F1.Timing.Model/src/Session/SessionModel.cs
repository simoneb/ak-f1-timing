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
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Model.Collections;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Provides detailed information about a single F1 timing session.
    /// </summary>
    public class SessionModel : ModelBase, IMessageProcessor
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

            Builder = new SessionModelBuilder(this);            
            InnerDrivers = new SortableObservableCollection<DriverModel>((x, y) => {
                return x.Position.CompareTo(y.Position);
            });
            Drivers = new ReadOnlyObservableCollection<DriverModel>(InnerDrivers);
            DriversById = new Dictionary<int, DriverModel>(25);
            Feed = new FeedModel();
            Grid = GridModelBase.Create(SessionType.None);
            FastestTimes = new FastestTimesModel();
            Messages = new MessageModel();
            OneSecondTimer = new DispatcherTimer(DispatcherPriority.Normal);
            OneSecondTimer.Interval = ONE_SECOND;
            OneSecondTimer.Tick += (s, e) => OnOneSecondElapsed();
            SessionStatus = SessionStatus.Finished;
            Weather = new WeatherModel();
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public void Process(Message message) {

            Builder.Process(message);            
        }

        /// <summary>
        /// Resets this session.
        /// </summary>
        public void Reset() {

            OneSecondTimer.Stop();            
            DecrementRemainingSessionTime = false;
            InnerDrivers.Clear();
            DriversById.Clear();
            ElapsedSessionTime = TimeSpan.Zero;
            FastestTimes.Reset();
            Feed.Reset();
            Grid = GridModelBase.Create(SessionType.None);            
            Messages.Reset();
            RaceLapNumber = 0;
            RemainingSessionTime = TimeSpan.Zero;
            SessionStatus = SessionStatus.Finished;
            SessionType = SessionType.None;            
            Weather.Reset();
        }

        /// <inheritdoc/>
        public DriverModel GetDriver(int id) {

            Guard.InRange(id > 0, "id");

            DriverModel driver;

            if(!DriversById.TryGetValue(id, out driver)) {
                driver = new DriverModel(id);
                DriversById.Add(id, driver);
                InnerDrivers.Add(driver);
            }

            return driver;
        }

        /// <summary>
        /// Gets the collection of drivers participating in this session.
        /// </summary>
        public ReadOnlyObservableCollection<DriverModel> Drivers { get; private set; }

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

            ElapsedSessionTime += ONE_SECOND;

            if(!DecrementRemainingSessionTime) {
                return;
            }

            TimeSpan remaining = RemainingSessionTime - ONE_SECOND;

            RemainingSessionTime = remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
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

        /// <summary>
        /// Gets the inner collection of drivers.
        /// </summary>
        internal SortableObservableCollection<DriverModel> InnerDrivers { get; set; }

        #endregion

        #region Private Impl.

        private IDictionary<int, DriverModel> DriversById { get; set; }

        #endregion
    }
}
