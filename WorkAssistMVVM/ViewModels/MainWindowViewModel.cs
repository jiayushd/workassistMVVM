using Prism.Mvvm;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using WorkAssistMVVM.ViewModels;
using System.Windows.Controls;
using WorkAssistMVVM.Services;
using WorkAssistMVVM.Models;
using System.Collections.ObjectModel;
using WorkAssistMVVM.Views;
using System.Windows;
using System.IO;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Prism.Regions;
using mshtml;
using System.Text;
using System.Runtime.InteropServices;

namespace WorkAssistMVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private static string Cookie {get;set;}
        private static List<Department> Departments { get; set; }
        private UserInfo userInfo;
        public UserInfo Userinfo
        {
            get { return userInfo; }
            set { SetProperty(ref userInfo, value); }
        }

        public DelegateCommand<object[]> SearchCommand { get; private set; }
        public DelegateCommand<string> RadioButtonSelecteCommand { get; private set; }
        public DelegateCommand<object[]> KPICommand { get; private set; }
        public DelegateCommand<object[]> TeamCommand { get; private set; }
        public DelegateCommand<object[]> ForwardCommand { get; private set; }
        public DelegateCommand<object[]> BackwardCommand { get; private set; }
        public DelegateCommand<object[]> OpenlocalCommand { get; private set; }
        public DelegateCommand<object[]> CreateInventionCommand { get; private set; }
        public DelegateCommand<object[]> CreateUnityCommand { get; private set; }
        public DelegateCommand<object[]> CreateOACommand { get; private set; }
        public DelegateCommand<object[]> SelectionCommand { get; private set; }
        public DelegateCommand<WebBrowser> BrowseCommand { get; private set; }
        public DelegateCommand<WebBrowser> GetCookieCommand { get; private set; }

        public DelegateCommand<string> NavigateCommand { get; private set; }
        private void Navigate(string navigatePath)
        {
            var parameters = new NavigationParameters();
            parameters.Add("cookie", Cookie);
            parameters.Add("departments", Departments);
            if (navigatePath != null && Cookie != null)
                _regionManager.RequestNavigate("ContentRegion", navigatePath, parameters);
            else
            {
                MessageBox.Show("请先转到浏览器页签登录到系统","未登录",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            
        }

        private string _title = "Work Assist";
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
                RaisePropertyChanged();
            }
        }

        private string searchString;
        public string SearchString
        {
            get { return searchString; }
            set { SetProperty(ref searchString, value); }
        }
        

        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                SetProperty(ref count, value);
                RaisePropertyChanged();
            }
        }

        private string selectedAttorneySeries;
        public string SelectedAttorneySeries
        {
            get { return selectedAttorneySeries; }
            set { SetProperty(ref selectedAttorneySeries, value); }
        }

        private CaseInfo selectedCaseInfo;  
        public CaseInfo SelectedCaseInfo
        {
            get { return selectedCaseInfo; }
            set { SetProperty(ref selectedCaseInfo, value); }
        }

        private string casePath;
        public string CasePath
        {
            get { return casePath; }
            set { SetProperty(ref casePath, value); RaisePropertyChanged(); }

        }


        private List<string> paths; 
        public List<string> Paths
        {
            get { return paths; }
            set { SetProperty(ref paths, value); }
        }
        private int index;
        public int Index
        {
            get { return index; }
            set { SetProperty(ref index, value); }
        }

        private bool hasFolder;
        public bool HasFolder
        {
            get { return hasFolder; }
            set { SetProperty(ref hasFolder, value); }
        }

        private List<CaseInfo> caseInfos;
        public List<CaseInfo> CaseInfos
        {
            get { return caseInfos; }
            set { SetProperty(ref caseInfos, value); }
        }

        private ObservableCollection<TaskViewModel> taskLists;
        public ObservableCollection<TaskViewModel> TaskLists
        {
            get { return taskLists; }
            set
            {
                SetProperty(ref taskLists, value);
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<FileViewModel> fileList;
        public ObservableCollection<FileViewModel> FileList
        {
            get { return fileList; }
            set { SetProperty(ref fileList, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            Userinfo = new UserInfo();
            Userinfo.WorkPath = "C:\\WORK\\";
            SearchCommand = new DelegateCommand<object[]>(Search);
            RadioButtonSelecteCommand = new DelegateCommand<string>(OnRadiobuttonSelected);
            SelectionCommand = new DelegateCommand<object[]> (OnItemSelected);
            KPICommand = new DelegateCommand<object[]>(OnItemSelected);
            TeamCommand = new DelegateCommand<object[]>(OnItemSelected);
            ForwardCommand = new DelegateCommand<object[]>(OnForward);
            BackwardCommand = new DelegateCommand<object[]>(OnBackward);
            OpenlocalCommand = new DelegateCommand<object[]>(OnOpenLocal);
            CreateInventionCommand = new DelegateCommand<object[]>(OnItemSelected);
            CreateUnityCommand = new DelegateCommand<object[]>(OnItemSelected);
            CreateOACommand = new DelegateCommand<object[]>(OnItemSelected);
            BrowseCommand = new DelegateCommand<WebBrowser>(OnBrowse);
            GetCookieCommand = new DelegateCommand<WebBrowser>(GetCookie);
            DownloadFileCommand = new DelegateCommand<object[]>(DownloadFile);

            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);

        }

        public DelegateCommand<object[]> DownloadFileCommand { get; private set; }
        private void DownloadFile(object[] parameter)
        {
            FileViewModel fileinfo = (FileViewModel)parameter.FirstOrDefault();
            if (MessageBox.Show("是否下载文件到本地？","",MessageBoxButton.OKCancel,MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                //MessageBox.Show(fileinfo.FileID);
                HttpDataService hds = new HttpDataService(Cookie);
                string uri = "http://218.17.24.82:88/file_service/common.ashx";
                string postData = "call=DownLoad&user_id="+userInfo.UserId+"&file_id="+ fileinfo.FileID + "&table_code=";
                string filePath = Userinfo.WorkPath + "NewApplication\\" + SelectedAttorneySeries + "\\"+fileinfo.FileName;

                HttpDataService.DownloadTaskFile(uri, postData, filePath, Cookie);
                
            }
        }

        private void Search(object parameter)
        {
            HttpDataService hds = new HttpDataService(Cookie);
            List<CaseInfo> caseInfos = hds.SearchBySingleString(SearchString);
            TaskLists = new ObservableCollection<TaskViewModel>();
            SetTaskListViewModel(caseInfos);
            Count = TaskLists.Count;
        }

        private void OnRadiobuttonSelected(string type)
        {
            HttpDataService hds = new HttpDataService(Cookie);

            switch (type)
            {
                case "app":
                    CaseInfos = hds.GetNewApp("");
                    break;
                case "oa":
                    CaseInfos = hds.GetOA("");
                    break;
                case "other":
                    CaseInfos = hds.GetOther("");
                    break;
                default:
                    break;
            }
            TaskLists = new ObservableCollection<TaskViewModel>();
            SetTaskListViewModel(CaseInfos);
            Count = TaskLists.Count;           
        }

        private void SetTaskListViewModel(List<CaseInfo> caseInfos)
        {
            foreach (CaseInfo item in caseInfos)
            {
                TaskViewModel task = new TaskViewModel
                {
                    AttorneySeries = item.AttorneySeries,
                    CaseName = item.CasedocumentName,
                    CustomerName = item.ClientName,
                    FirstVersionDeadline = item.taskInfos[0].FirstVirsionDeadlineInternal,
                    ProcessStage = item.taskInfos[0].ProcessStage,
                    TaskName = item.taskInfos[0].TaskName,
                    TaskAttribute = item.taskInfos[0].TaskAttribute,
                    Proc_id = item.taskInfos[0].TaskID,
            };
                TaskLists.Add(task);
            }
        }

        private void OnItemSelected(object[] selectedItems)
        {
            if (selectedItems != null && selectedItems.Count() > 0)
            {
                TaskViewModel task = (TaskViewModel)selectedItems.FirstOrDefault();
                SelectedAttorneySeries = task.AttorneySeries;
                SelectedCaseInfo = CaseInfos.Find(s => s.AttorneySeries == SelectedAttorneySeries);
                Paths = new List<string>();
                CasePath = Userinfo.WorkPath + "NewApplication\\"+SelectedAttorneySeries;
                HttpDataService hds = new HttpDataService(Cookie);
                List<FileViewModel> fileinfos = hds.GetFileInfos(SelectedCaseInfo.taskInfos[0].TaskID);
                FileList = new ObservableCollection<FileViewModel>();
                foreach (var item in fileinfos)
                {
                    FileViewModel fileinfo = new FileViewModel();
                    fileinfo.FileName = item.FileName;
                    fileinfo.FileID = item.FileID;
                    fileinfo.FileDescribe = item.FileDescribe;
                    fileinfo.UploadUser = item.UploadUser;
                    fileinfo.UploadTime = item.UploadTime;

                    FileList.Add(fileinfo);
                }
                Index = 0;
            }

            //获取文件信息
            if (!Directory.Exists(CasePath))
            {
                HasFolder = true;
            }
            else
            {
                HasFolder = false;
            }

        }

        private void OnForward(object parameter)
        {
            if (Index < Paths.Count - 1)
            {
                Index = Index + 1;
                CasePath = Paths[Index];
            }

        }

        private void OnBackward(object parameter)
        {
            if (Index > 0)
            {
                Index = Index - 1;
                CasePath = Paths[Index];
            }

        }

        private void OnBrowse(object parameter)
        {
            if (parameter != null)
            {
                WebBrowser wb = (WebBrowser)parameter;

                if (wb.Source != null)
                {
                    string path = wb.Source.LocalPath;
                    if (Paths.FindIndex(s => s.Equals(path)) < 0)
                    {
                        Paths.Add(path);
                    }
                    Index = Paths.FindIndex(s=>s.Equals(path));
                }
            }
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        private int INTERNET_COOKIE_HTTPONLY = 0x00002000;
        private void GetCookie(object parameter)
        {
            if (parameter != null)
            {
                WebBrowser wb = (WebBrowser)parameter;

                HTMLDocument doc = (HTMLDocument)wb.Document;
                object obj = doc.getElementById("d_left");

                if (obj != null)
                {
                    uint datasize = 1024;
                    StringBuilder cookieData = new StringBuilder((int)datasize);
                    InternetGetCookieEx("http://www.acip.vip/index.aspx", null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
                    //Cookie = cookieData.ToString().Replace(';', ',');
                    Cookie = cookieData.ToString();

                    HttpDataService httpdataservice = new HttpDataService(Cookie);
                    Departments = httpdataservice.GetDepartmentList();
                    Userinfo.UserId = httpdataservice.GetUserID();
                }
            }
        }

        private void OnOpenLocal(object parameter)
        {
            Process.Start("explorer.exe ", Userinfo.WorkPath + "NewApplication\\" + SelectedAttorneySeries);
        }



    }
}
