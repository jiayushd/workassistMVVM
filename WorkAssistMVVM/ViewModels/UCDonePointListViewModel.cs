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

namespace WorkAssistMVVM.ViewModels
{
    class UCDonePointListViewModel:BindableBase, INavigationAware
    {
        private string _title = "递交权值";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public List<Department> Departments { get; set; }
        public string Cookie { get; set; }
        private int year;
        public int Year
        {
            get { return year; }
            set { SetProperty(ref year, value); }
        }
        private int month;
        public int Month
        {
            get { return month; }
            set { SetProperty(ref month, value); }
        }

        private double totalDonePoint;
        public double TotalDonePoint
        {
            get { return totalDonePoint; }
            set { SetProperty(ref totalDonePoint, value); }
        }

        private ObservableCollection<DonePointViewModel> donepointList;
        public ObservableCollection<DonePointViewModel> DonepointList
        {
            get { return donepointList; }
            set { SetProperty(ref donepointList, value); }
        }

        public DelegateCommand<object[]> QueryDonePointsListCommand { get; private set; }
        private void QueryDonePointsList(object[] parameters)
        {
            HttpDataService httpDataService = new HttpDataService(Cookie);
            List<Bill> bills = new List<Bill>();
            TotalDonePoint = 0.0;
            foreach (Department dept in Departments)
            {
                bills = bills.Concat( httpDataService.GetBill(dept,Year.ToString(),Month.ToString(),Cookie)).ToList();
            }
            DonepointList = new ObservableCollection<DonePointViewModel>();
            string currentParentDepartment = bills.First().Zone;
            foreach (Bill bill in bills)
            {
                DonePointViewModel donepoint = new DonePointViewModel();
                donepoint.DonePoint = bill.CN_Point + bill.F_Point;
                donepoint.Department = bill.Zone;
                donepoint.Name = bill.Name;
                donepoint.Level = bill.Level;
                DonepointList.Add(donepoint);
                if (donepoint.Department != null && donepoint.Department.Substring(17,5).Equals(currentParentDepartment.Substring(17, 5))) 
                {
                    if (donepoint.Department.Equals(currentParentDepartment))
                    {
                        TotalDonePoint += donepoint.DonePoint;
                    }
                    else 
                    {
                        if (!donepoint.Department.Contains(currentParentDepartment))
                        {
                            TotalDonePoint += donepoint.DonePoint;
                            currentParentDepartment = donepoint.Department;
                        }
                    }

                }
            }
            TotalDonePoint = Math.Round(TotalDonePoint,4);
        }

        public DelegateCommand<object[]> NextYearCommand { get; set; }
        private void NextYear(object[] parameters)
        {
            ++Year;
        }

        public DelegateCommand<object[]> PreviousYearCommand { get; set; }
        private void PreviousYear(object[] parameters)
        {
            --Year;
        }

        public DelegateCommand<object[]> NextMonthCommand { get; set; }
        private void NextMonth(object[] parameters)
        {
            if (Month < 12)
            {
                ++Month;
            }
        }

        public DelegateCommand<object[]> PreviousMonthCommand { get; set; }
        private void PreviousMonth(object[] parameters)
        {
            if (Month > 0)
            {
                --Month;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Cookie = navigationContext.Parameters["cookie"] as string;
            Departments = navigationContext.Parameters["departments"] as List<Department>;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public UCDonePointListViewModel()
        {
            Year = DateTime.Now.Year;
            Month = DateTime.Now.Month;
            QueryDonePointsListCommand = new DelegateCommand<object[]>(QueryDonePointsList);
            NextYearCommand = new DelegateCommand<object[]>(NextYear);
            PreviousYearCommand = new DelegateCommand<object[]>(PreviousYear);
            PreviousMonthCommand = new DelegateCommand<object[]>(PreviousMonth);
            NextMonthCommand = new DelegateCommand<object[]>(NextMonth);

        }
    }
}
