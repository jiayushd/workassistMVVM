using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;

namespace WorkAssistMVVM.Services
{
    interface ITeamCaseService
    {
        List<TaskDetail> GetAllTasks();
    }
}
