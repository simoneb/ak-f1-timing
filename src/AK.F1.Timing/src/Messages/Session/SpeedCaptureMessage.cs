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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messages.Session
{
    /// <summary>
    /// A message which contains the speeds captured at a specific location on the track.
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(63644251)]
    public sealed class SpeedCaptureMessage : Message, ICustomSerializable
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SpeedCaptureMessage"/> class
        /// and specifies the capture location <paramref name="location"/> and the captured
        /// <paramref name="speeds"/>.
        /// </summary>
        /// <param name="location">The capture location.</param>
        /// <param name="speeds">The captured speeds.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="speeds"/> is <see langword="null"/>.
        /// </exception>
        public SpeedCaptureMessage(SpeedCaptureLocation location, KeyValuePair<string, int>[] speeds)
        {
            Guard.NotNull(speeds, "speeds");

            Location = location;
            Speeds = Array.AsReadOnly(speeds);
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
            var sb = new StringBuilder();
            var culture = CultureInfo.InvariantCulture;
            sb.AppendFormat(culture, "Location='{0}', Speeds=[", Location);
            var appendComma = false;
            foreach(var speed in Speeds)
            {
                if(appendComma)
                {
                    sb.Append(", ");
                }
                sb.AppendFormat(culture, "(DriverName='{0}', Speed={1})", speed.Key, speed.Value);
                appendComma = true;
            }
            sb.Append(']');
            return Repr(sb.ToString());
        }

        /// <summary>
        /// Gets the capture location.
        /// </summary>
        [PropertyId(0)]
        public SpeedCaptureLocation Location { get; private set; }

        /// <summary>
        /// Gets captured speeds.
        /// </summary>
        [PropertyId(1)]
        public IList<KeyValuePair<string, int>> Speeds { get; private set; }

        #endregion

        #region Explicit Interface.

        void ICustomSerializable.Write(IObjectWriter writer)
        {
            Guard.NotNull(writer, "writer");

            writer.Write(Location);
            writer.Write(Speeds.Count);
            foreach(var speed in Speeds)
            {
                writer.Write(speed.Key);
                writer.Write(speed.Value);
            }
        }

        void ICustomSerializable.Read(IObjectReader reader)
        {
            Guard.NotNull(reader, "reader");

            Location = reader.Read<SpeedCaptureLocation>();
            var speeds = new KeyValuePair<string, int>[reader.Read<int>()];
            for(int i = 0; i < speeds.Length; ++i)
            {
                speeds[i] = new KeyValuePair<string, int>(reader.Read<string>(), reader.Read<int>());
            }
            Speeds = Array.AsReadOnly(speeds);
        }

        #endregion
    }
}