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

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// Driver model.
    /// </summary>
    [Serializable]
    public partial class DriverModel : ModelBase, IMessageProcessor
    {
        #region Private Fields.

        private string _name;
        private int _position;
        private int _carNumber;        
        private int _lapsCompleted;        
        private DriverStatus _status;
        private Gap _gap;
        private Gap _interval;        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DriverModel"/> class and specifies the
        /// driver's Id.
        /// </summary>
        /// <param name="id">The driver's Id.</param>
        public DriverModel(int id) {

            Id = id;
            LapTimes = new LapTimesModel();
            PitTimes = new PitTimesModel();            
            QuallyTimes = new QuallyTimesModel();
            Status = DriverStatus.InPits;
            Builder = new DriverModelBuilder(this);
        }

        /// <inheritdoc/>        
        public void Process(Message message) {

            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Name ?? base.ToString();
        }

        /// <summary>
        /// Gets the Id of this driver.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets or sets the gap between this driver and the leader.
        /// </summary>
        public Gap Gap {

            get { return _gap; }
            set { SetProperty("Gap", ref _gap, value); }
        }

        /// <summary>
        /// Gets or sets the interval between this driver and the driver in front.
        /// </summary>
        public Gap Interval {

            get { return _interval; }
            set { SetProperty("Interval", ref _interval, value); }
        }

        /// <summary>
        /// Gets or sets the number of laps completed by this driver.
        /// </summary>
        public int LapsCompleted {

            get { return _lapsCompleted; }
            set { SetProperty("LapsCompleted", ref _lapsCompleted, value); }
        }

        /// <summary>
        /// Gets or sets the name of this driver.
        /// </summary>
        public string Name {

            get { return _name; }
            set { SetProperty("Name", ref _name, value); }
        }

        /// <summary>
        /// Gets or sets the driver's car number.
        /// </summary>
        public int CarNumber {

            get { return _carNumber; }
            set { SetProperty("CarNumber", ref _carNumber, value); }
        }

        /// <summary>
        /// Gets or sets the position of this driver.
        /// </summary>
        public int Position {

            get { return _position; }
            set { SetProperty("Position", ref _position, value); }
        }

        /// <summary>
        /// Gets the lap times model.
        /// </summary>
        public LapTimesModel LapTimes { get; private set; }

        /// <summary>
        /// Gets the qually time model.
        /// </summary>
        public QuallyTimesModel QuallyTimes { get; private set; }

        /// <summary>
        /// Gets the pit times model.
        /// </summary>
        public PitTimesModel PitTimes { get; private set; }

        /// <summary>
        /// Gets or sets the status of this driver.
        /// </summary>
        public DriverStatus Status {

            get { return _status; }
            set { SetProperty("Status", ref _status, value); }
        }

        #endregion

        #region Private Impl.

        private IMessageProcessor Builder { get; set; }

        #endregion
    }
}
