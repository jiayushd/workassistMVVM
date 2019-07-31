using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using System.Collections.ObjectModel;
using WorkAssistMVVM.Models;
using Microsoft.Win32;

namespace WorkAssistMVVM.ViewModels
{
    class KPIPlanViewModel : BindableBase
    {
        private string _title = "KPI计划";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string year;
        public string Year
        {
            get { return year; }
            set { SetProperty(ref year, value); }
        }

        private string month;
        public string Month
        {
            get { return month; }
            set { SetProperty(ref month, value); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { SetProperty(ref filePath, value); }
        }

        private ObservableCollection<KPIinfo> kPIPlans;
        public ObservableCollection<KPIinfo> KPIPlans
        {
            get { return kPIPlans; }
            set { SetProperty(ref kPIPlans, value); }
        }

        public DelegateCommand<object[]> BrowseCommand { get; private set; }
        private void BrowsePlanFile(object[] parameter)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                FilePath = openFile.FileName;
            }
        }

        public KPIPlanViewModel()
        {
            BrowseCommand = new DelegateCommand<object[]>(BrowsePlanFile);
        }
    }
}
