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
using System.Diagnostics;

namespace AK.F1.Timing.UI
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Internal Interface.

        [DebuggerStepThrough]
        internal static void NotNull<T>(T instance, string paramName)
        {
            if(instance == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void InRange(bool condition, string paramName)
        {
            if(!condition)
            {
                throw ArgumentOutOfRange(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty(string s, string paramName)
        {
            NotNull(s, paramName);
            if(s.Length == 0)
            {
                throw new ArgumentException(Resource.ArgEmptyString, paramName);
            }
        }

        internal static void ObjectDisposed(object instance)
        {
            throw new ObjectDisposedException(instance.GetType().Name);
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName)
        {
            return new ArgumentOutOfRangeException(paramName);
        }

        internal static NotImplementedException NotImplemented()
        {
            return new NotImplementedException();
        }

        private static string Format(string format, params object[] args)
        {
            return string.Format(Resource.Culture, format, args);
        }

        #endregion
    }
}