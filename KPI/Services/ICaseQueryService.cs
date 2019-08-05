using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPI.Models;

namespace KPI.Services
{
    interface ICaseQueryService
    {
        List<TaskInfo> GetFirstVersionList(List<string> names, DateTime startDate, DateTime endDate);

        List<TaskInfo> GetFirstVersionList(DateTime startDate, DateTime endDate);

        List<TaskInfo> GetDoneList(List<string> names, DateTime startDate, DateTime endDate);

        List<TaskInfo> GetDoneList(DateTime startDate, DateTime endDate);

        List<TaskInfo> GetGrantedList(List<string> names, DateTime startDate, DateTime endDate);

        List<TaskInfo> GetGrantedList(DateTime startDate, DateTime endDate);

        List<TaskInfo> GetRejectedList(List<string> names, DateTime startDate, DateTime endDate);

        List<TaskInfo> GetRejectedList(DateTime startDate, DateTime endDate);

        List<Bill> GetBill(Department department, string year, string month);
    }
}
