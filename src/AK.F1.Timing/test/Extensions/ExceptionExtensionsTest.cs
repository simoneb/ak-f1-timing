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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using AK.F1.Timing.Extensions;
using Xunit;

namespace AK.F1.Timing.Extensions
{
    public class ExceptionExtensionsTest
    {
        [Fact]
        public void is_fatal_correctly_identifies_fatal_exceptions() {

            foreach(var exception in GetFatalExceptions()) {
                Assert.True(ExceptionExtensions.IsFatal(exception),
                    "a " + exception.GetType() + " exception should be fatal");
            }
        }

        [Fact]
        public void is_fatal_correctly_identifies_non_fatal_exceptions() {

            foreach(var exception in GetNonFatalExceptions()) {
                Assert.False(ExceptionExtensions.IsFatal(exception),
                    "a " + (exception != null ? exception.GetType().FullName : "(null)") + " exception should not be fatal");
            }
        }

        private IEnumerable<Exception> GetFatalExceptions() {

            yield return (Exception)FormatterServices.GetUninitializedObject(typeof(ThreadAbortException));
            yield return new StackOverflowException();
            yield return new OutOfMemoryException();
            yield return new ExecutionEngineException();
            yield return new ArgumentException();
        }

        private IEnumerable<Exception> GetNonFatalExceptions() {

            yield return null;
            yield return new Exception();
            yield return new InvalidOperationException();
        }
    }
}
