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

namespace AK.F1.Timing.Utility.Server
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Validation.

        [DebuggerStepThrough]
#if DEBUG
        internal static void NotNull<T>(T instance, string paramName) where T : class
        {
#else
        internal static void NotNull(object instance, string paramName) {
#endif
            if(instance == null)
            {
                throw new ArgumentNullException(paramName);
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

        [DebuggerStepThrough]
        internal static void InRange(bool condition, string paramName)
        {
            if(!condition)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }

        #endregion
    }
}