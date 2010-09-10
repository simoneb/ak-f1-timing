// Copyright 2010 Andy Kernahan
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
using AK.F1.Timing.Messages.Session;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void translator_does_not_thrown_when_a_lap_time_colour_update_is_processed_when_no_previous_lap_time_has_been_set(SessionType session)
        {
            In(session).Assert(translator =>
            {
                Message message = null;
                translator.Translate(new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.Red, "OUT"));
                Assert.DoesNotThrow(() =>
                {
                    message = translator.Translate(new SetGridColumnColourMessage(1, GridColumn.LapTime, GridColumnColour.White));
                });
                Assert.Null(message);
            });
        }
    }
}