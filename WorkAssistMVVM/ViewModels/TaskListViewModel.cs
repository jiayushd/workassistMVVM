using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;


namespace WorkAssistMVVM.ViewModels
{
    public class TaskListViewModel : BindableBase
    {
        private string attorneySeries;
        public string AttorneySeries
        {
            get { return attorneySeries; }
            set
            {
                SetProperty(ref attorneySeries, value);
                RaisePropertyChanged();
            }
        }

        private string taskName;
        public string TaskName
        {
            get { return taskName; }
            set { SetProperty(ref taskName, value); RaisePropertyChanged(); }
        }

        private string taskAttribute;
        public string TaskAttribute
        {
            get { return taskAttribute; }
            set { SetProperty(ref taskAttribute, value); RaisePropertyChanged(); }
        }

        private DateTime firstVersionDeadline;
        public DateTime FirstVersionDeadline
        {
            get { return firstVersionDeadline; }
            set { SetProperty(ref firstVersionDeadline, value); RaisePropertyChanged(); }
        }

        private int daysLeft;
        public int DaysLeft
        {
            get { return daysLeft; }
            set
            {
                DaysLeft = (DateTime.Now.Date - firstVersionDeadline).Days;
                SetProperty(ref daysLeft, value);
                RaisePropertyChanged();
            }
        }
    }
}
