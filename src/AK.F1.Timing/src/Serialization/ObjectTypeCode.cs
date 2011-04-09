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

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// A <see cref="System.TypeCode"/> superset which specifies other well known types.
    /// </summary>
    [Serializable]
    public enum ObjectTypeCode
    {
        #region TypeCode Members.

        /// <summary>
        /// A null reference.
        /// </summary>
        Empty = 0,
        /// <summary>
        /// A <see cref="System.Object"/>.
        /// </summary>
        Object = 1,
        /// <summary>
        /// A <see cref="System.DBNull"/>.
        /// </summary>
        DBNull = 2,
        /// <summary>
        /// A <see cref="System.Boolean"/>.
        /// </summary>
        Boolean = 3,
        /// <summary>
        /// A <see cref="System.Char"/>.
        /// </summary>
        Char = 4,
        /// <summary>
        /// A <see cref="System.SByte"/>.
        /// </summary>
        SByte = 5,
        /// <summary>
        /// A <see cref="System.Byte"/>.
        /// </summary>
        Byte = 6,
        /// <summary>
        /// A <see cref="System.Int16"/>.
        /// </summary>
        Int16 = 7,
        /// <summary>
        /// A <see cref="System.UInt16"/>.
        /// </summary>
        UInt16 = 8,
        /// <summary>
        /// A <see cref="System.Int32"/>.
        /// </summary>
        Int32 = 9,
        /// <summary>
        /// A <see cref="System.UInt32"/>.
        /// </summary>
        UInt32 = 10,
        /// <summary>
        /// A <see cref="System.Int64"/>.
        /// </summary>
        Int64 = 11,
        /// <summary>
        /// A <see cref="System.UInt64"/>.
        /// </summary>
        UInt64 = 12,
        /// <summary>
        /// A <see cref="System.Single"/>.
        /// </summary>
        Single = 13,
        /// <summary>
        /// A <see cref="System.Double"/>.
        /// </summary>
        Double = 14,
        /// <summary>
        /// A <see cref="System.Decimal"/>.
        /// </summary>
        Decimal = 15,
        /// <summary>
        /// A <see cref="System.DateTime"/>.
        /// </summary>
        DateTime = 16,
        /// <summary>
        /// A <see cref="System.String"/>.
        /// </summary>
        String = 18,

        #endregion

        #region Extended Members.

        /// <summary>
        /// A <see cref="System.TimeSpan"/>.
        /// </summary>
        TimeSpan = 19,
        /// <summary>
        /// A zero <see cref="System.TimeSpan"/>.
        /// </summary>
        TimeSpanZero = 20,
        /// <summary>
        /// A <see cref="System.TimeSpan"/> compressed to an <see cref="System.Int16"/>.
        /// </summary>
        TimeSpanInt16 = 21,
        /// <summary>
        /// A <see cref="System.TimeSpan"/> compressed to an <see cref="System.Int32"/>.
        /// </summary>
        TimeSpanInt32 = 22,
        /// <summary>
        /// A zero <see cref="System.SByte"/>.
        /// </summary>
        SByteZero = 23,
        /// <summary>
        /// A zero <see cref="System.Byte"/>.
        /// </summary>
        ByteZero = 24,
        /// <summary>
        /// A zero <see cref="System.Int16"/>.
        /// </summary>
        Int16Zero = 25,
        /// <summary>
        /// A zero <see cref="System.UInt16"/>.
        /// </summary>
        UInt16Zero = 26,
        /// <summary>
        /// A zero <see cref="System.Int32"/>.
        /// </summary>
        Int32Zero = 27,
        /// <summary>
        /// A zero <see cref="System.UInt32"/>.
        /// </summary>
        UInt32Zero = 28,
        /// <summary>
        /// A zero <see cref="System.Int64"/>.
        /// </summary>
        Int64Zero = 29,
        /// <summary>
        /// A zero <see cref="System.UInt64"/>.
        /// </summary>
        UInt64Zero = 30,
        /// <summary>
        /// A <see cref="System.Boolean"/> true.
        /// </summary>
        BooleanTrue = 31,
        /// <summary>
        /// A <see cref="System.Boolean"/> false.
        /// </summary>
        BooleanFalse = 32,

        #endregion
    }
}