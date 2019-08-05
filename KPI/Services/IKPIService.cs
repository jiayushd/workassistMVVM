using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPI.Model;

namespace KPI.Services
{
    public interface IKPIService
    {
        List<KPIInfo> GetKPIs(List<string> years, List<string> months);

        //获取初稿权值
        double GetFirstVersionWeight(List<string> names);
        //获取递交权值
        double GetDonePoint(List<string> names);
        //获取专利度
        double GetPatentDegree(List<string> names);
        //获取及时率
        double GetIntimeRatio(List<string> names);
    }
}
