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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Session;
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Internal Interface.

        [DebuggerStepThrough]
        internal static void NotNull<T>(T instance, string paramName) where T: class {

            if(instance == null) {
                throw new ArgumentNullException(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void InRange(bool condition, string paramName) {

            if(!condition) {
                throw ArgumentOutOfRange(paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty(string s, string paramName) {

            NotNull(s, paramName);
            if(s.Length == 0) {
                throw new ArgumentException(Resource.ArgEmptyString, paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void NotNullOrEmpty<T>(IEnumerable<T> collection, string paramName) {

            NotNull(collection, paramName);

            using(IEnumerator<T> elements = collection.GetEnumerator()) {
                if(!elements.MoveNext()) {
                    throw new ArgumentException(Resource.ArgNonEmptyCollection, paramName);
                }
            }
        }

        [DebuggerStepThrough]
        internal static void DirectoryExists(string path, string paramName) {

            if(!Directory.Exists(path)) {
                throw DirectoryNotFound(path, paramName);
            }
        }

        [DebuggerStepThrough]
        internal static void CheckBufferArgs<T>(T[] buffer, int offset, int count) {

            NotNull(buffer, "buffer");
            InRange(offset <= buffer.Length, "offset");
            InRange(offset + count < buffer.Length, "count");
        }

        internal static DirectoryNotFoundException DirectoryNotFound(string path, string paramName) {

            return new DirectoryNotFoundException(path);
        }

        internal static EndOfStreamException UnexpectedEndOfStream() {

            return new EndOfStreamException(Resource.UnexpectedEndOfStreamException);
        }

        internal static ObjectDisposedException ObjectDisposed(object instance) {

            return new ObjectDisposedException(instance.GetType().FullName);
        }

        internal static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName) {

            return new ArgumentOutOfRangeException(paramName);
        }

        internal static NotImplementedException NotImplemented() {

            return new NotImplementedException();
        }

        private static string Format(string format, params object[] args) {

            return string.Format(Resource.Culture, format, args);
        }

        internal static FormatException LiveMessageReader_InvalidMessageType(int messageType) {

            return new FormatException(Format(Resource.LiveMessageReader_InvalidMessageType, messageType));
        }

        internal static void Fail(string message) {

            Assert(false, message);
        }

        internal static void Assert(bool condition) {

            Assert(condition, string.Empty);
        }
        
        internal static void Assert(bool condition, string message) {

            if(!condition) {
                Debug.Assert(condition, message);
                Trace.Assert(condition, message);
                throw new InvalidProgramException(message);
            }
        }

        internal static FormatException LiveData_UnableToConvertToSessionStatus(string s) {

            return new FormatException(Format(Resource.LiveData_UnableToConvertToSessionStatus, s));
        }

        internal static FormatException LiveData_UnableToConvertToDriverStatus(int value) {

            return new FormatException(Format(Resource.LiveData_UnableToConvertToDriverStatus, value));
        }

        internal static FormatException LiveData_UnableToConvertToSessionType(int value) {

            return new FormatException(Format(Resource.LiveData_UnableToConvertToSessionType, value));
        }

        internal static FormatException LiveData_UnableToConvertToPostedTimeType(int value) {

            return new FormatException(Format(Resource.LiveData_UnableToConvertToPostedTimeType, value));
        }

        internal static FormatException LiveData_UnableToConvertToGridColumn(byte column, SessionType currentSessionType) {

            return new FormatException(Format(Resource.LiveData_UnableToConvertToGridColumn, column, currentSessionType));
        }

        internal static FormatException LiveData_UnableToParseTime(string value) {

            return new FormatException(Format(Resource.LiveData_UnableToParseTime, value));
        }

        internal static FormatException LiveData_UnableToParseInt32(string s) {

            return new FormatException(Format(Resource.LiveData_UnableToParseInt32, s));
        }

        internal static FormatException LiveData_UnableToParseDouble(string s) {

            return new FormatException(Format(Resource.LiveData_UnableToParseDouble, s));
        }

        internal static FormatException LiveMessageReader_UnexpectedFirstMessage(Message message) {

            return new FormatException(Format(Resource.LiveMessageReader_UnexpectedFirstMessage, message));
        }

        internal static FormatException MessageReader_InvalidMessage() {

            return new FormatException(Resource.MessageReader_InvalidMessage);
        }

        internal static FormatException MessageReader_InvalidMessage(Exception exc) {

            return new FormatException(Resource.MessageReader_InvalidMessage, exc);
        }

        internal static ArgumentException TimeGap_InvalidCompareToArgument(object obj) {

            return new ArgumentException(Format(Resource.TimeGap_InvalidCompareToArgument,
                typeof(TimeGap).FullName, obj.GetType().FullName), "obj");
        }

        internal static ArgumentException LapGap_InvalidCompareToArgument(object obj) {

            return new ArgumentException(Format(Resource.LapGap_InvalidCompareToArgument,
                typeof(LapGap).FullName, obj.GetType().FullName), "obj");
        }

        internal static FormatException LiveDecryptorFactory_UnableToParseSeed(string s) {

            return new FormatException(Format(Resource.LiveDecryptorFactory_UnableToParseSeed, s));
        }

        internal static CredentialsRejectedException LiveDecryptorFactory_CredentialsRejected() {

            return new CredentialsRejectedException();
        }

        internal static IOException RecordedMessageReader_DeserializationFailed(SerializationException exc) {

            // TODO maybe we should provide a custom exception message?
            return new IOException(exc.Message, exc);
        }

        internal static SerializationException PropertyDescriptor_PropertyIsNotDecorated(PropertyInfo property) {

            return new SerializationException(Format(
                "The '{0}' type cannot be serialized as the '{1}' property is not decorated with the '{2}' attribute.",
                property.DeclaringType, property.Name, typeof(PropertyIdAttribute)));
        }

        internal static SerializationException TypeDescriptor_TypeIsNotDecorated(Type type) {

            return new SerializationException(Format(
                "The '{0}' type cannot be serialized as it is not decorated with the '{1}' attribute.",
                type, typeof(TypeIdAttribute)));
        }

        internal static InvalidOperationException PropertyDescriptorCollection_DuplicatePropertyDescriptor(PropertyDescriptor item) {

            return new InvalidOperationException(Format(
                "The '{0}' type already has a property with an identifier of '{1}'.",
                item.Info.DeclaringType, item.PropertyId));
        }

        internal static NotSupportedException PropertyDescriptorCollection_CollectionIsSealed() {

            return new NotSupportedException();
        }

        internal static SerializationException TypeDescriptor_NoDescriptorWithTypeId(int typeId) {

            return new SerializationException();
        }

        internal static SerializationException ObjectReader_UnexpectedEndOfStream() {

            return new SerializationException();
        }

        #endregion        
    }
}
