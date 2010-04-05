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

namespace AK.F1.Timing.Model.Grid
{
    /// <summary>
    /// Defines a null grid model. This model is used when no session is in progress. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    public sealed class NullGridModel : GridModelBase<NullGridRowModel>
    {
        #region Public Interface.

        /// <summary>
        /// Defines the only <see cref="NullGridModel"/> instance. This field is
        /// <see langword="readonly"/>.
        /// </summary>
        public static readonly NullGridModel Instance = new NullGridModel();

        /// <summary>
        /// This method is a no-operation.
        /// </summary>
        /// <param name="message">The message to process.</param>        
        public override void Process(Message message) { }

        /// <summary>
        /// This method always returns <see langword="null"/>.
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns><see langword="null"/>.</returns>
        public override NullGridRowModel GetRow(int driverId) {

            return null;
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// This method always throws a <see cref="System.NotImplementedException"/>
        /// </summary>        
        /// <param name="driverId"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override NullGridRowModel CreateRow(int driverId) {

            throw Guard.NotImplemented();
        }

        #endregion

        #region Private Impl.

        private NullGridModel() { }

        #endregion
    }
}
