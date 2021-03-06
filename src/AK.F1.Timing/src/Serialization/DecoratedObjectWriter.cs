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
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Serialization
{
    /// <summary>
    /// Writes objects to an underlying output stream that have been decorated with a
    /// <see cref="TypeIdAttribute"/>. This class cannot be inherited.
    /// </summary>
    public sealed class DecoratedObjectWriter : DisposableBase, IObjectWriter
    {
        #region Fields.

        private static readonly Encoding TextEncoding = Encoding.UTF8;

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Creates a <see cref="System.IO.BinaryReader"/> suitable for reading data.
        /// </summary>
        /// <param name="input">The underlying input stream.</param>
        /// <returns>A <see cref="System.IO.BinaryReader"/>.</returns>
        internal static BinaryReader CreateBinaryReader(Stream input)
        {
            return new BinaryReader(input, TextEncoding);
        }

        /// <summary>
        /// Creates a <see cref="System.IO.BinaryWriter"/> suitable for writing data.
        /// </summary>
        /// <param name="output">The underlying output stream.</param>
        /// <returns>A <see cref="System.IO.BinaryWriter"/>.</returns>
        internal static BinaryWriter CreateBinaryWriter(Stream output)
        {
            // TODO consider compression:
            // - is it supported on .NETCF?
            // - is it worth the memory overhead?
            //   - average race session is < 1MiB
            //   - what is the average tms compression ratio?
            // - is the format likely to change? backwards compatibility is essential
            return new BinaryWriter(output, TextEncoding);
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
        public DecoratedObjectWriter(Stream output)
        {
            Guard.NotNull(output, "output");

            Output = CreateBinaryWriter(output);
            SeenHashCodes = new HashSet<int>();
        }

        /// <inheritdoc/>        
        public void Write(object graph)
        {
            CheckDisposed();

            try
            {
                var context = CreateContext(graph);
                IsCompressionEnabled = context.TypeCode == ObjectTypeCode.Object;
                WriteGraph(ref context);
            }
            finally
            {
                SeenHashCodes.Clear();
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void DisposeOfManagedResources() { }

        #endregion

        #region Private Impl.

        private void WriteDescendant(object graph)
        {
            var context = CreateContext(graph);
            WriteGraph(ref context);
        }

        private void WriteGraph(ref GraphContext context)
        {
            if(context.TypeCode == ObjectTypeCode.Object)
            {
                AssertGraphHasNotBeenSeen(context.Graph);
                WriteComplex(ref context);
            }
            else
            {
                WritePrimitive(ref context);
            }
        }

        private void AssertGraphHasNotBeenSeen(object graph)
        {
            if(!SeenHashCodes.Add(RuntimeHelpers.GetHashCode(graph)))
            {
                throw Guard.DecoratedObjectWriter_CirularReferencesAreNotSupported(graph);
            }
        }

        private void WriteComplex(ref GraphContext context)
        {
            WriteObjectTypeCode(context.TypeCode);
            Output.Write(context.Descriptor.TypeId);
            var serializable = context.Graph as ICustomSerializable;
            if(serializable != null)
            {
                serializable.Write(this);
            }
            else
            {
                Output.Write((byte)context.Descriptor.Properties.Count);
                foreach(var property in context.Descriptor.Properties)
                {
                    Output.Write(property.PropertyId);
                    WriteDescendant(property.GetValue(context.Graph));
                }
            }
        }

        private void WritePrimitive(ref GraphContext context)
        {
            switch(context.TypeCode)
            {
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
                    WriteBoolean((bool)context.Graph);
                    break;
                case ObjectTypeCode.Char:
                    WriteChar((char)context.Graph);
                    break;
                case ObjectTypeCode.SByte:
                    WriteSByte((sbyte)context.Graph);
                    break;
                case ObjectTypeCode.Byte:
                    WriteByte((byte)context.Graph);
                    break;
                case ObjectTypeCode.Int16:
                    WriteInt16((short)context.Graph);
                    break;
                case ObjectTypeCode.Int32:
                    WriteInt32((int)context.Graph);
                    break;
                case ObjectTypeCode.Int64:
                    WriteInt64((long)context.Graph);
                    break;
                case ObjectTypeCode.UInt16:
                    WriteUInt16((ushort)context.Graph);
                    break;
                case ObjectTypeCode.UInt32:
                    WriteUInt32((uint)context.Graph);
                    break;
                case ObjectTypeCode.UInt64:
                    WriteUInt64((ulong)context.Graph);
                    break;
                case ObjectTypeCode.Single:
                    WriteSingle((float)context.Graph);
                    break;
                case ObjectTypeCode.Double:
                    WriteDouble((double)context.Graph);
                    break;
                case ObjectTypeCode.Decimal:
                    WriteDecimal((decimal)context.Graph);
                    break;
                case ObjectTypeCode.DateTime:
                    WriteDateTime((DateTime)context.Graph);
                    break;
                case ObjectTypeCode.String:
                    WriteString((string)context.Graph);
                    break;
                case ObjectTypeCode.TimeSpan:
                    WriteTimeSpan((TimeSpan)context.Graph);
                    break;
                default:
                    throw Guard.ArgumentOutOfRange("context.ObjectTypeCode");
            }
        }

        private void WriteEmpty()
        {
            WriteObjectTypeCode(ObjectTypeCode.Empty);
        }

        private void WriteDBNull()
        {
            WriteObjectTypeCode(ObjectTypeCode.DBNull);
        }

        private void WriteObjectTypeCode(ObjectTypeCode value)
        {
            Output.Write((byte)value);
        }

        private void WriteBoolean(bool value)
        {
            WriteObjectTypeCode(value ? ObjectTypeCode.BooleanTrue : ObjectTypeCode.BooleanFalse);
        }

        private void WriteChar(char value)
        {
            WriteObjectTypeCode(ObjectTypeCode.Char);
            Output.Write(value);
        }

        private void WriteSByte(sbyte value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.SByteZero);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.SByte);
                Output.Write(value);
            }
        }

        private void WriteByte(byte value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.ByteZero);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.Byte);
                Output.Write(value);
            }
        }

        private void WriteSingle(float value)
        {
            WriteObjectTypeCode(ObjectTypeCode.Single);
            Output.Write(value);
        }

        private void WriteTimeSpan(TimeSpan value)
        {
#if DEBUG
            checked
            {
#endif
                var ticks = value.Ticks;
                if(ticks == 0L)
                {
                    WriteObjectTypeCode(ObjectTypeCode.TimeSpanZero);
                }
                else if(ticks >= short.MinValue && ticks <= short.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.TimeSpanInt16);
                    Output.Write((short)ticks);
                }
                else if(ticks >= int.MinValue && ticks <= int.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.TimeSpanInt32);
                    Output.Write((int)ticks);
                }
                else
                {
                    WriteObjectTypeCode(ObjectTypeCode.TimeSpan);
                    Output.Write(ticks);
                }
