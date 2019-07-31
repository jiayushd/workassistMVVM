using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkAssistMVVM.Models
{
    public class TaskDetail
    {
        public CaseInfo Caseinfo { get; set; }
        public TaskInfo Taskinfo { get; set; }

        public TaskDetail()
        {
            Caseinfo = new CaseInfo();
            Taskinfo = new TaskInfo();
        }
    }
}
