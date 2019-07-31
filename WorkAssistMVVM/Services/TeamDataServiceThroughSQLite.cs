using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;
using System.Data;

namespace WorkAssistMVVM.Services
{
    class TeamDataServiceThroughSQLite : ITeamCaseService
    {
        public List<TaskDetail> GetAllTasks()
        {
            List<TaskDetail> allTasks = new List<TaskDetail>();
            string sql = "select * from Caselist,FirstVirsionRule";
            sql = sql + " WHERE Caselist.任务名称=FirstVirsionRule.任务名称";
            sql = sql + " AND Caselist.申请类型=FirstVirsionRule.申请类型";
            sql = sql + " AND Caselist.是否套案=FirstVirsionRule.是否套案";
            sql = sql + " AND Caselist.代理人处理状态 IN ('未处理','撰写中','客户补充资料')";

            DataTable dt = DBSQLite.GetDataTableBySQL(sql);

            foreach (DataRow dr in dt.Rows)
            {
                TaskDetail taskDetail = new TaskDetail();
                taskDetail.Taskinfo.Attorney = dr["部门成员"].ToString();
                taskDetail.Taskinfo.TaskName = dr["任务名称"].ToString();
                taskDetail.Caseinfo.CasedocumentName = dr["案件名称"].ToString();
                taskDetail.Caseinfo.AttorneySeries = dr["我方文号"].ToString();
                taskDetail.Caseinfo.AppType = dr["申请类型"].ToString();
                if(dr["初稿期限"] != null) taskDetail.Taskinfo.FirstVirsionDeadlineInternal = Convert.ToDateTime(dr["初稿期限"]);
                taskDetail.Taskinfo.ProcessStage = dr["代理人处理状态"].ToString();
                taskDetail.Caseinfo.ClientName = dr["客户名称"].ToString();
                if (dr["初稿日期"] != null) taskDetail.Taskinfo.FirstVirsionDate = Convert.ToDateTime(dr["初稿日期"]);
                taskDetail.Taskinfo.Department = dr["部门名称"].ToString();
                taskDetail.Caseinfo.Remark = dr["案件备注"].ToString();
                taskDetail.Taskinfo.Weight = Convert.ToDouble(dr["权值"]);

                allTasks.Add(taskDetail);
            }

            return allTasks;
        }
    }
}
