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

            this.Values = new ObservableCollection<double>();
            this.Values.CollectionChanged += OnValuesCollectionChanged;
        }

        /// <summary>
        /// Adds the specified item to this collection.
        /// </summary>
        /// <param name="item">The item to add to this collection.</param>
        public void Add(double item) {

            this.Values.Add(item);
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
        public ObservableCollection<double> Values { get; private set; }

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

        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {            

            switch(e.Action) {
                case NotifyCollectionChangedAction.Add:
                    UpdateStatistics((double)e.NewItems[0]);
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
                ComputeCurrent();
                ComputeMinimum();
                ComputeMaximum();
                ComputeRange();
                ComputeMean();
                ComputeStandardDeviation();
            } else {
                ResetStatistics();
            }
        }

        private void ComputeCount() {

            this.Count = this.Values.Count;
        }

        private void ComputeMean() {

            this.Mean = this.Values.Average();
        }

        private void ComputeRange() {

            this.Range = this.Maximum - this.Minimum;
        }

        private void ComputeCurrent() {

            this.Current = this.Values[this.Values.Count - 1];
        }

        private void ComputeMinimum() {

            this.Minimum = this.Values.Min();
        }

        private void ComputeMaximum() {

            this.Maximum = this.Values.Max();
        }

        private void ComputeStandardDeviation() {

            UpdateStandardDeviation(this.Current.Value);
        }

        private void UpdateStatistics(double value) {

            ComputeCount();
            ComputeCurrent();
            UpdateMinimum(value);
            UpdateMaximum(value);
            ComputeRange();
            UpdateMean(value);
            UpdateStandardDeviation(value);
        }

        private void UpdateStandardDeviation(double value) {

            double n = this.Values.Count;
            double mean = this.Mean != null ? this.Mean.Value : 0d;

            this.SumOfSquaredValues += Sqr(value);
            this.StandardDeviation = Math.Sqrt(1d / n * (this.SumOfSquaredValues - (n * Sqr(mean))));
        }

        private void UpdateMean(double value) {

            this.Total += value;
            this.Mean = this.Total / this.Values.Count;
        }

        private void UpdateMinimum(double value) {

            if(this.Minimum == null || value < this.Minimum) {
                this.Minimum = value;
            }
        }

        private void UpdateMaximum(double value) {

            if(this.Maximum == null || value > this.Maximum) {
                this.Maximum = value;
            }
        }

        private void ResetStatistics() {

            this.Count = 0;
            this.Current = null;
            this.Minimum = null;
            this.Maximum = null;
            this.Range = null;
            this.Mean = null;
            this.StandardDeviation = null;
            this.Total = 0d;            
            this.SumOfSquaredValues = 0d;            
        }

        private static double Sqr(double d) {

            return d * d;
        }

        private double Total { get; set; }

        private double SumOfSquaredValues { get; set; }

        #endregion
    }
}
