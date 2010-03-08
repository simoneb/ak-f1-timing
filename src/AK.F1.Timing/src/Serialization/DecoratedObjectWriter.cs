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
using System.Runtime.InteropServices;
using System.Text;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Writes objects to an underlying output stream that have been decorated with a
    /// <see cref="TypeIdAttribute"/>. This class cannot be inherited.
    /// </summary>
    public sealed class DecoratedObjectWriter : Disposable, IObjectWriter
    {
        #region Fields.

        private static readonly Encoding TEXT_ENCODING = Encoding.UTF8;

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Creates a <see cref="System.IO.BinaryReader"/> suitable for reading data.
        /// </summary>
        /// <param name="input">The underlying input stream.</param>
        /// <returns>A <see cref="System.IO.BinaryReader"/>.</returns>
        internal static BinaryReader CreateBinaryReader(Stream input) {

            return new BinaryReader(input, TEXT_ENCODING);
        }

        /// <summary>
        /// Creates a <see cref="System.IO.BinaryWriter"/> suitable for writing data.
        /// </summary>
        /// <param name="output">The underlying output stream.</param>
        /// <returns>A <see cref="System.IO.BinaryWriter"/>.</returns>
        internal static BinaryWriter CreateBinaryWriter(Stream output) {

            // TODO consider compression:
            // - is it supported on .NETCF?
            // - is it worth the memory overhead?
            //   - average race session is < 1MiB
            //   - what is the average tms compression ratio?
            // - is the format likely to change? backwards compatibility is essential
            return new BinaryWriter(output, TEXT_ENCODING);
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DecoratedObjectWriter"/> class and specifies the
        /// underlying <paramref name="output"/> stream.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="output"/> is <see langword="null"/>.
        /// </exception>
        public DecoratedObjectWriter(Stream output) {

            Guard.NotNull(output, "output");

            this.Output = CreateBinaryWriter(output);
        }

        /// <inheritdoc/>        
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
                    WriteEmpty();
                    break;
                case ObjectTypeCode.DBNull:
                    WriteDBNull();
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

        private void WriteEmpty() {

            WriteObjectTypeCode(ObjectTypeCode.Empty);
        }

        private void WriteDBNull() {

            WriteObjectTypeCode(ObjectTypeCode.DBNull);
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

            WriteObjectTypeCode(ObjectTypeCode.TimeSpan);
            this.Output.Write(value.Ticks);
        }

        private void WriteString(string value) {

            WriteObjectTypeCode(ObjectTypeCode.String);
            this.Output.Write(value);
        }

        private void WriteDateTime(DateTime value) {

            WriteObjectTypeCode(ObjectTypeCode.DateTime);
            this.Output.Write(value.ToBinary());
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
#if DEBUG
            checked {
#endif
                if(value >= byte.MinValue && value <= byte.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.Byte);
                    this.Output.Write((byte)value);
                } else if(value >= short.MinValue && value <= short.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.Int16);
                    this.Output.Write((short)value);
                } else if(value >= int.MinValue && value <= int.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.Int32);
                    this.Output.Write((int)value);
                } else {
                    WriteObjectTypeCode(ObjectTypeCode.Int64);
                    this.Output.Write(value);
                }
#if DEBUG
            }
#endif
        }

        private void WriteUInt64(ulong value) {
#if DEBUG
            checked {
#endif
                if(value <= byte.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.Byte);
                    this.Output.Write((byte)value);
                } else if(value <= ushort.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.UInt16);
                    this.Output.Write((ushort)value);
                } else if(value <= uint.MaxValue) {
                    WriteObjectTypeCode(ObjectTypeCode.UInt32);
                    this.Output.Write((uint)value);
                } else {
                    WriteObjectTypeCode(ObjectTypeCode.UInt64);
                    this.Output.Write(value);
                }
#if DEBUG
            }
#endif
        }

        private static GraphContext CreateContext(object graph) {

            if(graph == null) {
                return GraphContext.Empty;
            }

            TypeDescriptor descriptor = null;
            ObjectTypeCode typeCode = graph.GetType().GetObjectTypeCode();

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

        [StructLayout(LayoutKind.Auto)]
        private struct GraphContext
        {
            public static readonly GraphContext Empty = new GraphContext() { 
                TypeCode = ObjectTypeCode.Empty 
            };

            public object Graph { get; set; }

            public TypeDescriptor Descriptor { get; set; }

            public ObjectTypeCode TypeCode { get; set; }
        }

        #endregion
    }
}