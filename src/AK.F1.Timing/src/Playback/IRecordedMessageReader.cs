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

namespace AK.F1.Timing.Playback
{
    /// <summary>
    /// Allows recorded <see cref="AK.F1.Timing.Message"/>s to be read from an underlying
    /// data stream and played back at a certain speed.
    /// </summary>
    public interface IRecordedMessageReader : IMessageReader
    {
        /// <summary>
        /// Gets or sets the playback speed.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="value"/> is not positive.
        /// </exception>
        double PlaybackSpeed { get; set; }
    }
}