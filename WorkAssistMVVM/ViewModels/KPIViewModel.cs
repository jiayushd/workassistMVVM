using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows;
using WorkAssistMVVM.Models;
using WorkAssistMVVM.Services;
using System.Collections.ObjectModel;
using Prism.Regions;
using System.IO;

namespace WorkAssistMVVM.ViewModels
{
    class KPIViewModel:BindableBase
    {
        private readonly IRegionManager _regionManager;

        private string _title = "KPI";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //选择区域
        public DelegateCommand<object[]> CheckCommand { get; private set; }
        private void OnCheck(object[] parameter)
        {
            var values = parameter;
            string zone = (string)values[0];
            bool check = (bool)values[1];
            DoneTotalForExam = 0;
            FirstVersionTotal = 0;
            if (check)
            {
                Zones.Add(zone);
                if (SelectedKpiInfos == null)
                {
                    SelectedKpiInfos = new ObservableCollection<KpiInfoViewModel>();
                }
                foreach (KPIinfo item in KPIinfos.FindAll(x => x.Zone.Contains(zone)))
                {
                    KpiInfoViewModel kpi = new KpiInfoViewModel();
                    kpi.KPIInfo = item;
                    kpi.IsSelected = true;
                    SelectedKpiInfos.Add(kpi);
                    DoneTotalForExam += item.DonePoint;
                    FirstVersionTotal += item.FirstVirsionPoint;
                }
            }
            else
            {
                Zones.Remove(zone);
                List<KpiInfoViewModel> kpis = new List<KpiInfoViewModel>(SelectedKpiInfos);
                kpis.RemoveAll(x => x.KPIInfo.Zone.Contains(zone));
                SelectedKpiInfos = new ObservableCollection<KpiInfoViewModel>(kpis);
                foreach (KpiInfoViewModel item in SelectedKpiInfos)
                {
                    DoneTotalForExam += item.KPIInfo.DonePoint;
                    FirstVersionTotal += item.KPIInfo.FirstVirsionPoint;
                }
            }
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
                Zones = new List<string> { "北京","上海", "广州", "深圳", "长沙", "苏州", "西安", "佛山", "杭州" };
                if (SelectedKpiInfos == null)
                {
                    SelectedKpiInfos = new ObservableCollection<KpiInfoViewModel>();
                }
                foreach (string zone in Zones)
                {
                    foreach (KPIinfo item in KPIinfos.FindAll(x => x.Zone.Contains(zone)))
                    {
                        KpiInfoViewModel kpi = new KpiInfoViewModel();
                        kpi.KPIInfo = item;
                        kpi.IsSelected = true;
                        SelectedKpiInfos.Add(kpi);
                    }
                }
            }
            else
            {
                IsChecked = false;
                Zones = new List<string>();
                SelectedKpiInfos = null;
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value);RaisePropertyChanged(); }
        }

        private bool allCheck;
        public bool AllCheck
        {
            get { return allCheck; }
            set { SetProperty(ref allCheck, value); }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value); }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(ref endDate, value); }
        }

        private List<string> zones; 
        public List<string> Zones
        {
            get { return zones; }
            set { SetProperty(ref zones, value); RaisePropertyChanged(); }
        }

        private List<KPIinfo> kPIinfos;
        public List<KPIinfo> KPIinfos
        {
            get { return kPIinfos; }
            set { SetProperty(ref kPIinfos, value); }
        }

        private ObservableCollection<KpiInfoViewModel> selectedKpiInfos;
        public ObservableCollection<KpiInfoViewModel> SelectedKpiInfos
        {
            get { return selectedKpiInfos; }
            set { SetProperty(ref selectedKpiInfos, value); }
        }

        public DelegateCommand<object[]> SiftCommand { get; private set; }
        private void Sift(object[] parameter)
        {
            try
            {
                List<string> names = new List<string>();
                List<string> years = new List<string>();
                List<string> months = new List<string>();
                string yearStart = StartDate.Year.ToString();
                string yearEnd = EndDate.Year.ToString();
                string monthStart = StartDate.Month.ToString();
                string monthEnd = EndDate.Month.ToString();

                if (yearStart.Equals(yearEnd))
                {
                    years.Add(yearStart);
                    if (StartDate <= EndDate)
                    {
                        for (int i = StartDate.Month; i <= EndDate.Month; i++)
                        {
                            if (i < 10) months.Add("0" + i);
                            else
                                months.Add(i.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("起始日期应小于结束日期!", "出错", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("不支持跨年查询!", "出错", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
                KpiDataServiceThroughSQLite kds = new KpiDataServiceThroughSQLite();
                KPIinfos = new List<KPIinfo>();
                KPIinfos = kds.GetKPIs(years, months);
                List<KpiInfoViewModel> kpis = new List<KpiInfoViewModel>();
                DoneTotalForExam = 0;
                FirstVersionTotal = 0;
                foreach (KPIinfo item in KPIinfos)
                {
                    KpiInfoViewModel kpi = new KpiInfoViewModel();
                    kpi.KPIInfo = item;
                    kpi.IsSelected = true;
                    kpis.Add(kpi);
                    DoneTotalForExam += item.DonePoint;
                    FirstVersionTotal += item.FirstVirsionPoint;
                }
                SelectedKpiInfos = new ObservableCollection<KpiInfoViewModel>(kpis);                       

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        public DelegateCommand<object[]> RefreshCommand { get; private set; }
        private void Refresh(object[] parameter)
        {

            MessageBox.Show("刷新");
        }

        public DelegateCommand<object[]> PlanWindowCommand { get; private set; }
        private void OpenPlanWindow(object[] parameter)
        {

            MessageBox.Show("打开计划表");
        }

        public DelegateCommand<object[]> ScoreWindowCommand { get; private set; }
        private void OpenScoreWindow(object[] parameter)
        {

            MessageBox.Show("打开打分表");
        }

        public DelegateCommand<object[]> GeneratePlanTableCommand { get; private set; }
        private void GeneratePlanTable(object[] parameter)
        {
            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string file = exepath + "MyData\\绩效考核表-模板.xls";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file);
            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
            Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["绩效指标考核表"];

            foreach (KpiInfoViewModel kpi in SelectedKpiInfos)
            {
                Generate_KPI_Target(kpi, wsh);
            }
            
            wbks.Close();
            MessageBox.Show("生成完毕！", "", MessageBoxButton.OKCancel, MessageBoxImage.Information);

        }
        //生成绩效目标表
        private void Generate_KPI_Target(KpiInfoViewModel kpi, Microsoft.Office.Interop.Excel.Worksheet wsh)
        {
            try
            {
                wsh.Cells[2, 2].value = kpi.KPIInfo.Year + "年" + kpi.KPIInfo.Period.Substring(0, 2) + "绩效指标考核表——国内专利事业部+" + kpi.KPIInfo.Zone + "+" + kpi.KPIInfo.Name;
                wsh.Cells[3, 2].value = "考核周期：" + kpi.KPIInfo.Period;
                wsh.Cells[6, 7].value = kpi.KPIInfo.DonePoint_Target;
                wsh.Cells[7, 7].value = kpi.KPIInfo.FirstVirsionPoint_Target;
                wsh.Cells[8, 7].value = kpi.KPIInfo.PatentDegree_Target;
                wsh.Cells[9, 7].value = kpi.KPIInfo.InTimePortion_Target;
                wsh.Cells[6, 8].value = 0.3;
                wsh.Cells[7, 8].value = 0.25;
                wsh.Cells[8, 8].value = 0.1;
                wsh.Cells[9, 8].value = 0.2;
                wsh.Cells[15, 4].value = kpi.KPIInfo.Name;
                wsh.Cells[16, 4].value = kpi.KPIInfo.Examiner;
                wsh.Cells[15, 7].value = kpi.KPIInfo.Position;
                wsh.Cells[16, 7].value = kpi.KPIInfo.Examiner_Positon;

                //华进员工绩效考核表——国内专利事业部+广州电子部+蔡抒枫
                string file = "C:\\WORK\\绩效考核\\华进员工绩效考核表——国内专利事业部+" + kpi.KPIInfo.Zone + "+" + kpi.KPIInfo.Name + ".xlsx";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                wsh.SaveAs(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        //生成绩效考核表
        private void Generate_KPI_Done(KpiInfoViewModel kpi, Microsoft.Office.Interop.Excel.Worksheet wsh)
        {
            try
            {
                wsh.Cells[2, 2].value = kpi.KPIInfo.Year + "年" + kpi.KPIInfo.Period.Substring(0, 2) + "绩效指标考核表——国内专利事业部+" + kpi.KPIInfo.Zone + "+" + kpi.KPIInfo.Name;
                wsh.Cells[3, 2].value = "考核周期：" + kpi.KPIInfo.Period;
                wsh.Cells[6, 7].value = kpi.KPIInfo.DonePoint_Target;
                wsh.Cells[7, 7].value = kpi.KPIInfo.FirstVirsionPoint_Target;
                wsh.Cells[8, 7].value = kpi.KPIInfo.PatentDegree_Target;
                wsh.Cells[9, 7].value = kpi.KPIInfo.InTimePortion_Target;
                wsh.Cells[6, 11].value = kpi.KPIInfo.DonePoint;
                wsh.Cells[7, 11].value = kpi.KPIInfo.FirstVirsionPoint;
                wsh.Cells[8, 11].value = kpi.KPIInfo.PatentDegree;
                wsh.Cells[9, 11].value = kpi.KPIInfo.InTimePortion;
                wsh.Cells[6, 8].value = 0.3;
                wsh.Cells[7, 8].value = 0.25;
                wsh.Cells[8, 8].value = 0.1;
                wsh.Cells[9, 8].value = 0.2;
                wsh.Cells[15, 4].value = kpi.KPIInfo.Name;
                wsh.Cells[16, 4].value = kpi.KPIInfo.Examiner;
                wsh.Cells[15, 7].value = kpi.KPIInfo.Position;
                wsh.Cells[16, 7].value = kpi.KPIInfo.Examiner_Positon;

                wsh.Cells[6, 12].value = Math.Min(kpi.KPIInfo.DonePoint * 100 / kpi.KPIInfo.DonePoint_Target, 100);
                wsh.Cells[7, 12].value = Math.Min(kpi.KPIInfo.FirstVirsionPoint * 100 / kpi.KPIInfo.FirstVirsionPoint_Target, 100);
                wsh.Cells[8, 12].value = Math.Min(kpi.KPIInfo.PatentDegree * 100 / kpi.KPIInfo.PatentDegree_Target, 100);
                wsh.Cells[9, 12].value = Math.Min(kpi.KPIInfo.InTimePortion * 100 / kpi.KPIInfo.InTimePortion_Target, 100);
                wsh.Cells[10, 12].value = kpi.KPIInfo.Score_Cowork;
                wsh.Cells[11, 12].value = kpi.KPIInfo.Score_Passion;
                wsh.Cells[12, 12].value = kpi.KPIInfo.Score_Selfdrive;

                wsh.Cells[17, 4].value = kpi.KPIInfo.Comment;

                //华进员工绩效考核表——国内专利事业部+广州电子部+蔡抒枫
                string file = "C:\\WORK\\绩效考核\\华进员工绩效考核表——国内专利事业部+" + kpi.KPIInfo.Zone + "+" + kpi.KPIInfo.Name + "-完成情况.xlsx";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                wsh.SaveAs(file);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        public DelegateCommand<object[]> GenerateDoneTableCommand { get; private set; }
        private void GenerateDoneTable(object[] parameter)
        {
            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string file = exepath + "MyData\\绩效考核表-模板.xls";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file);
            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
            Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["绩效指标考核表"];

            foreach (KpiInfoViewModel kpi in SelectedKpiInfos)
            {
                Generate_KPI_Done(kpi, wsh);
            }
            
            wbks.Close();
            MessageBox.Show("生成完毕！", "", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }

        public DelegateCommand<object[]> ExportDoneTableCommand { get; private set; }
        private void ExportDoneTable(object[] parameter)
        {
            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string file = exepath + "MyData\\绩效汇总表-模板.xls";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file);
            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
            Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["sheet1"];

            int i = 0;
            foreach (KpiInfoViewModel kpi in SelectedKpiInfos)
            {
                wsh.Cells[i + 2, 1].value = kpi.KPIInfo.Period;
                wsh.Cells[i + 2, 2].value = kpi.KPIInfo.Zone;
                wsh.Cells[i + 2, 3].value = kpi.KPIInfo.Name;
                wsh.Cells[i + 2, 4].value = kpi.KPIInfo.DonePoint_Target;
                wsh.Cells[i + 2, 5].value = kpi.KPIInfo.FirstVirsionPoint_Target;
                wsh.Cells[i + 2, 6].value = kpi.KPIInfo.PatentDegree_Target;
                wsh.Cells[i + 2, 7].value = kpi.KPIInfo.InTimePortion_Target;
                wsh.Cells[i + 2, 8].value = kpi.KPIInfo.Score_Cowork;
                wsh.Cells[i + 2, 9].value = kpi.KPIInfo.Score_Passion;
                wsh.Cells[i + 2, 10].value = kpi.KPIInfo.Score_Selfdrive;
                wsh.Cells[i + 2, 11].value = kpi.KPIInfo.DonePoint;
                wsh.Cells[i + 2, 12].value = kpi.KPIInfo.PatentDegree;
                wsh.Cells[i + 2, 13].value = kpi.KPIInfo.InTimePortion;
                wsh.Cells[i + 2, 14].value = kpi.KPIInfo.FirstVirsionPoint;

                wsh.Cells[i + 2, 15].value = kpi.KPIInfo.Position;
                wsh.Cells[i + 2, 16].value = kpi.KPIInfo.Examiner;
                wsh.Cells[i + 2, 17].value = kpi.KPIInfo.Examiner_Positon;
                wsh.Cells[i + 2, 18].value = kpi.KPIInfo.Score;
                wsh.Cells[i + 2, 20].value = kpi.KPIInfo.Comment;
                i++;
            }

            string fileSummarize = "C:\\WORK\\绩效考核\\绩效考核汇总表.xlsx";
            if (File.Exists(fileSummarize))
            {
                File.Delete(fileSummarize);
            }

            wsh.SaveAs(fileSummarize);
            wbks.Close();
            MessageBox.Show("生成完毕！", "", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        }

        public DelegateCommand<object[]> SupervisorCommand { get; private set; }
        private void Supervisor(object[] parameter)
        {

            MessageBox.Show("打开主管绩效窗口");
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); RaisePropertyChanged(); }
        }

        private double doneTotalForExam;
        public double DoneTotalForExam
        {
            get { return doneTotalForExam; }
            set { SetProperty(ref doneTotalForExam, value); }
        }

        private double doneTotal;
        public double DoneTotal
        {
            get { return doneTotal; }
            set { SetProperty(ref doneTotal, value); }
        }

        private double firstVersionTotal;
        public double FirstVersionTotal
        {
            get { return firstVersionTotal; }
            set { SetProperty(ref firstVersionTotal, value); }
        }

        private double patentDegree;
        public double PatentDegree
        {
            get { return patentDegree; }
            set { SetProperty(ref patentDegree, value); }
        }

        private double intimeRatio;
        public double IntimeRatio
        {
            get { return intimeRatio; }
            set { SetProperty(ref intimeRatio, value); }
        }

        private double inventionRatio;
        public double InventionRatio
        {
            get { return inventionRatio; }
            set { SetProperty(ref inventionRatio, value); }
        }

        private double grantedRatio;
        public double GrantedRatio
        {
            get { return grantedRatio; }
            set { SetProperty(ref grantedRatio, value); }
        }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }

        public KPIViewModel(IRegionManager regionManager)
        {
            Zones = new List<string>();
            EndDate = DateTime.Now.Date;
            StartDate = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date;
            KPIinfos = new List<KPIinfo>();
            CheckCommand = new DelegateCommand<object[]>(OnCheck);
            SiftCommand = new DelegateCommand<object[]>(Sift);
            RefreshCommand = new DelegateCommand<object[]>(Refresh);
            PlanWindowCommand = new DelegateCommand<object[]>(OpenPlanWindow);
            ScoreWindowCommand = new DelegateCommand<object[]>(OpenScoreWindow);
            GeneratePlanTableCommand = new DelegateCommand<object[]>(GeneratePlanTable);
            GenerateDoneTableCommand = new DelegateCommand<object[]>(GenerateDoneTable);
            ExportDoneTableCommand = new DelegateCommand<object[]>(ExportDoneTable);
            SupervisorCommand = new DelegateCommand<object[]>(Supervisor);
            CheckAllCommand = new DelegateCommand<object[]>(CheckAll);

            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);

        }


    }
}
