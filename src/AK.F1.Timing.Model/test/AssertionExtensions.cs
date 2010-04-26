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
using Xunit.Extensions;

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Model.Collections;

namespace AK.F1.Timing.Model.Driver
{
    public static class AssertExtensions
    {
        public static void Empty(this Assertions assert, DriverModel model) {

            assert.Equal(0, model.CarNumber);
            assert.Null(model.Gap);
            assert.Null(model.Interval);
            assert.Equal(0, model.LapsCompleted);
            assert.Empty(model.LapTimes);            
            assert.Null(model.Name);
            assert.Equal(0, model.PitCount);
            assert.Empty(model.PitTimes);
            assert.Equal(0, model.Position);
            assert.Empty(model.QuallyTimes);
            assert.Equal(DriverStatus.InPits, model.Status);
        }

        public static void Empty(this Assertions assert, PitTimesModel model) {

            assert.Empty(model.Items);
        }

        public static void Empty(this Assertions assert, QuallyTimesModel model) {

            assert.Null(model.Q1);
            assert.Null(model.Q2);
            assert.Null(model.Q3);
        }

        public static void Empty(this Assertions assert, LapTimesModel model) {

            assert.Empty(model.Laps);
            assert.Empty(model.S1);
            assert.Empty(model.S2);
            assert.Empty(model.S3);
        }

        public static void Empty(this Assertions assert, PostedTimeCollectionModel model) {

            assert.Empty(model.Items);
        }

        public static void NotEmpty(this Assertions assert, PostedTimeCollectionModel model) {

            assert.NotEmpty(model.Items);
        }
    }
}
