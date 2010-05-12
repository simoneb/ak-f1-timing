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

using AK.F1.Timing.Messages.Session;
using Xunit;

namespace AK.F1.Timing.Model.Grid
{
    public class GridModelBaseTest
    {
        [Fact]
        public void can_create_a_model_for_each_session_type()
        {
            Assert.IsType<NullGridModel>(GridModelBase.Create(SessionType.None));
            Assert.IsType<PracticeGridModel>(GridModelBase.Create(SessionType.Practice));
            Assert.IsType<QuallyGridModel>(GridModelBase.Create(SessionType.Qually));
            Assert.IsType<RaceGridModel>(GridModelBase.Create(SessionType.Race));
        }
    }
}