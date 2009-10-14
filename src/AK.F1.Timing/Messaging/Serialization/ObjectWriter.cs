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
using System.IO;
using System.Text;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Messaging.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectWriter : Disposable
    {
        #region Fields.

        internal static readonly Encoding TEXT_ENCODING = Encoding.UTF8;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectWriter"/> class and specifies the
        /// underlying <paramref name="output"/> stream.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="output"/> is <see langword="null"/>.
        /// </exception>
        public ObjectWriter(Stream output) {

            Guard.NotNull(output, "output");

            this.Output = new BinaryWriter(output, TEXT_ENCODING);
        }

        /// <summary>
        /// Writes the specified graph to the underlying stream.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed of.
        /// </exception>
        public void Write(object graph) {

            CheckDisposed();

            var context = CreateContext(graph);

            if(context.TypeCode == ObjectTypeCode.Object) {
                WriteObject(ref context);
            } else {
                WritePrimitive(ref context);
            }
        }

        #endregion

        #region Private Impl.

        private void WriteObject(ref GraphContext context) {

            WriteObjectTypeCode(context.TypeCode);
            this.Output.Write(context.Descriptor.TypeId);
            this.Output.Write((byte)context.Descriptor.Properties.Count);
            foreach(var property in context.Descriptor.Properties) {
                this.Output.Write(property.PropertyId);
                Write(property.GetValue(context.Graph));
            }
        }

        private void WritePrimitive(ref GraphContext context) {            

            switch(context.TypeCode) {
                case ObjectTypeCode.Empty:
                case ObjectTypeCode.DBNull:
                    break;
                case ObjectTypeCode.Object:
                    Guard.Fail("WritePrimitive should not have been called for an Object.");
                    break;
                case ObjectTypeCode.Boolean:
                    WriteBoolean(Convert.ToBoolean(context.Graph));
                    break;
                case ObjectTypeCode.Char:
                    WriteChar(Convert.ToChar(context.Graph));
                    break;
                case ObjectTypeCode.SByte:
                    WriteSByte(Convert.ToSByte(context.Graph));
                    break;
                case ObjectTypeCode.Byte:
                    WriteByte(Convert.ToByte(context.Graph));
                    break;
                case ObjectTypeCode.Int16:
                case ObjectTypeCode.Int32:
                case ObjectTypeCode.Int64:
                    WriteInt64(Convert.ToInt64(context.Graph));
                    break;
                case ObjectTypeCode.UInt16:
                case ObjectTypeCode.UInt32:
                case ObjectTypeCode.UInt64:
                    WriteUInt64(Convert.ToUInt64(context.Graph));
                    break;
                case ObjectTypeCode.Single:
                    WriteSingle(Convert.ToSingle(context.Graph));
                    break;
                case ObjectTypeCode.Double:
                    WriteDouble(Convert.ToDouble(context.Graph));
                    break;
                case ObjectTypeCode.Decimal:
                    WriteDecimal(Convert.ToDecimal(context.Graph));
                    break;
                case ObjectTypeCode.DateTime:
                    WriteDateTime(Convert.ToDateTime(context.Graph));
                    break;
                case ObjectTypeCode.String:
                    WriteString(Convert.ToString(context.Graph));
                    break;
                case ObjectTypeCode.TimeSpan:
                    WriteTimeSpan((TimeSpan)context.Graph);
                    break;
                default:
                    throw Guard.ArgumentOutOfRange("GraphContext.ObjectTypeCode");
            }
        }

        private void WriteObjectTypeCode(ObjectTypeCode value) {

            this.Output.Write((byte)value);
        }

        private void WriteBoolean(bool value) {

            WriteObjectTypeCode(ObjectTypeCode.Boolean);
            this.Output.Write(value);
        }

        private void WriteChar(char value) {

            WriteObjectTypeCode(ObjectTypeCode.Char);
            this.Output.Write(value);
        }

        private void WriteSByte(sbyte value) {

            WriteObjectTypeCode(ObjectTypeCode.SByte);
            this.Output.Write(value);
        }

        private void WriteByte(byte value) {

            WriteObjectTypeCode(ObjectTypeCode.Byte);
            this.Output.Write(value);
        }

        private void WriteSingle(float value) {

            WriteObjectTypeCode(ObjectTypeCode.Single);
            this.Output.Write(value);
        }

        private void WriteTimeSpan(TimeSpan value) {

            WriteInt64(value.Ticks);
        }

        private void WriteString(string value) {

            WriteObjectTypeCode(ObjectTypeCode.String);
            this.Output.Write(value);
        }

        private void WriteDateTime(DateTime value) {

            WriteInt64(value.ToBinary());
        }

        private void WriteDecimal(decimal value) {

            WriteObjectTypeCode(ObjectTypeCode.Decimal);
            this.Output.Write(value);
        }

        private void WriteDouble(double value) {
            
            WriteObjectTypeCode(ObjectTypeCode.Double);
            this.Output.Write(value);
        }

        private void WriteInt64(long value) {

            if(value >= byte.MinValue && value <= byte.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.Byte);
                this.Output.Write(checked((byte)value));
            } else if(value >= short.MinValue && value <= short.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.Int16);
                this.Output.Write(checked((short)value));
            } else if(value >= int.MinValue && value <= int.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.Int32);
                this.Output.Write(checked((int)value));
            } else {
                WriteObjectTypeCode(ObjectTypeCode.Int64);
                this.Output.Write(value);
            }
        }

        private void WriteUInt64(ulong value) {

            if(value <= byte.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.Byte);
                this.Output.Write(checked((byte)value));
            } else if(value <= ushort.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.UInt16);
                this.Output.Write(checked((ushort)value));
            } else if(value <= uint.MaxValue) {
                WriteObjectTypeCode(ObjectTypeCode.UInt32);
                this.Output.Write(checked((uint)value));
            } else {
                WriteObjectTypeCode(ObjectTypeCode.UInt64);
                this.Output.Write(value);
            }
        }

        private static GraphContext CreateContext(object graph) {

            if(graph == null) {
                return new GraphContext() { TypeCode = ObjectTypeCode.Empty };
            }

            TypeDescriptor descriptor = null;
            ObjectTypeCode typeCode = SerializationHelper.GetObjectTypeCode(graph.GetType());

            if(typeCode == ObjectTypeCode.Object) {
                descriptor = TypeDescriptor.For(graph.GetType());
            }

            return new GraphContext() {
                Descriptor = descriptor,
                Graph = graph,
                TypeCode = typeCode
            };
        }

        private BinaryWriter Output { get; set; }

        private struct GraphContext
        {
            public object Graph { get; set; }

            public TypeDescriptor Descriptor { get; set; }

            public ObjectTypeCode TypeCode { get; set; }
        }

        #endregion
    }
}