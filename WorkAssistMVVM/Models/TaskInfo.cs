using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkAssistMVVM.Models
{
    public class TaskInfo
    {
        public string TaskID { get; set; }//我方文号和任务名称
        public string TaskName { get; set; }//任务名称
        public string TaskAttribute { get; set; }//任务属性
        public string TaskCatogry { get; set; }//任务标识
        public string Attorney { get; set; }//承办人
        public DateTime? DistributeDate { get; set; }//配案日期
        public DateTime FirstVirsionDeadlineInternal { get; set; }//初稿期限内
        public DateTime? FirstVirsionDeadlineOutside { get; set; }//初稿期限外
        public DateTime? FirstVirsionDate { get; set; }//初稿日
        public DateTime? DoneDeadlineInternal { get; set; }//定稿期限内
        public DateTime? DoneDeadlineOutside { get; set; }//定稿期限外
        public DateTime? OfficalDeadline { get; set; }//官方期限
        public DateTime? OfficalActionDate { get; set; }//官方期限
        public DateTime? DoneDate { get; set; }//完成日
        public DateTime? Deliver_to_Office_Date { get; set; }//送官方日期
        public string ProcessStage { get; set; }//代理人处理状态
        //public string TaskStatus { get; set; }
        public string Department { get; set; }//承办人部门
        public double Weight { get; set; }//权值
    }
}
