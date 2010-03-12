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

using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Model.Grid
{
    /// <summary>
    /// Defines the base class for a grid model. This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class GridModelBase : ModelBase, IMessageProcessor
    {
        #region Public Interface.

        /// <summary>
        /// When override in a derived class; processes the specified message.
        /// </summary>
        /// <param name="message">The message to process.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public abstract void Process(Message message);

        /// <summary>
        /// Creates a grid model for the specified session type.
        /// </summary>
        /// <param name="type">The type of grid model to create.</param>
        /// <returns>The grid model for the specified session type.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="type"/> is not a valid session type.
        /// </exception>
        public static GridModelBase Create(SessionType type) {

            switch(type) {
                case SessionType.None:
                    return NullGridModel.Instance;
                case SessionType.Practice:
                    return new PracticeGridModel();
                case SessionType.Qually:
                    return new QuallyGridModel();
                case SessionType.Race:
                    return new RaceGridModel();
                default:
                    throw Guard.ArgumentOutOfRange("type");
            }
        }

        #endregion
    }
}
