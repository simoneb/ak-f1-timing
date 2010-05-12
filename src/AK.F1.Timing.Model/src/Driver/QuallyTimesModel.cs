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

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class QuallyTimesModel : ModelBase
    {
        #region Private Fields.

        private TimeSpan? _q1;
        private TimeSpan? _q2;
        private TimeSpan? _q3;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Sets the qually time for the specified one-based qually number.
        /// </summary>
        /// <param name="quallyNumber">The one-based qually number.</param>
        /// <param name="value">The new qually value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="quallyNumber"/> is not positive or is greater than three.
        /// </exception>
        public void SetTime(int quallyNumber, TimeSpan? value)
        {
            if(quallyNumber == 1)
            {
                Q1 = value;
            }
            else if(quallyNumber == 2)
            {
                Q2 = value;
            }
            else if(quallyNumber == 3)
            {
                Q3 = value;
            }
            else
            {
                throw Guard.ArgumentOutOfRange("quallyNumber");
            }
        }

        /// <summary>
        /// Gets the qually time for the specified one-based qually number.
        /// </summary>
        /// <param name="quallyNumber">The one-based qually number.</param>
        /// <returns>The qually time for the specified one-based qually number.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="quallyNumber"/> is not positive or is greater than three.
        /// </exception>
        public TimeSpan? GetTime(int quallyNumber)
        {
            if(quallyNumber == 1)
            {
                return Q1;
            }
            else if(quallyNumber == 2)
            {
                return Q2;
            }
            else if(quallyNumber == 3)
            {
                return Q3;
            }
            else
            {
                throw Guard.ArgumentOutOfRange("quallyNumber");
            }
        }

        /// <summary>
        /// Gets or sets the qually one time.
        /// </summary>
        public TimeSpan? Q1
        {
            get { return _q1; }
            set { SetProperty("Q1", ref _q1, value); }
        }

        /// <summary>
        /// Gets or sets the qually two time.
        /// </summary>
        public TimeSpan? Q2
        {
            get { return _q2; }
            set { SetProperty("Q2", ref _q2, value); }
        }

        /// <summary>
        /// Gets or sets the qually three time.
        /// </summary>
        public TimeSpan? Q3
        {
            get { return _q3; }
            set { SetProperty("Q3", ref _q3, value); }
        }

        #endregion
    }
}