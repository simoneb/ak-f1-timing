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
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Model.Extensions;

namespace AK.F1.Timing.Model.Collections
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Messaging.Messages.Driver.PostedTime"/> collection model.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class PostedTimeCollectionModel : ModelBase
    {
        #region Private Fields.

        private int _count;
        private PostedTime _current;
        private PostedTime _previous;
        private PostedTime _minimum;
        private PostedTime _maximum;
        private TimeSpan? _mean;
        private TimeSpan? _range;
        private int _personBestCount;
        private int _sessionBestCount;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PostedTimeCollectionModel"/> class.
        /// </summary>
        public PostedTimeCollectionModel() {

            this.Values = new ObservableCollection<PostedTime>();
            this.Values.CollectionChanged += OnValuesCollectionChanged;
        }

        /// <summary>
        /// Resets this collection model.
        /// </summary>
        public void Reset() {

            this.Values.Clear();
        }

        /// <summary>
        /// Gets the collection of values.
        /// </summary>
        public ObservableCollection<PostedTime> Values { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public PostedTime Current {

            get { return _current; }
            protected set { SetProperty("Current", ref _current, value); }
        }

        /// <summary>
        /// Gets the previous value.
        /// </summary>
        public PostedTime Previous {

            get { return _previous; }
            protected set { SetProperty("Previous", ref _previous, value); }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public PostedTime Minimum {

            get { return _minimum; }
            protected set { SetProperty("Minimum", ref _minimum, value); }
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public PostedTime Maximum {

            get { return _maximum; }
            protected set { SetProperty("Maximum", ref _maximum, value); }
        }

        /// <summary>
        /// Gets the mean value.
        /// </summary>
        public TimeSpan? Mean {

            get { return _mean; }
            protected set { SetProperty("Mean", ref _mean, value); }
        }

        /// <summary>
        /// Gets the value range.
        /// </summary>
        public TimeSpan? Range {

            get { return _range; }
            protected set { SetProperty("Range", ref _range, value); }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count {

            get { return _count; }
            protected set { SetProperty("Count", ref _count, value); }
        }

        /// <summary>
        /// Gets the number of personal best times in this collection.
        /// </summary>
        public int PersonalBestCount {

            get { return _personBestCount; }
            protected set { SetProperty("PersonalBestCount", ref _personBestCount, value); }
        }

        /// <summary>
        /// Gets the number of session best times in this collection.
        /// </summary>
        public int SessionBestCount {

            get { return _sessionBestCount; }
            protected set { SetProperty("SessionBestCount", ref _sessionBestCount, value); }
        }

        #endregion

        #region Private Impl.

        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            switch(e.Action) {
                case NotifyCollectionChangedAction.Add:
                    UpdateStatistics((PostedTime)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    ComputeStatistics();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ComputeStatistics();
                    break;
                case NotifyCollectionChangedAction.Move:
                default:
                    break;
            }
        }

        private void ComputeStatistics() {

            if(this.Values.Count > 0) {
                ComputeCount();
                ComputeCurrentAndPrevious();
                ComputeMinimum();
                ComputeMaximum();
                ComputeRange();
                ComputeMean();
                ComputeSessionAndPersonBestCounts();
            } else {
                ResetStatistics();
            }
        }

        private void ComputeCount() {

            this.Count = this.Values.Count;
        }

        private void ComputeRange() {

            this.Range = this.Maximum.Time - this.Minimum.Time;
        }

        private void ComputeMean() {

            this.Mean = this.Values.Average(t => t.Time);
        }

        private void ComputeCurrentAndPrevious() {

            int count = this.Values.Count;

            this.Current = this.Values[count - 1];
            if(count > 1) {
                this.Previous = this.Values[count - 2];
            }
        }

        private void ComputeMinimum() {

            this.Minimum = this.Values.Min();
        }

        private void ComputeMaximum() {

            this.Maximum = this.Values.Max();
        }

        private void ComputeSessionAndPersonBestCounts() {

            int personal = 0;
            int session = 0;

            foreach(var time in this.Values) {
                if(time.Type == PostedTimeType.PersonalBest) {
                    ++personal;
                } else if(time.Type == PostedTimeType.SessionBest) {
                    ++session;
                }
            }

            this.PersonalBestCount = personal;
            this.SessionBestCount = session;
        }

        private void UpdateStatistics(PostedTime value) {

            ComputeCount();
            ComputeCurrentAndPrevious();
            UpdateMinimum(value);
            UpdateMaximum(value);
            ComputeRange();
            UpdateMean(value);
            UpdateSessionAndPersonBestCounts(value);
        }

        private void UpdateMean(PostedTime value) {

            this.Total += ToDouble(value.Time);
            this.Mean = FromDouble(this.Total / this.Values.Count);
        }

        private void UpdateMinimum(PostedTime value) {

            if(this.Minimum == null || value.CompareTo(this.Minimum) < 0) {
                this.Minimum = value;
            }
        }

        private void UpdateMaximum(PostedTime value) {

            if(this.Maximum == null || value.CompareTo(this.Maximum) > 0) {
                this.Maximum = value;
            }
        }

        private void UpdateSessionAndPersonBestCounts(PostedTime value) {

            if(value.Type == PostedTimeType.PersonalBest) {
                ++this.PersonalBestCount;
            } else if(value.Type == PostedTimeType.SessionBest) {
                ++this.SessionBestCount;
            }
        }

        private void ResetStatistics() {

            this.Count = 0;
            this.Current = null;
            this.Previous = null;
            this.Minimum = null;
            this.Maximum = null;
            this.Range = null;
            this.Mean = null;
            this.Total = 0d;
            this.PersonalBestCount = 0;
            this.SessionBestCount = 0;
        }

        private static double ToDouble(TimeSpan value) {

            return value.TotalMilliseconds;
        }

        private static TimeSpan FromDouble(double value) {

            return TimeSpan.FromMilliseconds(value);
        }

        private double Total { get; set; }

        #endregion
    }
}
