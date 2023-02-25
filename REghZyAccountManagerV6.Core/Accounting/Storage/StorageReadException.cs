using System;
using System.Runtime.Serialization;

namespace REghZyAccountManagerV6.Core.Accounting.Storage {
    public class StorageReadException : StorageException {
        public StorageReadException() {
        }

        protected StorageReadException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        public StorageReadException(string message) : base(message) {
        }

        public StorageReadException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}