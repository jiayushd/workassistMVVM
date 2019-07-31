using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkAssistMVVM.Models
{
    public class CaseInfo
    {
        public string CaseID { get; set; }
        public string AttorneySeries { get; set; }//我方文号
        public string ClientSeries { get; set; }//客户文号
        public string Applicant { get; set; }//申请人
        public DateTime EntrustDate { get; set; }//委案日期
        public string SalesmanID { get; set; }//案源人
        public string ClientName { get; set; }//客户名称
        public string TechdocumentName { get; set; }
        public string CasedocumentName { get; set; }//案件名称
        //public string ExtraInfo { get; set; }
        public string AppType { get; set; }//申请类型
        public string AppSerialNum { get; set; }//申请号
        //public string AppStatus { get; set; }
        //public DateTime ApplicationDate { get; set; }
        public string IsSuite { get; set; }
        public string Remark { get; set; }

        public List<TaskInfo> taskInfos { get; set; }
    }
}
