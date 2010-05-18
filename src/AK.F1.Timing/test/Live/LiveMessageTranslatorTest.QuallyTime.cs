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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllQuallyGridColumns_AndNumber))]
        public void when_a_driver_is_on_the_track_then_qually_time_column_values_are_translated_into_set_qually_time_and_set_lap_time_messages(
            GridColumn quallyColumn, int quallyNumber)
        {
            In(SessionType.Qually).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var time = TimeSpan.FromSeconds(80.123);

                driver.LapNumber = 5;
                driver.ChangeStatus(DriverStatus.OnTrack);

                var actual = translator.Translate(new SetGridColumnValueMessage(1, quallyColumn, GridColumnColour.White, "1:20.123"));

                Assert.IsType<CompositeMessage>(actual);

                var composite = (CompositeMessage)actual;

                Assert.Equal(2, composite.Messages.Count);
                Assert.MessagesAreEqual(
                    new SetDriverQuallyTimeMessage(1, quallyNumber, time),
                    composite.Messages[0]);
                Assert.MessagesAreEqual(
                    new SetDriverLapTimeMessage(1, new PostedTime(time, PostedTimeType.PersonalBest, 5)),
                    composite.Messages[1]);
            });
        }

        [Theory]
        [ClassData(typeof(AllQuallyGridColumns_AndNumber))]
        public void when_a_driver_is_not_on_the_track_then_qually_time_column_values_are_not_translated_into_set_qually_time_and_set_lap_time_messages(
            GridColumn quallyColumn, int quallyNumber)
        {
            In(SessionType.Qually).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var message = new SetGridColumnValueMessage(1, quallyColumn, GridColumnColour.White, "1:35.571");
                // In pits.
                driver.ChangeStatus(DriverStatus.InPits);
                Assert.Null(translator.Translate(message));
                // Out.
                driver.ChangeStatus(DriverStatus.Out);
                Assert.Null(translator.Translate(message));
                // Retired.
                driver.ChangeStatus(DriverStatus.Retired);
                Assert.Null(translator.Translate(message));
                // Stopped.
                driver.ChangeStatus(DriverStatus.Stopped);
                Assert.Null(translator.Translate(message));
            });
        }

        [Theory]
        [ClassData(typeof(AllQuallyGridColumns_AllSessionTypesExceptQually))]
        public void when_a_driver_is_on_the_track_during_a_non_qually_session_then_qually_time_column_values_are_not_translated_into_set_qually_time_and_set_lap_time_messages(
            GridColumn qually, SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);

                driver.LapNumber = 5;
                driver.ChangeStatus(DriverStatus.OnTrack);

                Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, qually, GridColumnColour.White, "1:35.571")));
            });
        }

        private sealed class AllQuallyGridColumns_AndNumber : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { GridColumn.Q1, 1 };
                yield return new object[] { GridColumn.Q2, 2 };
                yield return new object[] { GridColumn.Q3, 3 };
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class AllQuallyGridColumns_AllSessionTypesExceptQually : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return (from qually in A(GridColumn.Q1, GridColumn.Q2, GridColumn.Q3)
                        from session in Enum.GetValues(typeof(SessionType))
                            .Cast<object>()
                            .Except(A(SessionType.Qually))
                        select A(qually, session)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(params object[] args)
            {
                return args;
            }
        }
    }
}