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

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Messages.Session;

namespace AK.F1.Timing.Fixup
{
    internal class MessageClassifier : MessageVisitor
    {
        #region Public Interface.

        public bool IsTranslated(Message message) {

            this.Result = false;

            message.Accept(this);

            return this.Result;
        }

        public override void Visit(SetSessionStatusMessage message) {

            // The live message reader does not receive a Finished status, the ping interval message
            // is translated when appropiate.
            this.Result = message.SessionStatus == SessionStatus.Finished;
        }

        public override void Visit(ReplaceDriverLapTimeMessage message) {

            this.Result = true;
        }

        public override void Visit(ReplaceDriverSectorTimeMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverCarNumberMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverCompletedLapsMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverGapMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverIntervalMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverLapNumberMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverLapTimeMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverNameMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverPitCountMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverPositionMessage message) {

            this.Result = false;
        }

        public override void Visit(SetDriverQuallyTimeMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverSectorTimeMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverStatusMessage message) {

            this.Result = true;
        }

        public override void Visit(SetRaceLapNumberMessage message) {

            this.Result = true;
        }

        public override void Visit(EndOfSessionMessage message) {

            this.Result = true;
        }

        public override void Visit(SetDriverPitTimeMessage message) {

            this.Result = true;
        }

        #endregion

        #region Private Impl.

        private bool Result { get; set; }

        #endregion
    }
}
