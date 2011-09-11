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
using System.Diagnostics;
using Xunit;

namespace AK.F1.Timing.Server.Threading
{
    public class AutoResetEventSlimTest
    {
        [Fact]
        public void initial_state_is_signaled()
        {
            var e = new AutoResetEventSlim();

            Assert.True(e.Wait(0));
        }

        [Fact]
        public void subsequent_waits_are_automatially_blocked_until_set()
        {
            var e = new AutoResetEventSlim();

            Assert.True(e.Wait(0));
            Assert.False(e.Wait(0));

            e.Set();
            Assert.True(e.Wait(0));
        }

        [Fact]
        public void wait_blocks_thread_for_specified_timeout_if_not_released()
        {
            var e = new AutoResetEventSlim();

            var sw = Stopwatch.StartNew();
            e.Wait(0);
            sw.Stop();
            Assert.InRange(sw.Elapsed, TimeSpan.Zero, TimeSpan.FromMilliseconds(1));

            sw.Restart();
            e.Wait(100);
            sw.Stop();
            Assert.InRange(sw.Elapsed, TimeSpan.FromMilliseconds(90), TimeSpan.FromMilliseconds(110));
        }
    }
}