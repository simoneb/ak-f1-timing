// Copyright 2011 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file exceptionept in compliance with the License.
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
using System.Security.Authentication;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Proxy.Messages
{
    /// <summary>
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(48186320)]
    internal class ServerExceptionMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerLoginMessage"/> class and
        /// specifies the <paramref name="exception"/> thrown by the server.
        /// </summary>
        /// <param name="exception">The exception thrown by the server.</param>
        /// <exceptioneption cref="System.ArgumentNullException">
        /// Thrown when <paramref name="exception"/> is <see langword="null"/>.
        /// </exceptioneption>        
        public ServerExceptionMessage(IOException exception)
            : this(exception, ServerExceptionType.IOException) { }

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerLoginMessage"/> class and
        /// specifies the <paramref name="exception"/> thrown by the server.
        /// </summary>
        /// <param name="exception">The exception thrown by the server.</param>
        /// <exceptioneption cref="System.ArgumentNullException">
        /// Thrown when <paramref name="exception"/> is <see langword="null"/>.
        /// </exceptioneption>   
        public ServerExceptionMessage(SerializationException exception)
            : this(exception, ServerExceptionType.SerializationException) { }

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerLoginMessage"/> class and
        /// specifies the <paramref name="exception"/> thrown by the server.
        /// </summary>
        /// <param name="exception">The exception thrown by the server.</param>
        /// <exceptioneption cref="System.ArgumentNullException">
        /// Thrown when <paramref name="exception"/> is <see langword="null"/>.
        /// </exceptioneption>   
        public ServerExceptionMessage(AuthenticationException exception)
            : this(exception, ServerExceptionType.AuthenticationException) { }

        /// <summary>
        /// This method is a no-op.
        /// </summary>
        /// <param name="visitor">The message visitor.</param>
        public override void Accept(IMessageVisitor visitor) { }

        /// <summary>
        /// Throw an exception of the same type and message as thrown by the server.
        /// </summary>
        public void ThrowException()
        {
            switch(Type)
            {
                case ServerExceptionType.IOException:
                    throw new IOException(Message);
                case ServerExceptionType.SerializationException:
                    throw new SerializationException(Message);
                case ServerExceptionType.AuthenticationException:
                    throw new AuthenticationException(Message);
                default:
                    Guard.Fail("Invalid ServerExceptionType: " + Type);
                    break;
            }
        }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        [PropertyId(0)]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the <see cref="ServerExceptionType"/> of the exception thrown.
        /// </summary>
        [PropertyId(1)]
        public ServerExceptionType Type { get; private set; }

        #endregion

        #region Private Impl.

        private ServerExceptionMessage(Exception exception, ServerExceptionType type)
        {
            Guard.NotNull(exception, "exception");

            Message = exception.Message;
            Type = type;
        }

        #endregion
    }
}