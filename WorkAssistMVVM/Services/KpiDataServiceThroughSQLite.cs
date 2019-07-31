using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;

namespace WorkAssistMVVM.Services
{
    class KpiDataServiceThroughSQLite : IKpiService
    {
        public List<KPIinfo> GetKPIs(List<string> years, List<string> months)
        {
            List<KPIinfo> kpis = new List<KPIinfo>();
            List<string> years1 = new List<string>();
            List<string> months1 = new List<string>();

            string sql = "select * from KPI LEFT OUTER JOIN Bill ON (KPI.承办人 = Bill.姓名 AND KPI.月份=Bill.月 AND KPI.年份=Bill.年),ExamRelation WHERE (KPI.年份 in ({0}) AND KPI.月份 in ({1}) AND KPI.承办人=ExamRelation.被考核者姓名)";
            sql = string.Format(sql, "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            int Index = 0;
            if (dt != null)
            {
                List<string> names = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    KPIinfo kpi = new KPIinfo();
                    kpi.Index = ++Index;

                    //kpi.FirstVirsionPoint = (double)dr["初稿权值"];
                    kpi.InTimePortion = (double)dr["及时交付率"];
                    kpi.DonePoint_Target = (double)dr["递交目标"];
                    kpi.FirstVirsionPoint_Target = (double)dr["初稿目标"];
                    kpi.PatentDegree_Target = (double)dr["专利度目标"];
                    kpi.InTimePortion_Target = (double)dr["及时交付目标"];
                    kpi.Year = dr["年份"].ToString();
                    kpi.Period = dr["考核周期"].ToString();
                    kpi.Name = dr["承办人"].ToString();
                    kpi.Zone = dr["区域"].ToString();
                    kpi.Month = dr["月份"].ToString();
                    kpi.Score_Cowork = Convert.ToInt32(dr["团队协作打分"]);
                    kpi.Score_Passion = Convert.ToInt32(dr["积极性打分"]);
                    kpi.Score_Selfdrive = Convert.ToInt32(dr["主动性打分"]);
                    kpi.Comment = dr["评语"].ToString();
                    kpi.Position = dr["被考核者职位"].ToString();
                    kpi.Examiner = dr["考核者姓名"].ToString();
                    kpi.Examiner_Positon = dr["考核者职位"].ToString();
                    names.Add(kpi.Name);
                    names.Add(kpi.Name + "(离职)");
                    years1.Add(kpi.Year);
                    months1.Add(kpi.Month);
                    if ((double)dr["递交权值"] != 0)
                    {
                        kpi.DonePoint = (double)dr["递交权值"];
                    }
                    else
                    {
                        if (dr["总权值"].ToString() != "")
                        {
                            kpi.DonePoint = (double)dr["总权值"];
                        }
                        else
                        {
                            kpi.DonePoint = GetDonePoint(kpi.Name, kpi.Year, kpi.Month);
                        }
                    }
                    kpi.PatentDegree = CalculatePatentDegree(names, years1, months1);
                    kpi.InTimePortion = CalculateInTimeRatio(names, years1, months1);
                    if ((double)dr["初稿权值"] != 0)
                    {
                        kpi.FirstVirsionPoint = (double)dr["初稿权值"];
                    }
                    else
                    {
                        kpi.FirstVirsionPoint = CalculateFirstVersionPoint(names, years1, months1);
                    }

                    kpi.Score = Math.Round((Math.Min(kpi.DonePoint * 100 / kpi.DonePoint_Target, 100) * 0.3 +
                        Math.Min(kpi.FirstVirsionPoint * 100 / kpi.FirstVirsionPoint_Target, 100) * 0.25 +
                        Math.Min(kpi.PatentDegree * 100 / kpi.PatentDegree_Target, 100) * 0.1 +
                        Math.Min(kpi.InTimePortion * 100 / kpi.InTimePortion_Target, 100) * 0.2 +
                        (kpi.Score_Cowork + kpi.Score_Passion + kpi.Score_Selfdrive) * 0.05), 2);

                    kpis.Add(kpi);
                }

            }

            return kpis;
        }

        private static double GetDonePoint(string name, string year, string month)
        {
            string sql = "select 总权值 from Bill";
            sql = sql + " WHERE 年='" + year + "'";
            sql = sql + " AND 月='" + month + "'";
            sql = sql + " AND 姓名 LIKE '" + name + "(离职)'";

            DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            double DonePointTotal = 0.0;
            if (/*dr["总权值"].ToString() != ""*/ dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DonePointTotal = (double)dr["总权值"];
            }

            return DonePointTotal;
        }

