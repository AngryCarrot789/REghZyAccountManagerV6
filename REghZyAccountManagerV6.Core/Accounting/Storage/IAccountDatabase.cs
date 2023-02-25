using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REghZyAccountManagerV6.Core.Accounting.Storage {
    public interface IAccountDatabase {
        IEnumerable<AccountModel> ReadAccounts();
        
        void WriteAccounts(IEnumerable<AccountModel> models);

        bool DeleteUser(string accountName);
    }
}