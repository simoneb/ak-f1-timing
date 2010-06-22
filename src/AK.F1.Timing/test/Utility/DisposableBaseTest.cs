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
    public class DisposableBaseTest
    {
        [Fact]
        public void is_disposed_is_false_when_not_disposed()
        {
            var obj = new DisposableObject();

            Assert.False(obj.IsDisposed);
        }

        [Fact]
        public void is_disposed_is_true_when_explicity_disposed()
        {
            var obj = new DisposableObject();

            obj.Dispose();
            Assert.True(obj.IsDisposed);
        }

        [Fact]
        public void is_disposed_is_true_when_implicitly_disposed()
        {
            var obj = new DisposableObject();

            obj.DoFinalize();
            Assert.True(obj.IsDisposed);
        }

        [Fact]
        public void dispose_of_managed_resources_is_called_when_explicitly_disposed()
        {
            var obj = new DisposableObject();

            obj.Dispose();
            Assert.Equal(1, obj.DisposeOfManagedResourcesCount);
        }

        [Fact]
        public void dispose_of_managed_resources_is_called_only_once_when_explicitly_disposed()
        {
            var obj = new DisposableObject();

            obj.Dispose();
            Assert.Equal(1, obj.DisposeOfManagedResourcesCount);
            obj.Dispose();
            Assert.Equal(1, obj.DisposeOfManagedResourcesCount);
        }

        [Fact]
        public void dispose_of_managed_resources_is_called_only_once_under_exceptional_circumstances_when_explicitly_disposed()
        {
            var obj = new ThrowingDisposableObject();

            Assert.Throws<Exception>(() => obj.Dispose());
            Assert.Equal(1, obj.DisposeOfManagedResourcesCount);
            Assert.DoesNotThrow(() => obj.Dispose());
            Assert.Equal(1, obj.DisposeOfManagedResourcesCount);
        }

        [Fact]
        public void dispose_of_managed_resources_is_not_called_only_once_under_exceptional_circumstances_when_implicity_disposed()
        {
            var obj = new DisposableObject();

            obj.DoFinalize();
            Assert.Equal(0, obj.DisposeOfManagedResourcesCount);
        }

        [Fact]
        public void dispose_of_accepts_null()
        {
            DisposableBase.DisposeOf(null);
        }

        [Fact]
        public void dispose_of_disposes_of_disposable()
        {
            var obj = new DisposableObject();

            Assert.False(obj.IsDisposed);
            DisposableBase.DisposeOf(obj);
            Assert.True(obj.IsDisposed);
        }

        private class DisposableObject : DisposableBase
        {
            public void Dispose()
            {
                ((IDisposable)this).Dispose();
            }

            public void DoFinalize()
            {
                Dispose(false);
            }

            protected override void DisposeOfManagedResources()
            {
                ++DisposeOfManagedResourcesCount;
            }

            public int DisposeOfManagedResourcesCount { get; private set; }
        }

        private class ThrowingDisposableObject : DisposableObject
        {
            protected override void DisposeOfManagedResources()
            {
                base.DisposeOfManagedResources();
                throw new Exception();
            }
        }
    }
}