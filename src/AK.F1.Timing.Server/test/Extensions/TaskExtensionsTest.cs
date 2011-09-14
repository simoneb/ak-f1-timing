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
using System.Threading.Tasks;
using Xunit;

namespace AK.F1.Timing.Server.Extensions
{
    public class TaskExtensionsTest
    {
        [Fact]
        public void continuation_action_is_invoked_when_tasks_faults()
        {
            using(var task = new Task(ThrowException))
            {
                int invocationCount = 0;
                var continuationTask = TaskExtensions.ContinueFaultWith(task, _ => ++invocationCount);
                Assert.NotNull(continuationTask);

                task.Start();
                continuationTask.Wait(1000);

                Assert.Equal(1, invocationCount);
            }
        }

        [Fact]
        public void continuation_action_is_not_invoked_when_tasks_completes_successfully()
        {
            using(var task = new Task(DoNothing))
            {
                var invoked = false;
                var continuationTask = TaskExtensions.ContinueFaultWith(task, _ => invoked = true);
                Assert.NotNull(continuationTask);

                task.Start();
                continuationTask.Wait(1000);

                Assert.False(invoked);
            }
        }

        private static void ThrowException()
        {
            throw new Exception();
        }

        private static void DoNothing()
        {
        }
    }
}