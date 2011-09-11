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
using System.Threading;
using Xunit;

namespace AK.F1.Timing.Server.Extensions
{
    public class ReaderWriterLockSlimExtensionsTest
    {
        [Fact]
        public void can_execute_action_in_read_lock()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.False(locker.IsReadLockHeld);
                ReaderWriterLockSlimExtensions.InReadLock(locker, () =>
                {
                    Assert.True(locker.IsReadLockHeld);
                });
                Assert.False(locker.IsReadLockHeld);
            }
        }

        [Fact]
        public void read_lock_is_released_when_action_throws()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.False(locker.IsReadLockHeld);
                Assert.Throws<Exception>(() =>
                {
                    ReaderWriterLockSlimExtensions.InReadLock(locker, () =>
                    {
                        Assert.True(locker.IsReadLockHeld);
                        throw new Exception();
                    });
                });
                Assert.False(locker.IsReadLockHeld);
            }
        }

        [Fact]
        public void in_read_lock_throws_if_locker_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ReaderWriterLockSlimExtensions.InReadLock(null, delegate { }));
        }

        [Fact]
        public void in_read_lock_throws_if_action_is_null()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.Throws<ArgumentNullException>(() => ReaderWriterLockSlimExtensions.InReadLock(locker, null));
            }
        }

        [Fact]
        public void can_execute_action_in_write_lock()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.False(locker.IsWriteLockHeld);
                ReaderWriterLockSlimExtensions.InWriteLock(locker, () =>
                {
                    Assert.True(locker.IsWriteLockHeld);
                });
                Assert.False(locker.IsWriteLockHeld);
            }
        }

        [Fact]
        public void write_lock_is_released_when_action_throws()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.False(locker.IsWriteLockHeld);
                Assert.Throws<Exception>(() =>
                {
                    ReaderWriterLockSlimExtensions.InWriteLock(locker, () =>
                    {
                        Assert.True(locker.IsWriteLockHeld);
                        throw new Exception();
                    });
                });
                Assert.False(locker.IsWriteLockHeld);
            }
        }

        [Fact]
        public void in_writer_lock_throws_if_locker_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ReaderWriterLockSlimExtensions.InWriteLock(null, delegate { }));
        }

        [Fact]
        public void in_write_lock_throws_if_action_is_null()
        {
            using(var locker = new ReaderWriterLockSlim())
            {
                Assert.Throws<ArgumentNullException>(() => ReaderWriterLockSlimExtensions.InWriteLock(locker, null));
            }
        }
    }
}