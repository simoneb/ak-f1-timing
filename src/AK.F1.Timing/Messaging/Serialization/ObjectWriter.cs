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

            this.Output.Write((byte)context.TypeCode);
            this.Output.Write(context.Descriptor.TypeId);
            this.Output.Write((byte)context.Descriptor.Properties.Count);
            foreach(var property in context.Descriptor.Properties) {
                this.Output.Write(property.PropertyId);
                Write(property.GetValue(context.Graph));
            }
        }

        private void WritePrimitive(ref GraphContext context) {

            this.Output.Write((byte)context.TypeCode);

            switch(context.TypeCode) {
                case ObjectTypeCode.Empty:
                    // Null has no members, obviously.
                    break;
                case ObjectTypeCode.Object:
                    Guard.Fail("WritePrimitive should not have been called for an Object.");
                    break;
                case ObjectTypeCode.DBNull:
                    // Null has no members and it is a singleton.
                    break;
                case ObjectTypeCode.Boolean:
                    this.Output.Write(Convert.ToBoolean(context.Graph));
                    break;
                case ObjectTypeCode.Char:
                    this.Output.Write(Convert.ToChar(context.Graph));
                    break;
                case ObjectTypeCode.SByte:
                    this.Output.Write(Convert.ToSByte(context.Graph));
                    break;
                case ObjectTypeCode.Byte:
                    this.Output.Write(Convert.ToByte(context.Graph));
                    break;
                case ObjectTypeCode.Int16:
                    this.Output.Write(Convert.ToInt16(context.Graph));
                    break;
                case ObjectTypeCode.UInt16:
                    this.Output.Write(Convert.ToUInt16(context.Graph));
                    break;
                case ObjectTypeCode.Int32:
                    this.Output.Write(Convert.ToInt32(context.Graph));
                    break;
                case ObjectTypeCode.UInt32:
                    this.Output.Write(Convert.ToUInt32(context.Graph));
                    break;
                case ObjectTypeCode.Int64:
                    this.Output.Write(Convert.ToInt64(context.Graph));
                    break;
                case ObjectTypeCode.UInt64:
                    this.Output.Write(Convert.ToUInt64(context.Graph));
                    break;
                case ObjectTypeCode.Single:
                    this.Output.Write(Convert.ToSingle(context.Graph));
                    break;
                case ObjectTypeCode.Double:
                    this.Output.Write(Convert.ToDouble(context.Graph));
                    break;
                case ObjectTypeCode.Decimal:
                    this.Output.Write(Convert.ToDecimal(context.Graph));
                    break;
                case ObjectTypeCode.DateTime:
                    this.Output.Write(Convert.ToDateTime(context.Graph).ToBinary());
                    break;
                case ObjectTypeCode.String:
                    this.Output.Write(Convert.ToString(context.Graph));
                    break;
                case ObjectTypeCode.TimeSpan:
                    this.Output.Write(((TimeSpan)context.Graph).Ticks);
                    break;
                default:
                    throw Guard.ArgumentOutOfRange("GraphContext.ObjectTypeCode");
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