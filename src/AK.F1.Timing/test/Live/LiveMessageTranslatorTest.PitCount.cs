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
using AK.F1.Timing.Messages.Session;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void pit_count_columns_values_are_translated_into_set_pit_count_messages(SessionType session)
        {
            In(session).Assert(translator =>
            {
                Assert.MessagesAreEqual(
                    new SetDriverPitCountMessage(1, 2),
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.PitCount, GridColumnColour.White, "2"))
                );
            });
        }
    }
}