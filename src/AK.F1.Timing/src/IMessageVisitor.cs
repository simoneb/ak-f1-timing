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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Messages.Weather;

namespace AK.F1.Timing
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Message"/> visitor.
    /// </summary>
    public interface IMessageVisitor
    {
        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverPitTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(StartSessionTimeCountdownMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(EndOfSessionMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(StopSessionTimeCountdownMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(ClearGridRowMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetGridColumnColourMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetGridColumnValueMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(ReplaceDriverLapTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(ReplaceDriverSectorTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetElapsedSessionTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetAirTemperatureMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetAtmosphericPressureMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(AddCommentaryMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetCopyrightMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverLapTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverLapNumberMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverGapMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverCarNumberMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverIntervalMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverNameMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverPitCountMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverPositionMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverQuallyTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverSectorTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverStatusMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetKeyframeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetHumidityMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetRemainingSessionTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetRaceLapNumberMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetSessionStatusMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetPingIntervalMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetSystemMessageMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetSessionTypeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetStreamValidityMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetTrackTemperatureMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetIsWetMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetWindAngleMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetWindSpeedMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetNextMessageDelayMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetMinRequiredQuallyTimeMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SpeedCaptureMessage message);

        /// <summary>
        /// Visits the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to visit.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        void Visit(SetDriverSpeedMessage message);
    }
}