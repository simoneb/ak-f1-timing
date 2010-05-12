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
using AK.F1.Timing.Extensions;

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
        private DeltaType _currentDeltaType;
        private double? _minimum;
        private double? _maximum;
        private double? _mean;
        private DeltaType _meanDeltaType;
        private double? _range;
        private double? _standardDeviation;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Occurs when this collection is resetting.
        /// </summary>
        public event EventHandler ResetBegin;

        /// <summary>
        /// Occurs when this collection has been reset.
        /// </summary>
        public event EventHandler ResetComplete;

        /// <summary>
        /// Initialises a new instance of the <see cref="DoubleCollectionModel"/> class.
        /// </summary>
        public DoubleCollectionModel()
        {
            InnerItems = new ObservableCollection<double>();
            Items = new ReadOnlyObservableCollection<double>(InnerItems);
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to this collection.
        /// </summary>
        /// <param name="item">The item to add to this collection.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// When when <paramref name="item"/> is not a number.
        /// </exception>
        public void Add(double item)
        {
            Guard.InRange(!Double.IsNaN(item), "item");

            InnerItems.Add(item);
            UpdateStatistics(item);
        }

        /// <summary>
        /// Resets this collection model.
        /// </summary>
        public void Reset()
        {
            ResetBegin.Raise(this);
            InnerItems.Clear();
            ResetStatistics();
            ResetComplete.Raise(this);
        }

        /// <summary>
        /// Gets the underlying collection of items.
        /// </summary>
        public ReadOnlyObservableCollection<double> Items { get; private set; }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        public double? Current
        {
            get { return _current; }
            protected set { SetProperty("Current", ref _current, value); }
        }

        /// <summary>
        /// Gets delta type of the current item.
        /// </summary>
        public DeltaType CurrentDeltaType
        {
            get { return _currentDeltaType; }
            protected set { SetProperty("CurrentDeltaType", ref _currentDeltaType, value); }
        }

        /// <summary>
        /// Gets the smallest item in this collection.
        /// </summary>
        public double? Minimum
        {
            get { return _minimum; }
            protected set { SetProperty("Minimum", ref _minimum, value); }
        }

        /// <summary>
        /// Gets the largest item in this collection.
        /// </summary>
        public double? Maximum
        {
            get { return _maximum; }
            protected set { SetProperty("Maximum", ref _maximum, value); }
        }

        /// <summary>
        /// Gets the mean item value in this collection.
        /// </summary>
        public double? Mean
        {
            get { return _mean; }
            protected set { SetProperty("Mean", ref _mean, value); }
        }

        /// <summary>
        /// Gets delta type of the mean item value.
        /// </summary>
        public DeltaType MeanDeltaType
        {
            get { return _meanDeltaType; }
            protected set { SetProperty("MeanDeltaType", ref _meanDeltaType, value); }
        }

        /// <summary>
        /// Gets the range of items in this collection.
        /// </summary>
        public double? Range
        {
            get { return _range; }
            protected set { SetProperty("Range", ref _range, value); }
        }

        /// <summary>
        /// Gets the standard deviation across all items in this collection.
        /// </summary>
        public double? StandardDeviation
        {
            get { return _standardDeviation; }
            protected set { SetProperty("StandardDeviation", ref _standardDeviation, value); }
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return _count; }
            protected set { SetProperty("Count", ref _count, value); }
        }

        #endregion

        #region Private Impl.

        private void UpdateStatistics(double item)
        {
            UpdateCurrent();
            UpdateDeltaType(item);
            UpdateMinimum(item);
            UpdateMaximum(item);
            UpdateRange();
            UpdateMean(item);
            UpdateStandardDeviation(item);
            UpdateCount();
        }

        private void UpdateCount()
        {
            Count = InnerItems.Count;
        }

        private void UpdateCurrent()
        {
            Current = InnerItems[InnerItems.Count - 1];
        }

        private void UpdateDeltaType(double item)
        {
            CurrentDeltaType = Items.Count > 1
                ? ComputeDeltaType(InnerItems[InnerItems.Count - 2], item)
                : DeltaType.None;
        }

        private void UpdateStandardDeviation(double item)
        {
            double n = InnerItems.Count;
            double mean = Mean.GetValueOrDefault(0d);

            SumOfSquaredValues += Sqr(item);
            StandardDeviation = Math.Sqrt(1d / n * (SumOfSquaredValues - (n * Sqr(mean))));
        }

        private void UpdateMean(double item)
        {
            double? previousMean = Mean;

            SumOfValues += item;
            Mean = SumOfValues / InnerItems.Count;
            MeanDeltaType = previousMean.HasValue
                ? ComputeDeltaType(previousMean.Value, Mean.Value)
                : DeltaType.None;
        }

        private void UpdateMinimum(double item)
        {
            if(Minimum == null || item < Minimum)
            {
                Minimum = item;
            }
        }

        private void UpdateMaximum(double item)
        {
            if(Maximum == null || item > Maximum)
            {
                Maximum = item;
            }
        }

        private void UpdateRange()
        {
            Range = Maximum - Minimum;
        }

        private void ResetStatistics()
        {
            Count = 0;
            Current = null;
            CurrentDeltaType = DeltaType.None;
            Minimum = null;
            Maximum = null;
            Range = null;
            Mean = null;
            MeanDeltaType = DeltaType.None;
            StandardDeviation = null;
            SumOfValues = 0d;
            SumOfSquaredValues = 0d;
        }

        private static double Sqr(double d)
        {
            return d * d;
        }

        private static DeltaType ComputeDeltaType(double previousItem, double newItem)
        {
            if(newItem > previousItem)
            {
                return DeltaType.Increase;
            }
            else if(newItem < previousItem)
            {
                return DeltaType.Decrease;
            }
            else
            {
                return DeltaType.None;
            }
        }

        private double SumOfValues { get; set; }

        private double SumOfSquaredValues { get; set; }

        private ObservableCollection<double> InnerItems { get; set; }

        #endregion
    }
}