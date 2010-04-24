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
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Authentication;

using AK.F1.Timing.Live;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing
{
    /// <summary>
    /// Library guard. This class is <see langword="static"/>.
    /// </summary>
    internal static class Guard
    {
        #region Validation.

        [DebuggerStepThrough]
#if DEBUG
        internal static void NotNull<T>(T instance, string paramName) where T : class {
#else
        internal static void NotNull(object instance, string paramName) {
#endif
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

        private static string Format(string format, params object[] args) {

            return string.Format(Resource.Culture, format, args);
        }

        #endregion

        #region Exception Factory Methods.

        internal static DirectoryNotFoundException DirectoryNotFound(string path, string paramName) {

            return new DirectoryNotFoundException(path);
        }

        internal static EndOfStreamException LiveMessageReader_UnexpectedEndOfStream() {

            return new EndOfStreamException(Resource.LiveMessageReader_UnexpectedEndOfStream);
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

        private static SerializationException LiveMessageReader_UnsupportedMessage(LiveMessageHeader header, string classification) {

            return new SerializationException(Format(Resource.LiveMessageReader_UnsupportedMessage, header, classification));
        }

        internal static SerializationException LiveMessageReader_UnsupportedSystemMessage(LiveMessageHeader header) {

            return LiveMessageReader_UnsupportedMessage(header, Resource.LiveMessageReader_MessageClassification_System);
        }

        internal static SerializationException LiveMessageReader_UnsupportedDriverMessage(LiveMessageHeader header) {

            return LiveMessageReader_UnsupportedMessage(header, Resource.LiveMessageReader_MessageClassification_Driver);
        }

        internal static SerializationException LiveMessageReader_UnsupportedWeatherMessage(LiveMessageHeader header) {

            return LiveMessageReader_UnsupportedMessage(header, Resource.LiveMessageReader_MessageClassification_Weather);
        }

        internal static SerializationException LiveData_UnableToConvertToSessionStatus(string s) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToSessionStatus, s));
        }

        internal static SerializationException LiveData_UnableToConvertToDriverStatus(int value) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToDriverStatus, value));
        }

        internal static SerializationException LiveData_UnableToConvertToSessionType(int value) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToSessionType, value));
        }

        internal static SerializationException LiveData_UnableToConvertToPostedTimeType(int value) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToPostedTimeType, value));
        }

        internal static SerializationException LiveData_UnableToConvertToGridColumn(byte column, SessionType currentSessionType) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToGridColumn, column, currentSessionType));
        }

        internal static SerializationException LiveData_UnableToConvertToGridColumnColour(byte colour) {

            return new SerializationException(Format(Resource.LiveData_UnableToConvertToGridColumnColour, colour));
        }

        internal static SerializationException LiveData_UnableToParseTime(string value) {

            return new SerializationException(Format(Resource.LiveData_UnableToParseTime, value));
        }

        internal static SerializationException LiveData_UnableToParseInt32(string s) {

            return new SerializationException(Format(Resource.LiveData_UnableToParseInt32, s));
        }

        internal static SerializationException LiveData_UnableToParseDouble(string s) {

            return new SerializationException(Format(Resource.LiveData_UnableToParseDouble, s));
        }

        internal static SerializationException LiveMessageReader_UnexpectedFirstMessage(Message message) {

            return new SerializationException(Format(Resource.LiveMessageReader_UnexpectedFirstMessage, message));
        }

        internal static SerializationException MessageReader_InvalidMessage() {

            return new SerializationException(Resource.MessageReader_InvalidMessage);
        }

        internal static ArgumentException TimeGap_InvalidCompareToArgument(object obj) {

            return new ArgumentException(Format(Resource.TimeGap_InvalidCompareToArgument, typeof(TimeGap).FullName, obj.GetType().FullName), "obj");
        }

        internal static ArgumentException LapGap_InvalidCompareToArgument(object obj) {

            return new ArgumentException(Format(Resource.LapGap_InvalidCompareToArgument, typeof(LapGap).FullName, obj.GetType().FullName), "obj");
        }

        internal static ArgumentException PostedTime_InvalidCompareToArgument(object obj) {

            return new ArgumentException(Format(Resource.PostedTime_InvalidCompareToArgument, typeof(PostedTime).FullName, obj.GetType().FullName), "obj");
        }

        internal static SerializationException LiveDecrypterFactory_UnableToParseSeed(string s) {

            return new SerializationException(Format(Resource.LiveDecrypterFactory_UnableToParseSeed, s));
        }

        internal static AuthenticationException LiveDecrypterFactory_CredentialsRejected() {

            return new AuthenticationException(Resource.LiveDecrypterFactory_CredentialsRejected);
        }

        internal static AuthenticationException LiveAuthenticationService_CredentialsRejected() {

            return new AuthenticationException(Resource.LiveAuthenticationService_CredentialsRejected);
        }

        internal static SerializationException PropertyDescriptor_PropertyIsNotDecorated(PropertyInfo property) {

            return new SerializationException(Format(Resource.PropertyDescriptor_PropertyIsNotDecorated, property.DeclaringType, property.Name, typeof(PropertyIdAttribute)));
        }

        internal static SerializationException PropertyDescriptor_PropertyHaveGetAndSetMethod(PropertyInfo property) {

            return new SerializationException(Format(Resource.PropertyDescriptor_PropertyHaveGetAndSetMethod, property.DeclaringType, property.Name));
        }

        internal static SerializationException TypeDescriptor_TypeIsNotDecorated(Type type) {

            return new SerializationException(Format(Resource.TypeDescriptor_TypeIsNotDecorated, type, typeof(TypeIdAttribute)));
        }

        internal static SerializationException TypeDescriptor_DuplicateProperty(PropertyDescriptor item) {

            return new SerializationException(Format(Resource.TypeDescriptor_DuplicateProperty, item.Property.DeclaringType, item.PropertyId));
        }

        internal static SerializationException TypeDescriptor_NoDescriptorWithTypeId(int typeId) {

            return new SerializationException(Format(Resource.TypeDescriptor_NoDescriptorWithTypeId, typeId));
        }

        internal static SerializationException TypeDescriptor_DuplicateTypeId(TypeDescriptor existingDescriptor, Type duplicateType) {

            return new SerializationException(Format(Resource.TypeDescriptor_DuplicateTypeId, existingDescriptor.TypeId, existingDescriptor.Type, duplicateType));
        }

        internal static SerializationException DecoratedObjectReader_UnexpectedEndOfStream(EndOfStreamException exc) {

            return new SerializationException(Resource.DecoratedObjectReader_UnexpectedEndOfStream, exc);
        }

        internal static SerializationException DecoratedObjectReader_InvalidObjectTypeCode(ObjectTypeCode typeCode) {

            return new SerializationException(Format(Resource.DecoratedObjectReader_InvalidObjectTypeCode, (int)typeCode));
        }

        internal static SerializationException DecoratedObjectReader_PropertyMissing(byte propertyId, TypeDescriptor descriptor) {

            return new SerializationException(Format(Resource.DecoratedObjectReader_PropertyMissing, propertyId, descriptor.Type));
        }

        internal static SerializationException DecoratedObjectWriter_RootGraphMustBeAnObject(object root) {

            return new SerializationException(Format(Resource.DecoratedObjectWriter_RootGraphMustBeAnObject, root.GetType()));
        }

        internal static SerializationException DecoratedObjectWriter_CirularReferencesAreNotSupported(object graph) {

            return new SerializationException(Format(Resource.DecoratedObjectWriter_CirularReferencesAreNotSupported, graph, graph.GetType()));
        }

        internal static IOException LiveMessageStreamEndpoint_FailedToResolveStreamHost(string hostname) {

            return new IOException(Format(Resource.LiveMessageStreamEndpoint_FailedToResolveStreamHost, hostname));
        }

        internal static IOException LiveMessageStreamEndpoint_FailedToResolveStreamHost(string hostname, SocketException exc) {

            return new IOException(Format(Resource.LiveMessageStreamEndpoint_FailedToResolveStreamHost, hostname), exc);
        }

        internal static IOException LiveMessageStreamEndpoint_FailedToOpenStream(SocketException exc) {

            return new IOException(Format(Resource.LiveMessageStreamEndpoint_FailedToOpenStream, exc.Message), exc);
        }

        internal static IOException LiveMessageStreamEndpoint_FailedToOpenKeyframe(IOException exc) {

            return new IOException(Format(Resource.LiveMessageStreamEndpoint_FailedToOpenKeyframe, exc.Message), exc);
        }

        internal static IOException LiveAuthenticationService_FailedToFetchAuthToken(IOException exc) {

            return new IOException(Format(Resource.LiveAuthenticationService_FailedToFetchAuthToken, exc.Message), exc);
        }

        internal static IOException LiveDecrypterFactory_FailedToFetchSessionSeed(IOException exc) {

            return new IOException(Format(Resource.LiveDecrypterFactory_FailedToFetchSessionSeed, exc.Message), exc);
        }

        #endregion
    }
}
