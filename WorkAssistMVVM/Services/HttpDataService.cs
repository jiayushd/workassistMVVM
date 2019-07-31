using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WorkAssistMVVM.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using WorkAssistMVVM.ViewModels;
using System.Data;

namespace WorkAssistMVVM.Services
{
    public class HttpDataService : ICaseService
    {
        private string cookie;

        public HttpDataService(string cookiestring)
        {
            cookie = cookiestring;
        }
        public List<CaseInfo> GetAllUndone()
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();
            //param: {"search_range_default":false,                   "P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //非套案
            //param: {"search_range_default":false,"C43":"false;true","P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //套案
            //param: {"search_range_default":false,"C43":"true;false","P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //search_type: proc
            //search_form_code: case_search
            //call: SearchCase
            //page_size: 10
            //page_index: 0
            //get_total: true
            //key_id: proc_id
            //id: 
            //sort: create_time desc

            string sql = "select * from FirstVirsionRule";
            DataTable dt = DBSQLite.GetDataTableBySQL(sql);

            //获取权值规则字典
            Dictionary<string, double> weightList = new Dictionary<string, double>();
            foreach (DataRow dr in dt.Rows)
            {
                string type = dr["申请类型"].ToString() + dr["任务名称"].ToString() + dr["任务属性"].ToString() + dr["是否套案"].ToString();
                double weight = Convert.ToDouble(dr["权值"]);
                weightList.Add(type, weight);
            }

