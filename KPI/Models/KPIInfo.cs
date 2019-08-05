using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Model
{
    public class KPIInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }//承办人
        public string Year { get; set; }//年份
        public string Period { get; set; }//考核周期
        public string Zone { get; set; }//区域
        public string Position { get; set; }//职位
        public string Examiner { get; set; }//考核人
        public string Examiner_Positon { get; set; }//考核人职务
        public string Comment { get; set; }//评语
        public double DonePoint { get; set; }//递交权值
        public double FirstVirsionPoint { get; set; }//初稿权值
        public double PatentDegree { get; set; }//专利度
        public double InTimePortion { get; set; }//及时率
        public double DonePoint_Target { get; set; }//递交权值目标
        public double FirstVirsionPoint_Target { get; set; }//初稿权值目标
        public double PatentDegree_Target { get; set; }//专利度目标
        public double InTimePortion_Target { get; set; }//及时率目标
        public double Score { get; set; }//得分
        public double Score_Attitude { get; set; }//态度得分
        public int InventNum { get; set; }//发明数量
        public int InventNum_Claim_ex { get; set; }//超项发明数
        public int NewAppNum_total { get; set; }//新申请递交总数
        public int FirstVirsionNum_outofdate { get; set; }//初稿超期数量
        public int FirstVirsionNum_total { get; set; }//初稿数量
        public int Score_Cowork { get; set; }//团队协作打分
        public int Score_Passion { get; set; }//积极性打分
        public int Score_Selfdrive { get; set; }//主动性打分
        public string Month { get; set; }//月
    }
}
