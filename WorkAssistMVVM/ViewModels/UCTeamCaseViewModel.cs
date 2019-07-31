using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using WorkAssistMVVM.Models;
using WorkAssistMVVM.Services;
using Prism.Regions;
using System.Windows;

namespace WorkAssistMVVM.ViewModels
{
    class UCTeamCaseViewModel:BindableBase, INavigationAware
    {
        private string _title = "团队案件";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public string Cookie { get; set; }

        #region 属性
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); RaisePropertyChanged(); }
        }

        private bool allCheck;
        public bool AllCheck
        {
            get { return allCheck; }
            set { SetProperty(ref allCheck, value); }
        }

        private bool radiobuttonEnable;
        public bool RadiobuttonEnable
        {
            get { return radiobuttonEnable; }
            set { SetProperty(ref radiobuttonEnable, value); }
        }

        private string currentSelectedMember;
        public string SelectedMember
        {
            get { return currentSelectedMember; }
            set { SetProperty(ref currentSelectedMember, value); }
        }

        //任务类型
        private string ctrl_proc_code;
        public string Ctrl_proc_code
        {
            get { return ctrl_proc_code; }
            set { SetProperty(ref ctrl_proc_code, value); }
        }

        //处理状态
        private string proc_status;
        public string Proc_status
        {
            get { return proc_status; }
            set { SetProperty(ref proc_status, value); }
        }

        private List<string> zones;
        public List<string> Zones
        {
            get { return zones; }
            set { SetProperty(ref zones, value); RaisePropertyChanged(); }
        }

        private ObservableCollection<TaskViewModel> tasks;
        public ObservableCollection<TaskViewModel> Tasks
        {
            get { return tasks; }
            set { SetProperty(ref tasks, value); }
        }

        private List<TeamMemberInfo> teamMembers;
        public List<TeamMemberInfo> TeamMembers
        {
            get { return teamMembers; }
            set { SetProperty(ref teamMembers, value); }
        }

        private ObservableCollection<AttorneySummarizeViewModel> attorneySummarizes;
        public ObservableCollection<AttorneySummarizeViewModel> AttorneySummarizes
        {
            get { return attorneySummarizes; }
            set { SetProperty(ref attorneySummarizes, value); }
        }

        private bool allNewAppSelected;
        public bool AllNewAppSelected
        {
            get { return allNewAppSelected; }
            set { SetProperty(ref allNewAppSelected, value); }
        }

        private bool candoNewAppSelected;
        public bool CandoNewAppSelected
        {
            get { return candoNewAppSelected; }
            set { SetProperty(ref candoNewAppSelected, value); }
        }

        private bool allOASelected;
        public bool AllOASelected
        {
            get { return allOASelected; }
            set { SetProperty(ref allOASelected, value); }
        }

        private bool allOtherSelected;
        public bool AllOtherSelected
        {
            get { return allOtherSelected; }
            set { SetProperty(ref allOtherSelected, value); }
        }
        #endregion

        //选择区域
        public DelegateCommand<object[]> CheckCommand { get; private set; }
        private void OnCheck(object[] parameter)
        {
            var values = parameter;
            string zone = (string)values[0];
            bool check = (bool)values[1];
            
        }
        //选择/取消所有区域
        public DelegateCommand<object[]> CheckAllCommand { get; private set; }
        private void CheckAll(object[] parameter)
        {
            var values = parameter;
            //string zone = (string)values[0];
            bool checkall = (bool)values[1];
            if (checkall)
            {
                IsChecked = true;
                Zones = new List<string> { "北京", "上海", "广州", "深圳", "长沙", "苏州", "西安", "佛山", "杭州" };
            }
        }

        public DelegateCommand<object[]> SelectionCommand { get; private set; }
        private void OnItemSelected(object[] selectedItems)
        {
            if (selectedItems != null && selectedItems.Count() > 0)
            {
                RadiobuttonEnable = true;
                AttorneySummarizeViewModel asvm = (AttorneySummarizeViewModel)selectedItems.FirstOrDefault();
                string name = asvm.Name;
                HttpDataService hds = new HttpDataService(Cookie);
                SelectedMember = name;
                List<CaseInfo> caseInfos = hds.GetUndone(Ctrl_proc_code, Proc_status, SelectedMember);
                if (Tasks != null)
                {
                    Tasks.Clear();
                }
                foreach (CaseInfo item in caseInfos)
                {
                    TaskViewModel task = new TaskViewModel();
                    task.AttorneySeries = item.AttorneySeries;
                    task.TaskName = item.taskInfos[0].TaskName;
                    task.CustomerName = item.ClientName;
                    task.TaskAttribute = item.taskInfos[0].TaskAttribute;
                    task.CaseName = item.CasedocumentName;
                    task.ProcessStage = item.taskInfos[0].ProcessStage;
                    task.FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal;

                    Tasks.Add(task);
                }
            }

        }

        public DelegateCommand<object[]> GetAllNewAppCommand { get; private set; }
        private void GetAllNewApp(object[] parameter)
        {
            HttpDataService hds = new HttpDataService(Cookie);
            Ctrl_proc_code = "ap";
            Proc_status = "";
            List<CaseInfo> cases = hds.GetUndone(Ctrl_proc_code, Proc_status, SelectedMember);//, c.taskInfos[0].Weight
            Tasks.Clear();
            foreach (var item in cases)
            {
                TaskViewModel task = new TaskViewModel();
                task.AttorneySeries = item.AttorneySeries;
                task.TaskName = item.taskInfos[0].TaskName;
                task.CustomerName = item.ClientName;
                task.TaskAttribute = item.taskInfos[0].TaskAttribute;
                task.CaseName = item.CasedocumentName;
                task.ProcessStage = item.taskInfos[0].ProcessStage;
                task.FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal;

                Tasks.Add(task);
            }
        }

        public DelegateCommand<object[]> GetCandoNewAppCommand { get; private set; }
        private void GetCandoNewApp(object[] parameter)
        {
            HttpDataService hds = new HttpDataService(Cookie);
            Ctrl_proc_code = "ap";
            Proc_status = "KCL";
            List<CaseInfo> cases = hds.GetUndone(Ctrl_proc_code, Proc_status, SelectedMember);//, c.taskInfos[0].Weight
            Tasks.Clear();
            foreach (var item in cases)
            {
                TaskViewModel task = new TaskViewModel();
                task.AttorneySeries = item.AttorneySeries;
                task.TaskName = item.taskInfos[0].TaskName;
                task.CustomerName = item.ClientName;
                task.TaskAttribute = item.taskInfos[0].TaskAttribute;
                task.CaseName = item.CasedocumentName;
                task.ProcessStage = item.taskInfos[0].ProcessStage;
                task.FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal;

                Tasks.Add(task);
            }
        }

        public DelegateCommand<object[]> GetAllOACommand { get; private set; }
        private void GetAllOA(object[] parameter)
        {
            HttpDataService hds = new HttpDataService(Cookie);
            Ctrl_proc_code = "oa";
            Proc_status = "";
            List<CaseInfo> cases = hds.GetUndone(Ctrl_proc_code, Proc_status, SelectedMember);//, c.taskInfos[0].Weight
            Tasks.Clear();
            foreach (var item in cases)
            {
                TaskViewModel task = new TaskViewModel();
                task.AttorneySeries = item.AttorneySeries;
                task.TaskName = item.taskInfos[0].TaskName;
                task.CustomerName = item.ClientName;
                task.TaskAttribute = item.taskInfos[0].TaskAttribute;
                task.CaseName = item.CasedocumentName;
                task.ProcessStage = item.taskInfos[0].ProcessStage;
                task.FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal;

                Tasks.Add(task);
            }
        }

        public DelegateCommand<object[]> GetAllOtherCommand { get; private set; }
        private void GetAllOther(object[] parameter)
        {
            HttpDataService hds = new HttpDataService(Cookie);
            Ctrl_proc_code = "other";
            Proc_status = "";
            List<CaseInfo> cases = hds.GetUndone(Ctrl_proc_code, Proc_status, SelectedMember);//, c.taskInfos[0].Weight
            Tasks.Clear();
            foreach (var item in cases)
            {
                TaskViewModel task = new TaskViewModel();
                task.AttorneySeries = item.AttorneySeries;
                task.TaskName = item.taskInfos[0].TaskName;
                task.CustomerName = item.ClientName;
                task.TaskAttribute = item.taskInfos[0].TaskAttribute;
                task.CaseName = item.CasedocumentName;
                task.ProcessStage = item.taskInfos[0].ProcessStage;
                task.FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal;

                Tasks.Add(task);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Cookie = navigationContext.Parameters["cookie"] as string;
            Ctrl_proc_code = "ap";
            Proc_status = "KCL";
            CandoNewAppSelected = true;
            RadiobuttonEnable = false;
            if (Tasks == null)
            {
                Tasks = new ObservableCollection<TaskViewModel>();
            }
            else
            {
                Tasks.Clear();
            }
            currentSelectedMember = "";
            HttpDataService hds = new HttpDataService(Cookie);
            List<CaseInfo> cases = hds.GetCandoUndone();//, c.taskInfos[0].Weight
            var summarize = cases.GroupBy(c => new { c.taskInfos[0].Attorney, c.taskInfos[0].Department }).
                Select(a => new {
                    attorney = a.Key.Attorney,
                    zone = a.Key.Department,
                    summarize = a.Sum(b => b.taskInfos[0].Weight)
                    }).ToList();
            AttorneySummarizes = new ObservableCollection<AttorneySummarizeViewModel>();
            foreach (var item in summarize)
            {
                AttorneySummarizeViewModel attorneySummarize = new AttorneySummarizeViewModel();
                attorneySummarize.Name = item.attorney;
                attorneySummarize.Zone = item.zone;
                attorneySummarize.Weight = item.summarize;
                if (attorneySummarize.Zone !="其他")
                {
                    AttorneySummarizes.Add(attorneySummarize);
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }


        public UCTeamCaseViewModel()
        {
            Ctrl_proc_code = "ap";
            Ctrl_proc_code = "KCL";
            CandoNewAppSelected = true;
            SelectionCommand = new DelegateCommand<object[]>(OnItemSelected);
            GetCandoNewAppCommand = new DelegateCommand<object[]>(GetCandoNewApp);
            GetAllNewAppCommand = new DelegateCommand<object[]>(GetAllNewApp);
            GetAllOACommand = new DelegateCommand<object[]>(GetAllOA);
            GetAllOtherCommand = new DelegateCommand<object[]>(GetAllOther);
        }
    }
}