            //获取人员-部门信息
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "search_key=&groups=&groupType=PI&call=GetUserRankInfo&page_size=-1&page_index=0&key_id=user_id&id=&sort=";
            string content = Surfing(uri, cookie, postData);
            Dictionary<string, string> memberList = new Dictionary<string, string>();
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        memberList.Add(row["cn_name"].ToString(), row["dept_name"].ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            uri = "http://www.acip.vip/ajax/search.ashx";
            //非套案
            postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22false%3Btrue%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["C6"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["C9"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["CC3"].ToString();//客户名称
                        caseInfo.AppType = row["C2"].ToString();//申请类型
                        caseInfo.IsSuite = "否";

                        taskInfo.Attorney = row["P3"].ToString();//承办人
                        if (memberList.Keys.Contains(taskInfo.Attorney))
                        {
                            taskInfo.Department = memberList[taskInfo.Attorney];
                        }
                        else
                        {
                            taskInfo.Department = "其他";
                        }
                        taskInfo.TaskName = row["P1"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["P29"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["P5"].ToString();//处理状态
                        if (weightList.Keys.Contains(caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite))
                        {
                            taskInfo.Weight = weightList[caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite];
                        }
                        else
                        {
                            taskInfo.Weight = 0;
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            //套案
            postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22true%3Bfalse%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["C6"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["C9"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["CC3"].ToString();//客户名称
                        caseInfo.AppType = row["C2"].ToString();//申请类型
                        caseInfo.IsSuite = "是";


                        taskInfo.Attorney = row["P3"].ToString();//承办人
                        if (memberList.Keys.Contains(taskInfo.Attorney))
                        {
                            taskInfo.Department = memberList[taskInfo.Attorney];
                        }
                        else
                        {
                            taskInfo.Department = "其他";
                        }
                        taskInfo.TaskName = row["P1"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["P29"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["P5"].ToString();//处理状态

                        if (weightList.Keys.Contains(caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite))
                        {
                            taskInfo.Weight = weightList[caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite];
                        }
                        else
                        {
                            taskInfo.Weight = 0;
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return caseInfos;
        }

        public List<CaseInfo> GetNewApp(string name)
        {
            return Get_Case("ap", cookie);
        }

        public List<CaseInfo> GetOA(string name)
        {
            return Get_Case("oa", cookie);
        }

        public List<CaseInfo> GetOther(string name)
        {
            return Get_Case("other", cookie);
        }

        public List<CaseInfo> GetAllUndone(string name)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();
            string sql = "select * from FirstVirsionRule";
            DataTable dt = DBSQLite.GetDataTableBySQL(sql);

            //获取权值规则字典
            Dictionary<string, double> weightList = new Dictionary<string, double>();
            foreach (DataRow dr in dt.Rows)
            {
                string type = dr["申请类型"].ToString() + dr["任务名称"].ToString() + dr["任务属性"].ToString() + dr["是否套案"].ToString();
                double weight = Convert.ToDouble(dr["权值"]);
                weightList.Add(type, weight);
            }

            string nameUTF = "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(name)).Replace("-", "%");
            //获取人员信息
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "search_key="+ nameUTF + "&groups=&groupType=PI&call=GetUserRankInfo&page_size=10&page_index=0&get_total=true&key_id=user_id&id=&sort=";
            string content = Surfing(uri, cookie, postData);
            string userid = "";
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (var item in table)
                    {
                        userid = item["user_id"].ToString();
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            uri = "http://www.acip.vip/ajax/case_info.ashx";
            //新申请
            postData = "ctrl_proc_code=ap&proc_status=&user_id="+userid+"&search_key=&call=GetUserCaseList&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=legal_due_date+asc%2C+int_first_date+asc";
            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["volume"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["case_name"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["customer_name"].ToString();//客户名称
                        caseInfo.AppType = row["apply_type_name"].ToString();//申请类型

                        taskInfo.Attorney = name;//承办人

                        taskInfo.TaskName = row["ctrl_proc"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["ctrl_proc_property"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["proc_status_name"].ToString();//处理状态
                        if (row["int_first_date"].ToString() != "")
                        {
                            taskInfo.FirstVirsionDeadlineInternal = Convert.ToDateTime(row["int_first_date"]);
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return caseInfos;
        }

        public List<CaseInfo> GetCandoUndone()
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();
            //param: {"search_range_default":false,                   "P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //非套案
            //param: {"search_range_default":false,"C43":"false;true","P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //套案
            //param: {"search_range_default":false,"C43":"true;false","P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9;6f52ce6f-1f02-49c2-a056-936f01a67653;8B9421A0-E8D7-4E00-9B95-6DB23712CE62;ED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6;b80609da-66af-44b9-8d22-06c03bd58626;c20e516c-301e-4661-8c79-4a065ebc720e","P16":"true;;","P7":"true;;"}
            //search_type: proc
            //search_form_code: case_search
            //call: SearchCase
            //page_size: 10
            //page_index: 0
            //get_total: true
            //key_id: proc_id
            //id: 
            //sort: create_time desc

            string sql = "select * from FirstVirsionRule";
            DataTable dt = DBSQLite.GetDataTableBySQL(sql);

            //获取权值规则字典
            Dictionary<string, double> weightList = new Dictionary<string, double>();
            foreach (DataRow dr in dt.Rows)
            {
                string type = dr["申请类型"].ToString() + dr["任务名称"].ToString() + dr["任务属性"].ToString() + dr["是否套案"].ToString();
                double weight = Convert.ToDouble(dr["权值"]);
                weightList.Add(type, weight);
            }

            //获取人员-部门信息
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "search_key=&groups=&groupType=PI&call=GetUserRankInfo&page_size=-1&page_index=0&key_id=user_id&id=&sort=";
            string content = Surfing(uri, cookie, postData);
            Dictionary<string, string> memberList = new Dictionary<string, string>();
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        memberList.Add(row["cn_name"].ToString(), row["dept_name"].ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            uri = "http://www.acip.vip/ajax/search.ashx";
            //非套案
            //postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22false%3Btrue%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22false%3Btrue%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P28%22%3A%22untreated%3Bcus_supplement%3Bwriting%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";

            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["C6"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["C9"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["CC3"].ToString();//客户名称
                        caseInfo.AppType = row["C2"].ToString();//申请类型
                        caseInfo.IsSuite = "否";

                        taskInfo.Attorney = row["P3"].ToString();//承办人
                        if (memberList.Keys.Contains(taskInfo.Attorney))
                        {
                            taskInfo.Department = memberList[taskInfo.Attorney];
                        }
                        else
                        {
                            taskInfo.Department = "其他";
                        }
                        taskInfo.TaskName = row["P1"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["P29"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["P5"].ToString();//处理状态
                        if (weightList.Keys.Contains(caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite))
                        {
                            taskInfo.Weight = weightList[caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite];
                        }
                        else
                        {
                            taskInfo.Weight = 0;
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            //套案
            //postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22true%3Bfalse%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C43%22%3A%22true%3Bfalse%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%3B6f52ce6f-1f02-49c2-a056-936f01a67653%3B8B9421A0-E8D7-4E00-9B95-6DB23712CE62%3BED81B9BA-A067-4D1C-9B40-75BE8FAAE2B6%3Bb80609da-66af-44b9-8d22-06c03bd58626%3Bc20e516c-301e-4661-8c79-4a065ebc720e%22%2C%22P28%22%3A%22untreated%3Bcus_supplement%3Bwriting%22%2C%22P16%22%3A%22true%3B%3B%22%2C%22P7%22%3A%22true%3B%3B%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=10&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["C6"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["C9"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["CC3"].ToString();//客户名称
                        caseInfo.AppType = row["C2"].ToString();//申请类型
                        caseInfo.IsSuite = "是";


                        taskInfo.Attorney = row["P3"].ToString();//承办人
                        if (memberList.Keys.Contains(taskInfo.Attorney))
                        {
                            taskInfo.Department = memberList[taskInfo.Attorney];
                        }
                        else
                        {
                            taskInfo.Department = "其他";
                        }
                        taskInfo.TaskName = row["P1"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["P29"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["P5"].ToString();//处理状态

                        if (weightList.Keys.Contains(caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite))
                        {
                            taskInfo.Weight = weightList[caseInfo.AppType + taskInfo.TaskName + taskInfo.TaskAttribute + caseInfo.IsSuite];
                        }
                        else
                        {
                            taskInfo.Weight = 0;
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            return caseInfos;
        }

        public List<CaseInfo> GetUndone(string ctrl_proc_code, string proc_status, string name)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();          

            string nameUTF = "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(name)).Replace("-", "%");
            //获取人员信息
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "search_key=" + nameUTF + "&groups=&groupType=PI&call=GetUserRankInfo&page_size=10&page_index=0&get_total=true&key_id=user_id&id=&sort=";
            string content = Surfing(uri, cookie, postData);
            string userid = "";
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (var item in table)
                    {
                        userid = item["user_id"].ToString();
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            uri = "http://www.acip.vip/ajax/case_info.ashx";
            //可处理新申请
            postData = "ctrl_proc_code="+ ctrl_proc_code + "&proc_status="+ proc_status + "&user_id=" + userid + "&search_key=&call=GetUserCaseList&page_size=-1&page_index=0&get_total=true&key_id=proc_id&id=&sort=legal_due_date+asc%2C+int_first_date+asc";
            content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AttorneySeries = row["volume"].ToString();//我方案号
                        caseInfo.CasedocumentName = row["case_name"].ToString();//案件名称
                        caseInfo.CaseID = row["case_id"].ToString();//
                        caseInfo.ClientName = row["customer_name"].ToString();//客户名称
                        caseInfo.AppType = row["apply_type_name"].ToString();//申请类型

                        taskInfo.Attorney = name;//承办人

                        taskInfo.TaskName = row["ctrl_proc"].ToString();//任务名称
                        taskInfo.TaskAttribute = row["ctrl_proc_property"].ToString();//任务属性
                        taskInfo.TaskID = row["proc_id"].ToString();//
                        taskInfo.ProcessStage = row["proc_status_name"].ToString();//处理状态
                        if (row["int_first_date"].ToString() != "")
                        {
                            taskInfo.FirstVirsionDeadlineInternal = Convert.ToDateTime(row["int_first_date"]);
                        }

                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            return caseInfos;
        }

        public List<CaseInfo> SearchBySingleString(string searchString)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();
            caseInfos = GetCaseByAttorneySeries(searchString, cookie);
            caseInfos =  caseInfos.Concat(GetCaseByName(searchString, cookie)).ToList();
            return caseInfos;
        }

        private List<CaseInfo> GetCaseByAttorneySeries(string searchString, string cookie_str)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();

            //call: GetSearchForm
            //search_form_code: case_search
            //is_history: true
            //is_export: true

            string uri = "http://www.acip.vip/ajax/search.ashx";
            string postData = "param=%7B%22search_range_default%22%3Afalse%2C%22C6%22%3A%22" + searchString + "%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=30&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            string content = Surfing(uri, cookie_str, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        //caseInfo.AppSerialNum = row["app_no"].ToString();
                        caseInfo.AppType = row["C2"].ToString();
                        caseInfo.AttorneySeries = row["C6"].ToString();
                        caseInfo.CasedocumentName = row["C9"].ToString();
                        caseInfo.CaseID = row["case_id"].ToString();
                        caseInfo.ClientName = row["CC3"].ToString();
                        //caseInfo.ClientSeries = row["app_no"].ToString();
                        taskInfo.Attorney = row["P3"].ToString();
                        //taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        taskInfo.TaskName = row["P1"].ToString();
                        taskInfo.TaskAttribute = row["P29"].ToString();
                        taskInfo.TaskID = row["proc_id"].ToString();
                        //taskInfo.OfficalDeadline = (DateTime)row["P14"];
                        taskInfo.ProcessStage = row["P5"].ToString();
                        if (row["P14"].ToString() != "")
                        {
                            taskInfo.OfficalDeadline = (DateTime)row["P14"];
                        }
                        //if (row["int_first_date"].ToString() != "")
                        //{
                        //    taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        //}
                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return caseInfos;
        }

        private List<CaseInfo> GetCaseByName(string name, string cookie_str)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();

            //param: { "search_range_default":false,"P3_name":"舒丁","P3":"c73ebcba-a311-4cdf-92c8-270046937b01"}
            //search_type: proc
            //search_form_code: case_search
            //call: SearchCase
            //page_size: 10
            //page_index: 0
            //get_total: true
            //key_id: proc_id
            //id: 
            //sort: create_time desc

            string departmentcode = GetUserDepartmentCode(name,cookie_str);
            name ="%" + BitConverter.ToString(Encoding.UTF8.GetBytes(name)).Replace("-","%");

            string uri = "http://www.acip.vip/ajax/search.ashx";
            string postData = "param=%7B%22search_range_default%22%3Afalse%2C%22P3_name%22%3A%22"+name+"%22%2C%22P3%22%3A%22"+departmentcode+"%22%7D&search_type=proc&search_form_code=case_search&call=SearchCase&page_size=30&page_index=0&get_total=true&key_id=proc_id&id=&sort=create_time+desc";
            string content = Surfing(uri, cookie_str, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        //caseInfo.AppSerialNum = row["app_no"].ToString();
                        caseInfo.AppType = row["C2"].ToString();
                        caseInfo.AttorneySeries = row["C6"].ToString();
                        caseInfo.CasedocumentName = row["C9"].ToString();
                        caseInfo.CaseID = row["case_id"].ToString();
                        caseInfo.ClientName = row["CC3"].ToString();
                        //caseInfo.ClientSeries = row["app_no"].ToString();
                        taskInfo.Attorney = row["P3"].ToString();
                        //taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        taskInfo.TaskName = row["P1"].ToString();
                        taskInfo.TaskAttribute = row["P29"].ToString();
                        taskInfo.TaskID = row["proc_id"].ToString();
                        //taskInfo.OfficalDeadline = (DateTime)row["P14"];
                        taskInfo.ProcessStage = row["P5"].ToString();
                        if (row["P14"].ToString() != "")
                        {
                            taskInfo.OfficalDeadline = (DateTime)row["P14"];
                        }
                        //if (row["int_first_date"].ToString() != "")
                        //{
                        //    taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        //}
                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }

            return caseInfos;
        }

        private string GetUserDepartmentCode(string name,string cookie_str)
        {
            string departmentcode = "";
            //Call: GetUserInfo
            //search_form_code: case_search
            //is_charge: 
            //search_key: 舒丁
            if (name != "")
            {
                name = "%" + BitConverter.ToString(Encoding.UTF8.GetBytes(name)).Replace("-", "%");
            }
            else
            {
                MessageBox.Show("请输入关键词");
            }
            string uri = "http://www.acip.vip/ajax/Common.ashx";
            string postData = "Call=GetUserInfo&search_form_code=case_search&is_charge=&search_key="+ name;
            string content = Surfing(uri, cookie_str, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["UserList"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    JObject row = (JObject)table.FirstOrDefault();
                    departmentcode = row["id"].ToString();
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return departmentcode;
        }

        public string GetUserID()
        {
            string uri = "http://www.acip.vip/ajax/common.ashx";
            string postData = "call=GetMyData";
            string content = Surfing(uri, cookie, postData);
            string userid = "";
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["ALIST"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        userid = row["user_id"].ToString();
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return userid;
        }

        private List<CaseInfo> Get_Case(string type, string cookie_str)
        {
            List<CaseInfo> caseInfos = new List<CaseInfo>();
            //ctrl_proc_code: oa/app/other (type)
            //proc_status: 
            //search_key:
            //reloadpage: true
            //call: GetMyCaseList
            //page_size: 10
            //page_index: 0
            //get_total: true
            //key_id: proc_id
            //id: 
            //sort: int_first_date asc, cus_first_date asc, legal_due_date asc
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "ctrl_proc_code=" + type + "&proc_status=&search_key=&reloadpage=true&call=GetMyCaseList&page_size=10&page_index=0&get_total=true&key_id=proc_id&id=&sort=int_first_date+asc%2C+cus_first_date+asc%2C+legal_due_date+asc";
            string content = Surfing(uri, cookie_str, postData);
            if (!content.Contains("失效"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        CaseInfo caseInfo = new CaseInfo();
                        TaskInfo taskInfo = new TaskInfo();

                        caseInfo.AppSerialNum = row["app_no"].ToString();
                        caseInfo.AppType = row["apply_type_name"].ToString();
                        caseInfo.AttorneySeries = row["volume"].ToString();
                        caseInfo.CasedocumentName = row["case_name"].ToString();
                        caseInfo.CaseID = row["case_id"].ToString();
                        caseInfo.ClientName = row["customer_name"].ToString();
                        caseInfo.ClientSeries = row["app_no"].ToString();
                        taskInfo.Attorney = row["app_no"].ToString();
                        taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        taskInfo.TaskName = row["ctrl_proc"].ToString();
                        taskInfo.TaskAttribute = row["ctrl_proc_property"].ToString();
                        taskInfo.TaskID = row["proc_id"].ToString();
                        //taskInfo.OfficalDeadline = (DateTime)row["legal_due_date"];
                        taskInfo.ProcessStage = row["proc_status_name"].ToString();
                        taskInfo.Attorney = row["app_no"].ToString();
                        taskInfo.Attorney = row["app_no"].ToString();
                        taskInfo.Attorney = row["app_no"].ToString();
                        if (row["legal_due_date"].ToString() != "")
                        {
                            taskInfo.OfficalDeadline = (DateTime)row["legal_due_date"];
                        }
                        if (row["int_first_date"].ToString() != "")
                        {
                            taskInfo.FirstVirsionDeadlineInternal = (DateTime)row["int_first_date"];
                        }
                        caseInfo.taskInfos = new List<TaskInfo>();
                        caseInfo.taskInfos.Add(taskInfo);
                        caseInfos.Add(caseInfo);
                    }
                }                
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return caseInfos;
        }

        public  List<Department> GetDepartmentList()
        {
            List<Department> departments = new List<Department>();
            string uri = "http://www.acip.vip/ajax/case_info.ashx";
            string postData = "call=GetInDeptInfo";
            string content = Surfing(uri, cookie, postData);
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["Result"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        Department department = new Department();
                        department.Name = row["full_name"].ToString();
                        department.ID = row["dept_id"].ToString();
                        department.ParentID = row["parent_id"].ToString();
                        departments.Add(department);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return departments;
        }

        public List<Bill> GetBill(Department department, string year, string month, string cookie_str)
        {
            string uri = "http://www.acip.vip/ajax/bill_info.ashx";
            //string cookie_str = "UM_distinctid=16788d9cef9fd-0bcc9649da0e84-6313363-384000-16788d9cefd2c6; CNZZDATA1271442956=358702909-1544188911-null%7C1545309842; Hm_lvt_f5df380d5163c1cc4823c8d33ec5fa49=1545656171,1546693742,1546778220,1547003236; Hm_lvt_82131f194bfafb51664235f31934ebe0=1546693806,1547003365; iplatform1.0=user_name=H00669; ASP.NET_SessionId=wp0nsp2l5s5xl2s33ddm2fnp; Hm_lvt_bfc6c23974fbad0bbfed25f88a973fb0=1558523577,1559140092,1559220113,1559346861; acip.iplatform=652C40812A5FE2E60672AB0149118789104C146080C57E0B82FE6E320212EA1D5A6C950AA95CAF7AF7BAD2D1E1A6961F28C3EAAFEABA6FD6DFAE70416E6A653D049DFEAF855FED8F69EA88AF329DA62CFD41332723E40F5AE10849FC78304AC3E29C76D20C3C4C9398D6F13CCCD115726EC87804186C1A66EC9AE57D215C483C1A09348DBAD68E50546FBEE0846D77ABF5F7286B0A76D3E4E4C6B27ECEA68C75215B02FE96EC981B3142EA53409ACF5AD901DB5193BC10FE910AAB13CA1FE407; Hm_lpvt_bfc6c23974fbad0bbfed25f88a973fb0=1559361125";
            //string postData = string.Format("userid ={0}&password={1}","guset","123");
            //string postData = "dept_id=75926ed8-2f2f-4011-b206-6dc36a8632d0&year=2019&month=5&search_key=&call=GetDeptBonusList&page_size=10&page_index=0&get_total=true&key_id=id&id=&sort=cn_name+asc";
            string postData = string.Format("dept_id={0}&year={1}&month={2}&search_key={3}&call={4}&page_size={5}&page_index={6}&get_total={7}&key_id={8}&id={9}&sort={10}",
                                                department.ID, year, month, "", "GetDeptBonusList", "20", "0", "true", "id", "", "cn_name+asc");
            string content = Surfing(uri, cookie, postData);

            List<Bill> bills = new List<Bill>();
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "null" && table_str !="")
                {
                    JArray table = JArray.Parse(table_str);
                    Bill billTotal = new Bill();
                    billTotal.Name = "总计";
                    foreach (JObject row in table)
                    {
                        Bill bill = new Bill();
                        bill.Name = row["cn_name"].ToString();
                        if (row["real_point"].ToString() != "")
                        {
                            bill.CN_Point += (double)row["real_point"];
                            billTotal.CN_Point += (double)row["real_point"];
                        }
                        if (row["f_real_point"].ToString() != "")
                        {
                            bill.F_Point += (double)row["f_real_point"];
                            billTotal.F_Point += (double)row["f_real_point"];
                        }
                        bill.Level = row["cn_grade"].ToString();
                        bill.Zone = department.Name;

                        bills.Add(bill);
                    }
                    bills.Add(billTotal);
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return bills;
        }

        public List<FileViewModel> GetFileInfos(string proc_id)
        {
            List<FileViewModel> fileinfos = new List<FileViewModel>();
            string uri = "http://www.acip.vip/ajax/common.ashx";
            string postData = "obj_id="+ proc_id + "&call=GetFileListById&page_size=-1&page_index=0&get_total=true&key_id=file_id&id=&sort=";
            string content = Surfing(uri, cookie, postData);

            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "")
                {
                    JArray table = JArray.Parse(table_str);
                    foreach (JObject row in table)
                    {
                        FileViewModel fileinfo = new FileViewModel();
                        fileinfo.FileName = row["file_name"].ToString();
                        fileinfo.FileID = row["file_id"].ToString();
                        fileinfo.FileDescribe = row["desc_name"].ToString();
                        fileinfo.UploadUser = row["user_name"].ToString();
                        fileinfo.UploadTime = row["create_time_ss"].ToString();
                        fileinfos.Add(fileinfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("登录过期，请重新登录", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return fileinfos;
        }

        private string Surfing(string uri, string cookie_str,string postData)
        {
            byte[] data = Encoding.UTF8.GetBytes(postData);
            HttpWebRequest request;
            HttpWebResponse response;
            request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("Cookie", cookie_str);
            request.ContentLength = data.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            return reader.ReadToEnd(); ;
        }

        class DownloadFileInfo
        {
            public string FileCode { get; set; }
            public string FileSize { get; set; }
        }
        private static DownloadFileInfo GetFileCode(string uri, string cookie_str, string postData)
        {
            //string uri = "http://www.acip.vip/ajax/case_info.ashx";
            //string cookie_str = "ASP.NET_SessionId=qs1sy510n455ttl4rmhclp50; Hm_lvt_bfc6c23974fbad0bbfed25f88a973fb0=1559222117; iplatform1.0=user_name=H00669; acip.iplatform=26245FF9043D445483965AAB43CDFF8807F54896ED7ED157EF14853E6D47048849EF0FF3534671E047D860505C4239714BD28129B3CD20A75812850878A7C68A42BC1FC7D1247B003045338FE78DC3AACDAC43EF3E2630521095D04B56DBA262203A2CA649104DD27E7C3ED550E426CCEA1DBCEDD84DB226D76B92D2FC9E3721AF3EDDC1DE193CF12EF1EE5AA3EDC06D358E659C840B34588F3D7CB13EAE720C57201FEE2FD8966BADC855A47836FEA5E0392D3D850A1B392AA0B81D6724A16F; Hm_lpvt_bfc6c23974fbad0bbfed25f88a973fb0=1559222135";
            //string postData = string.Format("userid ={0}&password={1}","guset","123");
            //string postData = "call=ExportCaseUserStatusList&dept_id=&search_key=";

            DownloadFileInfo downloadFileInfo = new DownloadFileInfo();
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request;
            HttpWebResponse response;
            request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
            request.Headers.Add("Cookie", cookie_str);
            request.ContentLength = data.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            response = (HttpWebResponse)request.GetResponse();


            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            string content = reader.ReadToEnd();

            JObject jo = (JObject)JsonConvert.DeserializeObject(content);
            downloadFileInfo.FileCode = jo["guid"].ToString();
            downloadFileInfo.FileSize = jo["file_size"].ToString();

            return downloadFileInfo;
        }

        public static bool HttpDownload(string uri, string postData, string fileName, string cookie_str)
        {
            try
            {
                DownloadFileInfo downloadFileInfo = GetFileCode(uri, cookie_str, postData);
                string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string filePath = exepath + "MyData\\" + fileName;
                string fileUri = "http://www.acip.vip/ajax/file_handler.ashx?browser=Chrome&file_code=" + downloadFileInfo.FileCode + "&file_type=3";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(fileUri) as HttpWebRequest;
                //发送请求并获取相应回应数据
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
                request.Headers.Add("Cookie", cookie_str);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                byte[] bArr = new byte[1024];
                int iTotalSize = 0;
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    iTotalSize += size;
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                fs.Close();
                responseStream.Close();


                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(filePath);
                Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
                Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["sheet1"];

                wsh.SaveAs(filePath);
                wbk.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public static void DownloadTaskFile(string uri, string postData, string filePath, string cookie_str)
        {
            try
            {
                DownloadFileInfo downloadFileInfo = GetFileCode(uri, cookie_str, postData);
                string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string fileUri = "http://218.17.24.82:88/file_service/file_handler.ashx?browser=Chrome&file_code=" + downloadFileInfo.FileCode + "&file_size="+downloadFileInfo.FileSize+"&file_type=0";
                //http://218.17.24.82:88/file_service/file_handler.ashx?browser=Chrome&file_code=ff39c627-fdeb-4929-b71a-04432a56943d&file_size=356073&file_type=0
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(fileUri) as HttpWebRequest;
                //发送请求并获取相应回应数据
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
                request.Headers.Add("Cookie", cookie_str);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                byte[] bArr = new byte[1024];
                int iTotalSize = 0;
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    iTotalSize += size;
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                fs.Close();
                responseStream.Close();


                //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                //Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                //Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(filePath);
                //Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
                //Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["sheet1"];

                //wsh.SaveAs(filePath);
                //wbk.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
