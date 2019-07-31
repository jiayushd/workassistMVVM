using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;

namespace WorkAssistMVVM.Services
{
    public interface IKpiService
    {
        List<KPIinfo> GetKPIs(List<string> years, List<string> months);
    }
}
