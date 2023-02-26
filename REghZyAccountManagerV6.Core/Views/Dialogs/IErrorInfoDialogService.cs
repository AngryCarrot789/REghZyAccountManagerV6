using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace REghZyAccountManagerV6.Core.Views.Dialogs {
    public interface IErrorInfoDialogService {
        Task ShowDialogAsync(IEnumerable<Tuple<string, string>> errors);
    }
}