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

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PostedTimeCollectionModel"/> class.
        /// </summary>
        public PostedTimeCollectionModel() {

            InnerItems = new ObservableCollection<PostedTime>();
            Items = new ReadOnlyObservableCollection<PostedTime>(InnerItems);
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

            InnerItems.Add(item);
            UpdateStatistics(item);
        }

        /// <summary>
        /// Replaces the current item with the specified <paramref name="replacement"/>.
        /// </summary>
        /// <param name="replacement">The replacement item.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="item"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when the collection is empty.
        /// </exception>
        public void ReplaceCurrent(PostedTime replacement) {

            Guard.NotNull(replacement, "replacement");
            if(InnerItems.Count == 0) {
                throw Guard.PostedTimeCollectionModel_CurrentCannotBeReplaced();
            }

            InnerItems[InnerItems.Count - 1] = replacement;
            ReplaceCurrentStatistics(replacement);
        }

        /// <summary>
        /// Resets this collection model.
        /// </summary>
        public void Reset() {

            InnerItems.Clear();
            ResetStatistics();
        }

        /// <summary>
        /// Gets the underlying collection of items.
        /// </summary>
        public ReadOnlyObservableCollection<PostedTime> Items { get; private set; }

        /// <summary>
        /// Gets the current item in this collection.
        /// </summary>
        public PostedTime Current {

            get { return _current; }
            protected set { SetProperty("Current", ref _current, value); }
        }

        /// <summary>
        /// Gets the previous item in this collection.
        /// </summary>
        public PostedTime Previous {

            get { return _previous; }
            protected set { SetProperty("Previous", ref _previous, value); }
        }

        /// <summary>
        /// Gets the delta between the current and previous item.
        /// </summary>
        public TimeSpan? CurrentDelta {

            get { return _currentDelta; }
            protected set { SetProperty("CurrentDelta", ref _currentDelta, value); }
        }

        /// <summary>
        /// Gets the smallest item in this collection.
        /// </summary>
        public PostedTime Minimum {

            get { return _minimum; }
            protected set { SetProperty("Minimum", ref _minimum, value); }
        }

        /// <summary>
        /// Gets the largest item in this collection.
        /// </summary>
        public PostedTime Maximum {

            get { return _maximum; }
            protected set { SetProperty("Maximum", ref _maximum, value); }
        }

        /// <summary>
        /// Gets the mean item value in this collection.
        /// </summary>
        public TimeSpan? Mean {

            get { return _mean; }
            protected set { SetProperty("Mean", ref _mean, value); }
        }

        /// <summary>
        /// Gets the range of items in this collection.
        /// </summary>
        public TimeSpan? Range {

            get { return _range; }
            protected set { SetProperty("Range", ref _range, value); }
        }

        /// <summary>
        /// Gets the number of items in the collection.
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
            
            UpdateCurrentAndDelta();
            UpdateMinimum(item);
            UpdateMaximum(item);
            UpdateMean(item);
            UpdateRange();
            UpdateCount();
            UpdateTypeCounts(item);
        }

        private void ReplaceCurrentStatistics(PostedTime replacement) {

            ReplaceCurrentMinimum(replacement);
            ReplaceCurrentMaximum(replacement);
            ReplaceCurrentMean(replacement);
            UpdateRange();
            ReplaceCurrentTypeCounts(replacement);
            UpdateCurrentAndDelta();
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
            SumOfValues = 0L;
            PersonalBestCount = 0;
            SessionBestCount = 0;
        }

        private void UpdateCount() {

            Count = InnerItems.Count;
        }

        private void UpdateRange() {

            Range = Maximum.Time - Minimum.Time;
        }

        private void UpdateCurrentAndDelta() {

            Current = InnerItems[InnerItems.Count - 1];
            if(InnerItems.Count > 1) {
                Previous = InnerItems[InnerItems.Count - 2];
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

        private void ReplaceCurrentTypeCounts(PostedTime replacement) {

            if(replacement.Type == Current.Type) {
                return;
            }
            switch(Current.Type) {
                case PostedTimeType.PersonalBest:
                    PersonalBestCount = Math.Max(PersonalBestCount - 1, 0);
                    break;
                case PostedTimeType.SessionBest:
                    SessionBestCount = Math.Max(SessionBestCount - 1, 0);
                    break;
            }
            switch(replacement.Type) {
                case PostedTimeType.PersonalBest:
                    ++PersonalBestCount;
                    break;
                case PostedTimeType.SessionBest:
                    ++SessionBestCount;
                    break;
            }
        }

        private void UpdateMean(PostedTime item) {

            SumOfValues += item.Time.Ticks;
            Mean = TimeSpan.FromTicks(SumOfValues / InnerItems.Count);
        }

        private void ReplaceCurrentMean(PostedTime replacement) {

            if(replacement.Time != Current.Time) {
                SumOfValues -= Current.Time.Ticks;
                SumOfValues += replacement.Time.Ticks;
                Mean = TimeSpan.FromTicks(SumOfValues / InnerItems.Count);
            }
        }

        private void UpdateMinimum(PostedTime item) {

            if(Minimum == null || item.CompareTo(Minimum) < 0) {
                Minimum = item;
            }
        }

        private void ReplaceCurrentMinimum(PostedTime replacement) {

            if(replacement.CompareTo(Minimum) < 0) {
                Minimum = replacement;
            }
        }

        private void UpdateMaximum(PostedTime item) {

            if(Maximum == null || item.CompareTo(Maximum) > 0) {
                Maximum = item;
            }
        }

        private void ReplaceCurrentMaximum(PostedTime replacement) {

            if(replacement.CompareTo(Maximum) > 0) {
                Maximum = replacement;
            }
        }

        private long SumOfValues { get; set; }

        private ObservableCollection<PostedTime> InnerItems { get; set; }

        #endregion
    }
}
