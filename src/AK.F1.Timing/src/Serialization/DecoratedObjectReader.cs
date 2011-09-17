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
using System.IO;
using System.Runtime.Serialization;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Reads objects that have been serialized by an <see cref="DecoratedObjectWriter"/>. This
    /// class cannot be inherited.
    /// </summary>
    public sealed class DecoratedObjectReader : DisposableBase, IObjectReader
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DecoratedObjectReader"/> class and
        /// specifies the underlying <paramref name="input"/> stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="input"/> is <see langword="null"/>.
        /// </exception>
        public DecoratedObjectReader(Stream input)
        {
            Guard.NotNull(input, "input");

            Context = new StreamingContext(StreamingContextStates.All);
            Input = DecoratedObjectWriter.CreateBinaryReader(input);
        }

        /// <inheritcdoc/>
        public T Read<T>()
        {
            var obj = Read();
            try
            {
                return (T)obj;
            }
            catch(InvalidCastException exc)
            {
                throw Guard.DecoratedObjectReader_UnableToCastToExpectedType(obj, typeof(T), exc);
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void DisposeOfManagedResources() { }

        #endregion

        #region Private Impl.

        private object Read()
        {
            CheckDisposed();
            try
            {
                var typeCode = ReadObjectTypeCode();
                if(typeCode == ObjectTypeCode.Object)
                {
                    return ReadObject();
                }
                return ReadPrimitive(typeCode);
            }
            catch(EndOfStreamException exc)
            {
                throw Guard.DecoratedObjectReader_UnexpectedEndOfStream(exc);
            }
        }

        private ObjectTypeCode ReadObjectTypeCode()
        {
            return (ObjectTypeCode)Input.ReadByte();
        }

        private object ReadObject()
        {
            var descriptor = TypeDescriptor.For(Input.ReadByte());
            var component = descriptor.Type.GetUninitializedInstance();
            var serializable = component as ICustomSerializable;
            if(serializable != null)
            {
                serializable.Read(this);
            }
            else
            {
                var propertyCount = Input.ReadByte();
                while(propertyCount-- > 0)
                {
                    var propertyId = Input.ReadByte();
                    var property = descriptor.Properties.GetById(propertyId);
                    if(property == null)
                    {
                        throw Guard.DecoratedObjectReader_PropertyMissing(propertyId, descriptor);
                    }
                    property.SetValue(component, Read());
                }
            }
            return GetRealObject(component);
        }

        private object GetRealObject(object instance)
        {
            var reference = instance as IObjectReference;
            return reference != null ? reference.GetRealObject(Context) : instance;
        }

        private object ReadPrimitive(ObjectTypeCode typeCode)
        {
            switch(typeCode)
            {
                case ObjectTypeCode.Empty:
                    return null;
                case ObjectTypeCode.Object:
                    Guard.Fail("ReadPrimitive should not have been called for an Object.");
                    return null;
                case ObjectTypeCode.DBNull:
                    return DBNull.Value;
                case ObjectTypeCode.Boolean:
                    return Input.ReadBoolean();
                case ObjectTypeCode.BooleanTrue:
                    return Boxed.BooleanTrue;
                case ObjectTypeCode.BooleanFalse:
                    return Boxed.BooleanFalse;
                case ObjectTypeCode.Char:
                    return Input.ReadChar();
                case ObjectTypeCode.SByte:
                    return Input.ReadSByte();
                case ObjectTypeCode.SByteZero:
                    return Boxed.SByteZero;
                case ObjectTypeCode.Byte:
                    return Input.ReadByte();
                case ObjectTypeCode.ByteZero:
                    return Boxed.ByteZero;
                case ObjectTypeCode.Int16:
                    return Input.ReadInt16();
                case ObjectTypeCode.Int16Zero:
                    return Boxed.Int16Zero;
                case ObjectTypeCode.UInt16:
                    return Input.ReadUInt16();
                case ObjectTypeCode.UInt16Zero:
                    return Boxed.UInt16Zero;
                case ObjectTypeCode.Int32:
                    return Input.ReadInt32();
                case ObjectTypeCode.Int32Zero:
                    return Boxed.Int32Zero;
                case ObjectTypeCode.UInt32:
                    return Input.ReadUInt32();
                case ObjectTypeCode.UInt32Zero:
                    return Boxed.UInt32Zero;
                case ObjectTypeCode.Int64:
                    return Input.ReadInt64();
                case ObjectTypeCode.Int64Zero:
                    return Boxed.Int64Zero;
                case ObjectTypeCode.UInt64:
                    return Input.ReadUInt64();
                case ObjectTypeCode.UInt64Zero:
                    return Boxed.UInt64Zero;
                case ObjectTypeCode.Single:
                    return Input.ReadSingle();
                case ObjectTypeCode.Double:
                    return Input.ReadDouble();
                case ObjectTypeCode.Decimal:
                    return Input.ReadDecimal();
                case ObjectTypeCode.DateTime:
                    return DateTime.FromBinary(Input.ReadInt64());
                case ObjectTypeCode.String:
                    return Input.ReadString();
                case ObjectTypeCode.TimeSpan:
                    return TimeSpan.FromTicks(Input.ReadInt64());
                case ObjectTypeCode.TimeSpanZero:
                    return Boxed.TimeSpanZero;
                case ObjectTypeCode.TimeSpanInt16:
                    return TimeSpan.FromTicks(Input.ReadInt16());
                case ObjectTypeCode.TimeSpanInt32:
                    return TimeSpan.FromTicks(Input.ReadInt32());
                default:
                    throw Guard.DecoratedObjectReader_InvalidObjectTypeCode(typeCode);
            }
        }

        private BinaryReader Input { get; set; }

        private StreamingContext Context { get; set; }

        private static class Boxed
        {
            public static readonly object ByteZero = (byte)0;
            public static readonly object SByteZero = (sbyte)0;
            public static readonly object Int16Zero = (short)0;
            public static readonly object UInt16Zero = (ushort)0;
            public static readonly object Int32Zero = (int)0;
            public static readonly object UInt32Zero = (uint)0;
            public static readonly object Int64Zero = (long)0;
            public static readonly object UInt64Zero = (ulong)0;
            public static readonly object BooleanTrue = true;
            public static readonly object BooleanFalse = false;
            public static readonly object TimeSpanZero = TimeSpan.Zero;
        }

        #endregion
    }
}