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
using Xunit;

namespace AK.F1.Timing.Utility
{
    public class DisposableCallbackTest
    {
        [Fact]
        public void ctor_should_throw_if_action_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => { new DisposableCallback(null); });
        }

        [Fact]
        public void action_should_only_be_called_when_disposed()
        {
            int count = 0;
            var callback = new DisposableCallback(() => ++count);

            Assert.Equal(0, count);
            callback.Dispose();
            Assert.Equal(1, count);
        }

        [Fact]
        public void action_should_only_be_called_once()
        {
            int count = 0;
            var callback = new DisposableCallback(() => ++count);

            Assert.Equal(0, count);
            callback.Dispose();
            Assert.Equal(1, count);
            callback.Dispose();
            Assert.Equal(1, count);
        }
    }
}