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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Model.Extensions;

namespace AK.F1.Timing.Model.Collections
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Messages.Driver.PostedTime"/> collection model.
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
        private TimeSpan? _currentDelta;
        private int _personBestCount;
        private int _sessionBestCount;
        private ObservableCollection<PostedTime> _innerValues;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PostedTimeCollectionModel"/> class.
        /// </summary>
        public PostedTimeCollectionModel() {

            _innerValues = new ObservableCollection<PostedTime>();
            Values = new ReadOnlyObservableCollection<PostedTime>(_innerValues);
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to this collection.
        /// </summary>
        /// <param name="item">The item to add to this collection.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        public void Add(PostedTime item) {

            Guard.NotNull(item, "item");

            _innerValues.Add(item);
            UpdateStatistics(item);
        }

        /// <summary>
        /// Replaces the current item with the specified <paramref name="replacement"/>.
        /// </summary>
        /// <param name="replacement">The replacement item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        public void ReplaceCurrent(PostedTime replacement) {

            Guard.NotNull(replacement, "replacement");

            _innerValues[_innerValues.Count - 1] = replacement;
            ReplaceStatistics(replacement);
        }

        /// <summary>
        /// Resets this collection model.
        /// </summary>
        public void Reset() {

            _innerValues.Clear();
            ResetStatistics();
        }

        /// <summary>
        /// Gets the underlying collection of values.
        /// </summary>
        public ReadOnlyObservableCollection<PostedTime> Values { get; private set; }

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
        /// Gets the delta between the current and previous value.
        /// </summary>
        public TimeSpan? CurrentDelta {

            get { return _currentDelta; }
            protected set { SetProperty("CurrentDelta", ref _currentDelta, value); }
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

        private void UpdateStatistics(PostedTime item) {

            UpdateCount();
            UpdateCurrentAndDelta();
            UpdateMinimum(item);
            UpdateMaximum(item);
            UpdateMean(item);
            UpdateRange();
            UpdateTypeCounts(item);
        }

        private void ReplaceStatistics(PostedTime replacement) {

            if(_innerValues.Count > 0) {
                UpdateCount();                
                ReplaceMinimum(replacement);
                ReplaceMaximum(replacement);
                ReplaceMean(replacement);
                UpdateRange();
                ReplaceTypeCounts(replacement);
                UpdateCurrentAndDelta();
            } else {
                ResetStatistics();
            }
        }

        private void ResetStatistics() {

            Count = 0;
            Current = null;
            Previous = null;
            CurrentDelta = null;
            Minimum = null;
            Maximum = null;
            Range = null;
            Mean = null;
            Total = 0d;
            PersonalBestCount = 0;
            SessionBestCount = 0;
        }

        private void UpdateCount() {

            Count = _innerValues.Count;
        }

        private void UpdateRange() {

            Range = Maximum.Time - Minimum.Time;
        }

        private void UpdateCurrentAndDelta() {

            Current = _innerValues[_innerValues.Count - 1];
            if(_innerValues.Count > 1) {
                Previous = _innerValues[_innerValues.Count - 2];
                CurrentDelta = Current.Time - Previous.Time;
            } else {
                Previous = null;
                CurrentDelta = null;
            }
        }
        
        private void UpdateTypeCounts(PostedTime item) {

            if(item.Type == PostedTimeType.PersonalBest) {
                ++PersonalBestCount;
            } else if(item.Type == PostedTimeType.SessionBest) {
                ++SessionBestCount;
            }
        }

        private void ReplaceTypeCounts(PostedTime replacement) {

            if(replacement.Type != Current.Type) {
                if(replacement.Type == PostedTimeType.PersonalBest) {
                    ++PersonalBestCount;
                    SessionBestCount = Math.Max(SessionBestCount - 1, 0);
                } else if(replacement.Type == PostedTimeType.SessionBest) {
                    ++SessionBestCount;
                    PersonalBestCount = Math.Max(PersonalBestCount - 1, 0);
                }
            }
        }

        private void UpdateMean(PostedTime item) {

            Total += ToDouble(item.Time);
            Mean = FromDouble(Total / _innerValues.Count);
        }

        private void ReplaceMean(PostedTime replacement) {

            if(replacement.Time != Current.Time) {
                Total -= ToDouble(Current.Time);
                Total += ToDouble(replacement.Time);
                Mean = FromDouble(Total / _innerValues.Count);
            }
        }

        private void UpdateMinimum(PostedTime item) {

            if(Minimum == null || item.CompareTo(Minimum) < 0) {
                Minimum = item;
            }
        }

        private void ReplaceMinimum(PostedTime replacement) {

            if(replacement.Equals(Minimum)) {
                Minimum = _innerValues.Min();
            }
        }

        private void UpdateMaximum(PostedTime item) {

            if(Maximum == null || item.CompareTo(Maximum) > 0) {
                Maximum = item;
            }
        }

        private void ReplaceMaximum(PostedTime replacement) {

            if(replacement.Equals(Maximum)) {
                Maximum = _innerValues.Max();
            }
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
