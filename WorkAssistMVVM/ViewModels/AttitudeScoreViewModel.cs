using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;

namespace WorkAssistMVVM.ViewModels
{
    class AttitudeScoreViewModel:BindableBase
    {
        private string _title = "态度得分";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