        private static double CalculatePatentDegree(List<string> names, List<string> years, List<string> months)
        {
            string sql = "select * from Donelist ";
            sql = sql + "where 承办人 in ({0})";
            sql = sql + " AND (strftime('%Y',送官方日期) in ({1}))";
            sql = sql + " AND (strftime('%m',送官方日期) in ({2}))";
            sql = sql + " AND 申请类型='发明'";

            sql = String.Format(sql, "'" + String.Join("','", names.ToArray()) + "'", "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            System.Data.DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            int inventionNum = dt.Rows.Count;

            sql = sql + " AND 权项总数>10";

            dt.Clear();
            dt = DBSQLite.GetDataTableBySQL(sql);

            int inventionNum_claimNumex = dt.Rows.Count;

            double patentDegree = 0.0;
            if (inventionNum > 0)
            {
                patentDegree = (double)inventionNum_claimNumex / inventionNum;
            }

            patentDegree = Math.Round(patentDegree, 2);

            return patentDegree;

        }

        private static double CalculateInventionRatio(List<string> names, List<string> years, List<string> months)
        {
            string sql = "select * from Donelist ";
            sql = sql + "where 承办人 in ({0})";
            sql = sql + " AND (strftime('%Y',送官方日期) in ({1}))";
            sql = sql + " AND (strftime('%m',送官方日期) in ({2}))";
            sql = sql + " AND 申请类型='发明'";

            sql = String.Format(sql, "'" + String.Join("','", names.ToArray()) + "'", "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            System.Data.DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            int inventionNum = dt.Rows.Count;

            sql = "select * from Donelist ";
            sql = sql + "where 承办人 in ({0})";
            sql = sql + " AND (strftime('%Y',送官方日期) in ({1}))";
            sql = sql + " AND (strftime('%m',送官方日期) in ({2}))";
            sql = sql + " AND 申请类型 IN('发明','实用新型')";

            sql = String.Format(sql, "'" + String.Join("','", names.ToArray()) + "'", "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            dt.Clear();
            dt = DBSQLite.GetDataTableBySQL(sql);

            int newappNum = dt.Rows.Count;

            double inventionRatio = 0.0;
            if (inventionNum > 0)
            {
                inventionRatio = (double)inventionNum / newappNum;
            }

            inventionRatio = Math.Round(inventionRatio, 2);

            return inventionRatio;

        }

        private static double CalculateInTimeRatio(List<string> names, List<string> years, List<string> months)
        {
            string sql = "select * from FirstvirsionList ";
            sql = sql + "where 承办人 in ({0})";
            sql = sql + " AND (strftime('%Y',初稿日) in ({1}))";
            sql = sql + " AND (strftime('%m',初稿日) in ({2}))";

            sql = String.Format(sql, "'" + String.Join("','", names.ToArray()) + "'", "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            System.Data.DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            int firstversionNum = dt.Rows.Count;

            sql = sql + " AND 初稿日>初稿期限";

            dt.Clear();
            dt = DBSQLite.GetDataTableBySQL(sql);

            int firstversionNum_outofdate = dt.Rows.Count;

            double InTimeRatio = 0.0;
            if (firstversionNum > 0)
            {
                InTimeRatio = (double)(firstversionNum - firstversionNum_outofdate) / firstversionNum;
            }

            InTimeRatio = Math.Round(InTimeRatio, 2);

            return InTimeRatio;

        }

        private static double CalculateFirstVersionPoint(List<string> names, List<string> years, List<string> months)
        {
            string sql = "select SUM(权值) AS 初稿权值 from FirstvirsionList,FirstVirsionRule ";
            sql = sql + "where 承办人 in ({0})";
            sql = sql + " AND (strftime('%Y',初稿日) in ({1}))";
            sql = sql + " AND (strftime('%m',初稿日) in ({2}))";
            sql = sql + " AND FirstvirsionList.任务名称=FirstVirsionRule.任务名称";
            sql = sql + " AND FirstvirsionList.任务属性=FirstVirsionRule.任务属性";
            sql = sql + " AND FirstvirsionList.申请类型=FirstVirsionRule.申请类型";
            sql = sql + " AND FirstvirsionList.是否套案=FirstVirsionRule.是否套案";

            sql = String.Format(sql, "'" + String.Join("','", names.ToArray()) + "'", "'" + String.Join("','", years.ToArray()) + "'", "'" + String.Join("','", months.ToArray()) + "'");

            System.Data.DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            DataRow dr = dt.Rows[0];

            double FirstVersionPoint = 0.0;
            if (dr["初稿权值"].ToString() != "")
            {
                FirstVersionPoint = (double)dr["初稿权值"];

            }
            return FirstVersionPoint;

        }

        private static double CalculateDonePoint(string year, List<string> months)
        {
            string sql = "select SUM(总权值) AS 递交总权值 from Bill";
            sql = sql + " WHERE 年='" + year + "'";
            sql = sql + " AND 月 in ({0})";

            sql = String.Format(sql, "'" + String.Join("','", months.ToArray()) + "'");

            System.Data.DataTable dt = DBSQLite.GetDataTableBySQL(sql);
            double DonePointTotal = 0.0;
            DataRow dr = dt.Rows[0];
            if (dr["递交总权值"].ToString() != "")
            {
                DonePointTotal = (double)dr["递交总权值"];

            }

            return DonePointTotal;

        }

    }
}
