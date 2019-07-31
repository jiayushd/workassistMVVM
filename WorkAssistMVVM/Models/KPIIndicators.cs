using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkAssistMVVM.Models
{
    class KPIIndicators
    {
        public string Name { get; set; }
        public string Period { get; set; }
        //public double Indicator1 { get; set; }
        //public double Indicator2 { get; set; }
        //public double Indicator3 { get; set; }
        //public double Indicator4 { get; set; }
        //public double Indicator5 { get; set; }
        //public double Indicator6 { get; set; }
        //public double Indicator7 { get; set; }
        //public double Indicator8 { get; set; }
        //public double Indicator9 { get; set; }
        //public double Indicator10 { get; set; }
        public List<double> Indicators { get; set; }
        //public string Indicator1Name { get; set; }
        //public string Indicator2Name { get; set; }
        //public string Indicator3Name { get; set; }
        //public string Indicator4Name { get; set; }
        //public string Indicator5Name { get; set; }
        //public string Indicator6Name { get; set; }
        //public string Indicator7Name { get; set; }
        //public string Indicator8Name { get; set; }
        //public string Indicator9Name { get; set; }
        //public string Indicator10Name { get; set; }
        public List<string> IndicatorNames { get; set; }
        public double Score { get; set; }
        public string Grade { get; set; }

        public KPIIndicators()
        {
            Indicators = new List<double>();
            IndicatorNames = new List<string>();
        }
    }
}
