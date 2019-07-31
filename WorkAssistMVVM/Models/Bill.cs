using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkAssistMVVM.Models
{
    public class Bill
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public double CN_Point { get; set; }
        public double F_Point { get; set; }
        public string Level { get; set; }
    }
}
