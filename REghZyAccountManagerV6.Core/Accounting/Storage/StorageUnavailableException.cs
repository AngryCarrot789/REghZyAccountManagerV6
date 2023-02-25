using System;
using System.Runtime.Serialization;

namespace REghZyAccountManagerV6.Core.Accounting.Storage {
    public class StorageUnavailableException : StorageException {
        public StorageUnavailableException() {
        }

        protected StorageUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        public StorageUnavailableException(string message) : base(message) {
        }

        public StorageUnavailableException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}