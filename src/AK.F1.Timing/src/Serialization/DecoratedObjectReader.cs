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
    public sealed class DecoratedObjectReader : Disposable, IObjectReader
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
        public object Read()
        {
            CheckDisposed();

            ObjectTypeCode typeCode;

            try
            {
                typeCode = ReadObjectTypeCode();
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

        #endregion

        #region Private Impl.

        private ObjectTypeCode ReadObjectTypeCode()
        {
            return (ObjectTypeCode)Input.ReadByte();
        }

        private object ReadObject()
        {
            byte propertyId;
            PropertyDescriptor property;
            TypeDescriptor descriptor = TypeDescriptor.For(Input.ReadInt32());
            object component = descriptor.Type.GetUninitializedInstance();
            byte propertyCount = Input.ReadByte();

            while(propertyCount-- > 0)
            {
                propertyId = Input.ReadByte();
                if((property = descriptor.Properties.GetById(propertyId)) == null)
                {
                    throw Guard.DecoratedObjectReader_PropertyMissing(propertyId, descriptor);
                }
                property.SetValue(component, Read());
            }

            return GetRealObject(component);
        }

        private object GetRealObject(object instance)
        {
            IObjectReference reference = instance as IObjectReference;

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
                case ObjectTypeCode.Char:
                    return Input.ReadChar();
                case ObjectTypeCode.SByte:
                    return Input.ReadSByte();
                case ObjectTypeCode.Byte:
                    return Input.ReadByte();
                case ObjectTypeCode.Int16:
                    return Input.ReadInt16();
                case ObjectTypeCode.UInt16:
                    return Input.ReadUInt16();
                case ObjectTypeCode.Int32:
                    return Input.ReadInt32();
                case ObjectTypeCode.UInt32:
                    return Input.ReadUInt32();
                case ObjectTypeCode.Int64:
                    return Input.ReadInt64();
                case ObjectTypeCode.UInt64:
                    return Input.ReadUInt64();
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
                default:
                    throw Guard.DecoratedObjectReader_InvalidObjectTypeCode(typeCode);
            }
        }

        private BinaryReader Input { get; set; }

        private StreamingContext Context { get; set; }

        #endregion
    }
}