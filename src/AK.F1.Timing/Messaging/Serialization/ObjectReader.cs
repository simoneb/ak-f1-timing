﻿// Copyright 2009 Andy Kernahan
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
using System.IO;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Messaging.Serialization
{
    /// <summary>
    /// Reads objects that have been serialized by an <see cref="ObjectWriter"/>. This class
    /// is <see langword="sealed"/>.
    /// </summary>
    public sealed class ObjectReader : Disposable
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectReader"/> class and specifies the
        /// underlying <paramref name="input"/> stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="input"/> is <see langword="null"/>.
        /// </exception>
        public ObjectReader(Stream input) {

            Guard.NotNull(input, "input");

            this.Input = new BinaryReader(input, ObjectWriter.TEXT_ENCODING);
        }

        /// <summary>
        /// Reads a serialized object from the underlying stream.
        /// </summary>
        /// <returns>The serialized object from the underlying stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">
        /// Thrown when the end of the underlying stream has been reached.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when an error occurs during deserialization.
        /// </exception>
        public object Read() {

            CheckDisposed();

            var typeCode = ReadObjectTypeCode();

            try {
                if(typeCode == ObjectTypeCode.Object) {
                    return ReadObject();
                }
                return ReadPrimitive(typeCode);
            } catch(EndOfStreamException) {
                throw Guard.ObjectReader_UnexpectedEndOfStream();
            }
        }

        #endregion

        #region Private Impl.

        private ObjectTypeCode ReadObjectTypeCode() {

            return (ObjectTypeCode)this.Input.ReadByte();
        }

        private object ReadObject() {
            
            TypeDescriptor descriptor = TypeDescriptor.For(this.Input.ReadInt32());
            object instance = SerializationHelper.GetUninitializedObject(descriptor.Type);
            byte propertyCount = this.Input.ReadByte();

            while(propertyCount-- > 0) {
                try {
                    descriptor.Properties[this.Input.ReadByte()].SetValue(instance, Read());
                } catch(KeyNotFoundException) {
                    continue;
                }
            }

            return instance;
        }

        private object ReadPrimitive(ObjectTypeCode typeCode) {

            switch(typeCode) {
                case ObjectTypeCode.Empty:
                    return null;
                case ObjectTypeCode.Object:
                    Guard.Fail("ReadPrimitive should not have been called for an Object.");
                    return null;
                case ObjectTypeCode.DBNull:
                    return DBNull.Value;
                case ObjectTypeCode.Boolean:
                    return this.Input.ReadBoolean();
                case ObjectTypeCode.Char:
                    return this.Input.ReadChar();
                case ObjectTypeCode.SByte:
                    return this.Input.ReadSByte();
                case ObjectTypeCode.Byte:
                    return this.Input.ReadByte();
                case ObjectTypeCode.Int16:
                    return this.Input.ReadInt16();
                case ObjectTypeCode.UInt16:
                    return this.Input.ReadUInt16();
                case ObjectTypeCode.Int32:
                    return this.Input.ReadInt32();
                case ObjectTypeCode.UInt32:
                    return this.Input.ReadUInt32();
                case ObjectTypeCode.Int64:
                    return this.Input.ReadInt64();
                case ObjectTypeCode.UInt64:
                    return this.Input.ReadUInt64();
                case ObjectTypeCode.Single:
                    return this.Input.ReadSingle();
                case ObjectTypeCode.Double:
                    return this.Input.ReadDouble();
                case ObjectTypeCode.Decimal:
                    return this.Input.ReadDecimal();
                case ObjectTypeCode.DateTime:
                    return DateTime.FromBinary(this.Input.ReadInt64());
                case ObjectTypeCode.String:
                    return this.Input.ReadString();
                case ObjectTypeCode.TimeSpan:
                    return TimeSpan.FromTicks(this.Input.ReadInt64());
                default:
                    throw Guard.ArgumentOutOfRange("typeCode");
            }
        }

        private BinaryReader Input { get; set; }

        #endregion
    }
}