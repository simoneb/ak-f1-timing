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
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AK.F1.Timing.Model.Collections
{
    /// <summary>
    /// A <see cref="System.Double"/> collection model.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class DoubleCollectionModel : ModelBase
    {
        #region Private Fields.

        private int _count;
        private double? _current;
        private double? _minimum;
        private double? _maximum;
        private double? _mean;
        private double? _range;
        private double? _standardDeviation;        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DoubleCollectionModel"/> class.
        /// </summary>
        public DoubleCollectionModel() {

            InnerValues = new ObservableCollection<double>();            
            Values = new ReadOnlyObservableCollection<double>(InnerValues);
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to this collection.
        /// </summary>
        /// <param name="item">The item to add to this collection.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When when <paramref name="item"/> is not a number.
        /// </exception>
        public void Add(double item) {

            Guard.InRange(!Double.IsNaN(item), "item");

            InnerValues.Add(item);
            UpdateStatistics(item);
        }

        /// <summary>
        /// Resets this collection model.
        /// </summary>
        public void Reset() {

            InnerValues.Clear();
            ResetStatistics();
        }

        /// <summary>
        /// Gets the underlying collection of values.
        /// </summary>
        public ReadOnlyObservableCollection<double> Values { get; private set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public double? Current {

            get { return _current; }
            protected set { SetProperty("Current", ref _current, value); }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        public double? Minimum {

            get { return _minimum; }
            protected set { SetProperty("Minimum", ref _minimum, value); }
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        public double? Maximum {

            get { return _maximum; }
            protected set { SetProperty("Maximum", ref _maximum, value); }
        }

        /// <summary>
        /// Gets the mean value.
        /// </summary>
        public double? Mean {

            get { return _mean; }
            protected set { SetProperty("Mean", ref _mean, value); }
        }

        /// <summary>
        /// Gets the value range.
        /// </summary>
        public double? Range {

            get { return _range; }
            protected set { SetProperty("Range", ref _range, value); }
        }

        /// <summary>
        /// Gets the standard deviation.
        /// </summary>
        public double? StandardDeviation {

            get { return _standardDeviation; }
            protected set { SetProperty("StandardDeviation", ref _standardDeviation, value); }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count {

            get { return _count; }
            protected set { SetProperty("Count", ref _count, value); }
        }

        #endregion

        #region Private Impl.

        private void UpdateStatistics(double item) {

            UpdateCount();
            UpdateCurrent();
            UpdateMinimum(item);
            UpdateMaximum(item);
            UpdateRange();
            UpdateMean(item);
            UpdateStandardDeviation(item);
        }

        private void UpdateCount() {

            Count = InnerValues.Count;
        }

        private void UpdateCurrent() {

            Current = InnerValues[InnerValues.Count - 1];
        }

        private void UpdateStandardDeviation(double item) {

            double n = InnerValues.Count;
            double mean = Mean.GetValueOrDefault(0d);

            SumOfSquaredValues += Sqr(item);
            StandardDeviation = Math.Sqrt(1d / n * (SumOfSquaredValues - (n * Sqr(mean))));
        }

        private void UpdateMean(double item) {

            SumOfValues += item;
            Mean = SumOfValues / InnerValues.Count;
        }

        private void UpdateMinimum(double item) {

            if(Minimum == null || item < Minimum) {
                Minimum = item;
            }
        }

        private void UpdateMaximum(double item) {

            if(Maximum == null || item > Maximum) {
                Maximum = item;
            }
        }

        private void UpdateRange() {

            Range = Maximum - Minimum;
        }

        private void ResetStatistics() {

            Count = 0;
            Current = null;
            Minimum = null;
            Maximum = null;
            Range = null;
            Mean = null;
            StandardDeviation = null;
            SumOfValues = 0d;            
            SumOfSquaredValues = 0d;            
        }

        private static double Sqr(double d) {

            return d * d;
        }

        private double SumOfValues { get; set; }

        private double SumOfSquaredValues { get; set; }

        private ObservableCollection<double> InnerValues { get; set; }

        #endregion
    }
}
