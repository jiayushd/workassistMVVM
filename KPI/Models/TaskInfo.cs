using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Models
{
    public class TaskInfo
    {
        public string CaseID { get; set; }
        public string AttorneySeries { get; set; }//我方文号
        public string ClientSeries { get; set; }//客户文号
        public string Applicant { get; set; }//申请人
        public DateTime EntrustDate { get; set; }//委案日期
        public string ClientName { get; set; }//客户名称
        public string TechdocumentName { get; set; }
        public string CasedocumentName { get; set; }//案件名称
        public int ClaimNum { get; set; }//权项数量
        public string Attorney { get; set; }//我方文号
        public string AppType { get; set; }//申请类型
        public string AppSerialNum { get; set; }//申请号
        public string IsSuite { get; set; }//是否套案
        public string TaskID { get; set; }//任务ID
        public string TaskName { get; set; }//任务名称
        public string TaskAttribute { get; set; }//任务属性
        public string TaskCatogry { get; set; }//任务标识
        public DateTime FirstVirsionDeadlineInternal { get; set; }//初稿期限内
        public DateTime FirstVirsionDate { get; set; }//初稿日
        public DateTime DoneDate { get; set; }//送官方日
        public string ProcessStage { get; set; }//代理人处理状态

        public string Department { get; set; }//承办人部门
        public double Weight { get; set; }//权值
    }
}