#if DEBUG
            }
#endif
        }

        private void WriteString(string value)
        {
            WriteObjectTypeCode(ObjectTypeCode.String);
            Output.Write(value);
        }

        private void WriteDateTime(DateTime value)
        {
            WriteObjectTypeCode(ObjectTypeCode.DateTime);
            Output.Write(value.ToBinary());
        }

        private void WriteDecimal(decimal value)
        {
            WriteObjectTypeCode(ObjectTypeCode.Decimal);
            Output.Write(value);
        }

        private void WriteDouble(double value)
        {
            WriteObjectTypeCode(ObjectTypeCode.Double);
            Output.Write(value);
        }

        private void WriteInt16(short value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.Int16Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteInt64WithComression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.Int16);
                Output.Write(value);
            }
        }

        private void WriteInt32(int value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.Int32Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteInt64WithComression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.Int32);
                Output.Write(value);
            }
        }

        private void WriteInt64(long value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.Int64Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteInt64WithComression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.Int64);
                Output.Write(value);
            }
        }

        private void WriteInt64WithComression(long value)
        {
#if DEBUG
            checked
            {
#endif
                if(value >= byte.MinValue && value <= byte.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.Byte);
                    Output.Write((byte)value);
                }
                else if(value >= short.MinValue && value <= short.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.Int16);
                    Output.Write((short)value);
                }
                else if(value >= int.MinValue && value <= int.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.Int32);
                    Output.Write((int)value);
                }
                else
                {
                    WriteObjectTypeCode(ObjectTypeCode.Int64);
                    Output.Write(value);
                }
#if DEBUG
            }
#endif
        }

        private void WriteUInt16(ushort value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt16Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteUInt64WithCompression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt16);
                Output.Write(value);
            }
        }

        private void WriteUInt32(uint value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt32Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteUInt64WithCompression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt32);
                Output.Write(value);
            }
        }

        private void WriteUInt64(ulong value)
        {
            if(value == 0)
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt64Zero);
            }
            else if(IsCompressionEnabled)
            {
                WriteUInt64WithCompression(value);
            }
            else
            {
                WriteObjectTypeCode(ObjectTypeCode.UInt64);
                Output.Write(value);
            }
        }

        private void WriteUInt64WithCompression(ulong value)
        {
#if DEBUG
            checked
            {
#endif
                if(value <= byte.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.Byte);
                    Output.Write((byte)value);
                }
                else if(value <= ushort.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.UInt16);
                    Output.Write((ushort)value);
                }
                else if(value <= uint.MaxValue)
                {
                    WriteObjectTypeCode(ObjectTypeCode.UInt32);
                    Output.Write((uint)value);
                }
                else
                {
                    WriteObjectTypeCode(ObjectTypeCode.UInt64);
                    Output.Write(value);
                }
#if DEBUG
            }
#endif
        }

        private static GraphContext CreateContext(object graph)
        {
            if(graph == null)
            {
                return new GraphContext
                {
                    TypeCode = ObjectTypeCode.Empty
                };
            }

            TypeDescriptor descriptor = null;
            ObjectTypeCode typeCode = graph.GetType().GetObjectTypeCode();

            if(typeCode == ObjectTypeCode.Object)
            {
                descriptor = TypeDescriptor.For(graph.GetType());
            }

            return new GraphContext
            {
                Descriptor = descriptor,
                Graph = graph,
                TypeCode = typeCode
            };
        }

        private BinaryWriter Output { get; set; }

        private HashSet<int> SeenHashCodes { get; set; }

        private bool IsCompressionEnabled { get; set; }

        [StructLayout(LayoutKind.Auto)]
        private struct GraphContext
        {
            public object Graph { get; set; }

            public TypeDescriptor Descriptor { get; set; }

            public ObjectTypeCode TypeCode { get; set; }
        }

        #endregion
    }
}