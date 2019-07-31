using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace WorkAssistMVVM.ViewModels
{
    class AttorneySummarizeViewModel:BindableBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string zone;
        public string Zone
        {
            get { return zone; }
            set { SetProperty(ref zone, value); }
        }
        private double weight;
        public double Weight
        {
            get { return weight; }
            set { SetProperty(ref weight, value); }
        }
    }
}
