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
using Microsoft.Win32;
using System.Diagnostics;

namespace WorkAssistMVVM.ViewModels
{
    class KPISummarizeViewModel:BindableBase
    {
        //private readonly IRegionManager _regionManager;

        private string _title = "绩效汇总";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        //员工绩效文件夹路径
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }
        //干部绩效文件夹路径
        private string filePathS;
        public string FilePathS
        {
            get { return filePathS; }
            set { SetProperty(ref filePathS, value); }
        }

        private ObservableCollection<KPIIndicators> kPISummarizelist;
        public ObservableCollection<KPIIndicators> KPISummarizelist
        {
            get { return kPISummarizelist; }
            set { SetProperty(ref kPISummarizelist, value); }
        }

        private ObservableCollection<KPIIndicators> kPISSummarizelist;
        public ObservableCollection<KPIIndicators> KPISSummarizelist
        {
            get { return kPISSummarizelist; }
            set { SetProperty(ref kPISSummarizelist, value); }
        }

        private ObservableCollection<ErrorInfo> errorInfos;
        public ObservableCollection<ErrorInfo> ErrorInfos
        {
            get { return errorInfos; }
            set { SetProperty(ref errorInfos, value); }
        }

        //浏览员工绩效文件夹
        public DelegateCommand<object[]> BrowseCommand { get; private set; }
        private void BrowsePlanFile(object[] parameter)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.ShowDialog();
            FilePath = folder.SelectedPath;
        }
        //浏览干部绩效文件夹
        public DelegateCommand<object[]> BrowseSCommand { get; private set; }
        private void BrowseSPlanFile(object[] parameter)
        {
            System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();
            folder.ShowDialog();
            FilePathS = folder.SelectedPath;
        }

        //读取员工绩效文件
        public DelegateCommand<object[]> ReadCommand { get; private set; }
        private void ReadPlanFile(object[] parameter)
        {
            if (Directory.Exists(FilePath))
            {
                FileInfo fileInfo = new FileInfo(FilePath);
                DirectoryInfo folder = new DirectoryInfo(FilePath);
                KPISummarizelist = new ObservableCollection<KPIIndicators>();
                ErrorInfos = new ObservableCollection<ErrorInfo>();

                //try
                //{
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    foreach (FileInfo file in folder.GetFiles("*.xls"))
                    {
                        if (!file.Name.Contains("~$"))
                        {
                            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file.FullName);
                            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
                            for (int i = 1; i <= sheets.Count; i++)
                            {
                                Microsoft.Office.Interop.Excel.Worksheet wsh = sheets.Item[i];
                                string sheetname = wsh.Name;

                                if (wsh.Cells[17, 2].value == "被考核者姓名" && wsh.Cells[16, 2].value == "绩效考核指标总得分" && wsh.Cells[5, 12].value == "考核得分")
                                {
                                    KPISummarizelist.Add(GetKPIIndicators(file, wsh));
                                }
                                else
                                {
                                    ErrorInfo error = new ErrorInfo();
                                    error.FileName = file.Name;
                                    error.SheetName = sheetname;
                                    error.Information = "未使用标准表格";
                                    ErrorInfos.Add(error);
                                }
                            }
                            wbks.Close();
                        }
                    }

                //}
                //catch (Exception ex)
                //{

                //    MessageBox.Show(ex.Message);

                //    throw;
                //}


            }
            else
            {
                MessageBox.Show("当前选择的路径不存在", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //读取干部绩效文件
        public DelegateCommand<object[]> ReadSCommand { get; private set; }
        private void ReadSPlanFile(object[] parameter)
        {
            if (Directory.Exists(FilePathS))
            {
                FileInfo fileInfo = new FileInfo(FilePathS);
                DirectoryInfo folder = new DirectoryInfo(FilePathS);
                KPISSummarizelist = new ObservableCollection<KPIIndicators>();
                ErrorInfos = new ObservableCollection<ErrorInfo>();

                try
                {
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    foreach (FileInfo file in folder.GetFiles("*.xls"))
                    {
                        if (!file.Name.Contains("~$"))
                        {
                            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file.FullName);
                            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
                            for (int i = 1; i <= sheets.Count; i++)
                            {
                                Microsoft.Office.Interop.Excel.Worksheet wsh = sheets.Item[i];
                                string sheetname = wsh.Name;

                                if (wsh.Cells[18, 2].value == "被考核者姓名" && wsh.Cells[17, 2].value == "绩效考核指标总得分" && wsh.Cells[5, 10].value == "考核得分")
                                {
                                    KPISSummarizelist.Add(GetKPIIndicatorsofSupervisor(file, wsh));
                                }
                                else
                                {
                                    ErrorInfo error = new ErrorInfo();
                                    error.FileName = file.Name;
                                    error.SheetName = sheetname;
                                    error.Information = "未使用标准表格";
                                    ErrorInfos.Add(error);
                                }
                            }
                            wbks.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }

            }
            else
            {
                MessageBox.Show("当前选择的路径不存在", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DelegateCommand<object[]> ExportCommand { get; private set; }
        private void ExportSummarizeList(object[] parameter)
        {
            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string file = exepath + "MyData\\导出表.xlsx";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file);
            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
            Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["Sheet1"];
            int i = 0;
            if (KPISummarizelist.Count > 0)
            {
                foreach (KPIIndicators item in KPISummarizelist)
                {
                    wsh.Cells[i + 2, 1].value = item.Name;
                    wsh.Cells[i + 2, 2].value = item.Period;
                    for (int j = 0; j < 9; j++)
                    {
                        wsh.Cells[i + 2, j * 2 + 3].value = item.IndicatorNames[j];
                        wsh.Cells[i + 2, j * 2 + 4].value = item.Indicators[j];

                    }
                    wsh.Cells[i + 2, 23].value = item.Score;
                    wsh.Cells[i + 2, 24].value = item.Grade;
                    i++;
                }
                if (!Directory.Exists(FilePathS + "\\汇总"))
                {
                    Directory.CreateDirectory(FilePathS + "\\汇总");
                }
                string fileSummarize = FilePathS + "\\汇总\\员工汇总表.xlsx";
                if (File.Exists(fileSummarize))
                {
                    try
                    {
                        File.Delete(fileSummarize);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                }
                wsh.SaveAs(fileSummarize);
                wbks.Close();
                System.Windows.MessageBox.Show("生成完毕！", "", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start("explorer.exe ", FilePathS + "\\汇总");

            }

            else
            {
                System.Windows.MessageBox.Show("没有数据", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DelegateCommand<object[]> ExportSCommand { get; private set; }
        private void ExportSSummarizeList(object[] parameter)
        {
            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string file = exepath + "MyData\\导出表.xlsx";
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(file);
            Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
            Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["Sheet1"];
            int i = 0;
            if (KPISSummarizelist.Count > 0)
            {
                foreach (KPIIndicators item in KPISSummarizelist)
                {
                    wsh.Cells[i + 2, 1].value = item.Name;
                    wsh.Cells[i + 2, 2].value = item.Period;
                    for (int j = 0; j < 10; j++)
                    {
                        wsh.Cells[i + 2, j * 2 + 3].value = item.IndicatorNames[j];
                        wsh.Cells[i + 2, j * 2 + 4].value = item.Indicators[j];

                    }
                    wsh.Cells[i + 2, 23].value = item.Score;
                    wsh.Cells[i + 2, 24].value = item.Grade;
                    i++;
                }
                if (!Directory.Exists(FilePathS + "\\汇总"))
                {
                    Directory.CreateDirectory(FilePathS + "\\汇总");
                }
                string fileSummarize = FilePathS + "\\汇总\\干部汇总表.xlsx";
                if (File.Exists(fileSummarize))
                {
                    try
                    {
                        File.Delete(fileSummarize);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }
                }

                wsh.SaveAs(fileSummarize);
                wbks.Close();
                MessageBox.Show("生成完毕！", "", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start("explorer.exe ", FilePathS + "\\汇总");
            }
            else
            {
                MessageBox.Show("没有数据", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public KPISummarizeViewModel()
        {
            BrowseCommand = new DelegateCommand<object[]>(BrowsePlanFile);
            BrowseSCommand = new DelegateCommand<object[]>(BrowseSPlanFile);
            ReadCommand = new DelegateCommand<object[]>(ReadPlanFile);
            ReadSCommand = new DelegateCommand<object[]>(ReadSPlanFile);
            ExportCommand = new DelegateCommand<object[]>(ExportSummarizeList);
            ExportSCommand = new DelegateCommand<object[]>(ExportSSummarizeList);

        }

        private KPIIndicators GetKPIIndicators(FileInfo file, Microsoft.Office.Interop.Excel.Worksheet wsh)
        {
            try
            {
                KPIIndicators kPIIndicators = new KPIIndicators();
                kPIIndicators.Name = wsh.Cells[17, 4].value;
                string period = wsh.Cells[3, 2].value;
                period = period.Substring(5);
                kPIIndicators.Period = period;
                for (int i = 6; i < 15; i++)
                {
                    string s = Convert.ToString(wsh.Cells[i, 12].value );
                    if (Convert.ToString(wsh.Cells[i, 12].value) != null && Convert.ToString(wsh.Cells[i, 12].value) != "")
                    {
                        kPIIndicators.Indicators.Add(Math.Round(wsh.Cells[i, 12].value, 2));
                    }
                    else
                    {
                        kPIIndicators.Indicators.Add(0);
                    }
                    kPIIndicators.IndicatorNames.Add(wsh.Cells[i, 4].value);
                }
           
                //Score
                if (Convert.ToString(wsh.Cells[16, 4].value) != null)
                {
                    kPIIndicators.Score = Math.Round(wsh.Cells[16, 4].value, 2);
                }
                else
                {
                    kPIIndicators.Score = 0.0;
                }

                if (kPIIndicators.Score >= 95)
                {
                    kPIIndicators.Grade = "S级";
                }
                else if (kPIIndicators.Score >= 85 && kPIIndicators.Score < 95)
                {
                    kPIIndicators.Grade = "A级";
                }
                else if (kPIIndicators.Score >= 75 && kPIIndicators.Score < 85)
                {
                    kPIIndicators.Grade = "B级";
                }
                else if (kPIIndicators.Score >= 65 && kPIIndicators.Score < 75)
                {
                    kPIIndicators.Grade = "C级";
                }
                else
                {
                    kPIIndicators.Grade = "D级";
                }
                return kPIIndicators;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }

        private KPIIndicators GetKPIIndicatorsofSupervisor(FileInfo file, Microsoft.Office.Interop.Excel.Worksheet wsh)
        {
            try
            {
                KPIIndicators kPIIndicators = new KPIIndicators();
                kPIIndicators.Name = wsh.Cells[18, 4].value;
                string period = wsh.Cells[3, 2].value;
                period = period.Substring(5);
                kPIIndicators.Period = period;
                for (int i = 6; i < 16; i++)
                {
                    string s = Convert.ToString(wsh.Cells[i, 10].value);
                    if (Convert.ToString(wsh.Cells[i, 10].value) != null && Convert.ToString(wsh.Cells[i, 10].value) != "")
                    {
                        kPIIndicators.Indicators.Add(Math.Round(wsh.Cells[i, 10].value, 2));
                    }
                    else
                    {
                        kPIIndicators.Indicators.Add(0);
                    }
                    kPIIndicators.IndicatorNames.Add(wsh.Cells[i, 4].value);
                }

                //Score
                if (Convert.ToString(wsh.Cells[17, 4].value) != null)
                {
                    kPIIndicators.Score = Math.Round(wsh.Cells[17, 4].value, 2);
                }
                else
                {
                    kPIIndicators.Score = 0.0;
                }

                if (kPIIndicators.Score >= 95)
                {
                    kPIIndicators.Grade = "S级";
                }
                else if (kPIIndicators.Score >= 85 && kPIIndicators.Score < 95)
                {
                    kPIIndicators.Grade = "A级";
                }
                else if (kPIIndicators.Score >= 75 && kPIIndicators.Score < 85)
                {
                    kPIIndicators.Grade = "B级";
                }
                else if (kPIIndicators.Score >= 65 && kPIIndicators.Score < 75)
                {
                    kPIIndicators.Grade = "C级";
                }
                else
                {
                    kPIIndicators.Grade = "D级";
                }
                return kPIIndicators;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }

    }
}
