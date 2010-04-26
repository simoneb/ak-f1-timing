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

using System;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Model.Driver
{
    public class LapTimesModelTest : TestClass
    {
        [Fact]
        public void can_create() {

            var model = new LapTimesModel();

            Assert.Empty(model);
        }

        [Fact]
        public void can_get_the_times_for_a_sector() {
            
            var model = new LapTimesModel();

            Assert.Same(model.S1, model.GetSector(1));
            Assert.Same(model.S2, model.GetSector(2));
            Assert.Same(model.S3, model.GetSector(3));
        }

        [Fact]
        public void get_sector_throws_if_sector_number_is_not_positive_or_greater_than_three() {

            var model = new LapTimesModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => model.GetSector(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => model.GetSector(4));
        }
    }
}