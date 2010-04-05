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

using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Utility
{
    public class DisposableTest
    {
        [Fact]
        public void is_disposed_is_false_when_not_disposed() {

            var obj = new DisposableObject();

            Assert.False(obj.IsDisposed);
        }

        [Fact]
        public void is_disposed_is_true_when_disposed() {

            var obj = new DisposableObject();

            obj.Dispose();
            Assert.True(obj.IsDisposed);
        }

        [Fact]
        public void dispose_is_called_with_true_when_explicitly_disposed() {

            var obj = new DisposableObject();

            Assert.False(obj.Disposing);
            obj.Dispose();
            Assert.True(obj.Disposing);
        }

        [Fact]
        public void dispose_of_accepts_null() {

            Disposable.DisposeOf(null);
        }

        [Fact]
        public void dispose_of_disposes_of_disposable() {

            var obj = new DisposableObject();

            Assert.False(obj.IsDisposed);
            Disposable.DisposeOf(obj);
            Assert.True(obj.IsDisposed);
        }

        private sealed class DisposableObject : Disposable
        {
            public void Dispose() {

                ((IDisposable)this).Dispose();
            }

            protected override void Dispose(bool disposing) {

                Disposing = disposing;
                base.Dispose(disposing);
            }

            public bool Disposing { get; private set; }
        }
    }
}
