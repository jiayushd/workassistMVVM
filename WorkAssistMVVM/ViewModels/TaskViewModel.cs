using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace WorkAssistMVVM.ViewModels
{
    public class TaskViewModel:BindableBase
    {
        //我方文号
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
        //任务名称
        private string taskName;
        public string TaskName
        {
            get { return taskName; }
            set { SetProperty(ref taskName, value); RaisePropertyChanged(); }
        }
        //任务属性
        private string taskAttribute;
        public string TaskAttribute
        {
            get { return taskAttribute; }
            set { SetProperty(ref taskAttribute, value); RaisePropertyChanged(); }
        }
        //客户名称
        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { SetProperty(ref customerName, value); RaisePropertyChanged(); }
        }
        //案件名称
        private string caseName;
        public string CaseName
        {
            get { return caseName; }
            set { SetProperty(ref caseName, value); RaisePropertyChanged(); }
        }
        //代理人处理状态
        private string processStage;
        public string ProcessStage
        {
            get { return processStage; }
            set { SetProperty(ref processStage, value); RaisePropertyChanged(); }
        }
        //初稿期限
        private DateTime firstVersionDeadline;
        public DateTime FirstVersionDeadline
        {
            get { return firstVersionDeadline; }
            set { SetProperty(ref firstVersionDeadline, value); RaisePropertyChanged(); }
        }
        //剩余天数
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
        //权值
        private double weight;
        public double Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }
        //任务ID
        private string proc_id;
        public string Proc_id
        {
            get { return proc_id; }
            set { SetProperty(ref proc_id, value); }
        }
    }
}
