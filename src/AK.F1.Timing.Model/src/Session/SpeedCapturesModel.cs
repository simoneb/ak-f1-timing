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
using System.Collections.ObjectModel;
using System.Linq;
using AK.F1.Timing.Messages;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Contains information relating to the speeds captured during a session. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed partial class SpeedCapturesModel : ModelBase
    {
        #region Private Fields.

        /// <summary>
        /// The feed only sends through the top six speed captures.
        /// </summary>
        internal const int MaxCollectionSize = 6;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SpeedCapturesModel"/> class and specifies
        /// the driver model provider.
        /// </summary>
        /// <param name="driverLocator">The driver model provider.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="driverLocator"/> is <see langword="null"/>.
        /// </exception>
        public SpeedCapturesModel(IDriverModelLocator driverLocator)
        {
            Guard.NotNull(driverLocator, "driverLocator");

            DriverLocator = driverLocator;
            InnerS1 = new ObservableCollection<SpeedCaptureModel>();
            S1 = new ReadOnlyObservableCollection<SpeedCaptureModel>(InnerS1);
            InnerS2 = new ObservableCollection<SpeedCaptureModel>();
            S2 = new ReadOnlyObservableCollection<SpeedCaptureModel>(InnerS2);
            InnerS3 = new ObservableCollection<SpeedCaptureModel>();
            S3 = new ReadOnlyObservableCollection<SpeedCaptureModel>(InnerS3);
            InnerStraight = new ObservableCollection<SpeedCaptureModel>();
            Straight = new ReadOnlyObservableCollection<SpeedCaptureModel>(InnerStraight);
            Builder = new SpeedCapturesModelBuilder(this);
        }

        /// <inheritdoc/>        
        public void Process(Message message)
        {
            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <summary>
        /// Resets this model.
        /// </summary>
        public void Reset()
        {
            InnerS1.Clear();
            InnerS2.Clear();
            InnerS3.Clear();
            InnerStraight.Clear();
            NotifyIsEmptyChanged();
        }

        /// <summary>
        /// Gets the capture collection for the specified <paramref name="location"/>.
        /// </summary>
        /// <param name="location">the capture location.</param>
        /// <returns>The capture collection for the specified <paramref name="location"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="location"/> is not valid.
        /// </exception>
        public ReadOnlyObservableCollection<SpeedCaptureModel> GetCollection(SpeedCaptureLocation location)
        {
            switch(location)
            {
                case SpeedCaptureLocation.S1:
                    return S1;
                case SpeedCaptureLocation.S2:
                    return S2;
                case SpeedCaptureLocation.S3:
                    return S3;
                case SpeedCaptureLocation.Straight:
                    return Straight;
                default:
                    throw Guard.ArgumentOutOfRange("location");
            }
        }

        /// <summary>
        /// Gets the S1 speed captures.
        /// </summary>
        public ReadOnlyObservableCollection<SpeedCaptureModel> S1 { get; private set; }

        /// <summary>
        /// Gets the S2 speed captures.
        /// </summary>
        public ReadOnlyObservableCollection<SpeedCaptureModel> S2 { get; private set; }

        /// <summary>
        /// Gets the S3 speed captures.
        /// </summary>
        public ReadOnlyObservableCollection<SpeedCaptureModel> S3 { get; private set; }

        /// <summary>
        /// Gets the straight speed captures.
        /// </summary>
        public ReadOnlyObservableCollection<SpeedCaptureModel> Straight { get; private set; }

        /// <summary>
        /// Gets a value indicating if this model is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return S1.Count == 0 && S2.Count == 0 && S3.Count == 0 && Straight.Count == 0; }
        }

        #endregion

        #region Private Impl.

        private void UpdateCollection(SpeedCaptureLocation location, int driverId, int speed)
        {
            var collection = GetInnerCollection(location);
            var existing = collection.Where(x => x.Driver.Id == driverId).FirstOrDefault();
            if(existing != null)
            {
                if(speed <= existing.Speed)
                {
                    return;
                }
                collection.Remove(existing);
            }
            if(collection.Count < MaxCollectionSize)
            {
                int insertIndex = collection.Count;
                for(int i = 0; i < collection.Count; ++i)
                {
                    if(speed > collection[i].Speed)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                collection.Insert(insertIndex, CreateSpeedCaptureModel(location, driverId, speed));
            }
            else if(speed > collection.Last().Speed)
            {
                for(int i = 0; i < collection.Count; ++i)
                {
                    if(speed > collection[i].Speed)
                    {
                        var previous = collection[i];
                        for(int j = i + 1; j < collection.Count; ++j)
                        {
                            var temp = collection[j];
                            collection[j] = previous;
                            previous = temp;
                        }
                        collection[i] = CreateSpeedCaptureModel(location, driverId, speed);
                        break;
                    }
                }
            }
            NotifyIsEmptyChanged();
        }

        private SpeedCaptureModel CreateSpeedCaptureModel(SpeedCaptureLocation location, int driverId, int speed)
        {
            return new SpeedCaptureModel(DriverLocator.GetDriver(driverId), location, speed);
        }

        private ObservableCollection<SpeedCaptureModel> GetInnerCollection(SpeedCaptureLocation location)
        {
            switch(location)
            {
                case SpeedCaptureLocation.S1:
                    return InnerS1;
                case SpeedCaptureLocation.S2:
                    return InnerS2;
                case SpeedCaptureLocation.S3:
                    return InnerS3;
                case SpeedCaptureLocation.Straight:
                    return InnerStraight;
                default:
                    throw Guard.ArgumentOutOfRange("location");
            }
        }

        private void NotifyIsEmptyChanged()
        {
            OnPropertyChanged("IsEmpty");
        }

        private IMessageProcessor Builder { get; set; }

        private IDriverModelLocator DriverLocator { get; set; }

        private ObservableCollection<SpeedCaptureModel> InnerS1 { get; set; }

        private ObservableCollection<SpeedCaptureModel> InnerS2 { get; set; }

        private ObservableCollection<SpeedCaptureModel> InnerS3 { get; set; }

        private ObservableCollection<SpeedCaptureModel> InnerStraight { get; set; }

        #endregion
    }
}