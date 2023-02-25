using System;
using System.Runtime.Serialization;

namespace REghZyAccountManagerV6.Core.Accounting.Storage {
    public class StorageWriteException : StorageException {
        public StorageWriteException() {
        }

        protected StorageWriteException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        public StorageWriteException(string message) : base(message) {
        }

        public StorageWriteException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}