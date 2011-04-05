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
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messages.Session
{
    /// <summary>
    /// A message which contains the unprocessed speeds captured at a specific location
    /// on the track. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(63694251)]
    public sealed class RawSpeedCaptureMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RawSpeedCaptureMessage"/> class
        /// and specifies the apex location <paramref name="location"/> and the raw apex
        /// <paramref name="speeds"/>.
        /// </summary>
        /// <param name="location">The capture location.</param>
        /// <param name="speeds">The raw speeds captured.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="speeds"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="speeds"/> is of zero length.
        /// </exception>
        public RawSpeedCaptureMessage(SpeedCaptureLocation location, string speeds)
        {
            Guard.NotNullOrEmpty(speeds, "speeds");

            Location = location;
            Speeds = speeds;
        }

        /// <inheritdoc/>
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc/>        
        public override string ToString()
        {
            return Repr("Location='{0}', Speeds='{1}'", Location, EscapeSpeeds(Speeds));
        }

        /// <summary>
        /// Gets the capture location.
        /// </summary>
        [PropertyId(0)]
        public SpeedCaptureLocation Location { get; private set; }

        /// <summary>
        /// Gets raw speeds captured.
        /// </summary>
        [PropertyId(1)]
        public string Speeds { get; private set; }

        #endregion

        #region Private Impl.

        private static string EscapeSpeeds(string speeds)
        {
            return speeds.Replace("\r", "\\r");
        }

        #endregion
    }
}