using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;

namespace WorkAssistMVVM.Services
{
    public interface ICaseService
    {
        List<CaseInfo> GetAllUndone();
        List<CaseInfo> GetNewApp(string name);
        List<CaseInfo> GetOA(string name);
        List<CaseInfo> GetOther(string name);

        List<CaseInfo> SearchBySingleString(string searchString);

    }
}
