using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KPI.Services
{
    public class HttpDataService : ICaseQueryService
    {
        private string cookie;

        public HttpDataService(string cookiestring)
        {
            cookie = cookiestring;
        }

        public List<Bill> GetBill(Department department, string year, string month)
        {
            List<Bill> bills = new List<Bill>();

            string uri = "http://www.acip.vip/ajax/bill_info.ashx";
            //string cookie_str = "UM_distinctid=16788d9cef9fd-0bcc9649da0e84-6313363-384000-16788d9cefd2c6; CNZZDATA1271442956=358702909-1544188911-null%7C1545309842; Hm_lvt_f5df380d5163c1cc4823c8d33ec5fa49=1545656171,1546693742,1546778220,1547003236; Hm_lvt_82131f194bfafb51664235f31934ebe0=1546693806,1547003365; iplatform1.0=user_name=H00669; ASP.NET_SessionId=wp0nsp2l5s5xl2s33ddm2fnp; Hm_lvt_bfc6c23974fbad0bbfed25f88a973fb0=1558523577,1559140092,1559220113,1559346861; acip.iplatform=652C40812A5FE2E60672AB0149118789104C146080C57E0B82FE6E320212EA1D5A6C950AA95CAF7AF7BAD2D1E1A6961F28C3EAAFEABA6FD6DFAE70416E6A653D049DFEAF855FED8F69EA88AF329DA62CFD41332723E40F5AE10849FC78304AC3E29C76D20C3C4C9398D6F13CCCD115726EC87804186C1A66EC9AE57D215C483C1A09348DBAD68E50546FBEE0846D77ABF5F7286B0A76D3E4E4C6B27ECEA68C75215B02FE96EC981B3142EA53409ACF5AD901DB5193BC10FE910AAB13CA1FE407; Hm_lpvt_bfc6c23974fbad0bbfed25f88a973fb0=1559361125";
            //string postData = string.Format("userid ={0}&password={1}","guset","123");
            //string postData = "dept_id=75926ed8-2f2f-4011-b206-6dc36a8632d0&year=2019&month=5&search_key=&call=GetDeptBonusList&page_size=10&page_index=0&get_total=true&key_id=id&id=&sort=cn_name+asc";
            string postData = string.Format("dept_id={0}&year={1}&month={2}&search_key={3}&call={4}&page_size={5}&page_index={6}&get_total={7}&key_id={8}&id={9}&sort={10}",
                                                department.ID, year, month, "", "GetDeptBonusList", "20", "0", "true", "id", "", "cn_name+asc");
            string content = Surfing(uri, cookie, postData);

            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "null" && table_str != "")
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

        public List<TaskInfo> GetDoneList(List<string> names, DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> donelist = new List<TaskInfo>();



            return donelist;
        }

        public  List<TaskInfo> GetDoneList(DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> donelist = new List<TaskInfo>();

            DownloadDonelist(startDate.Date.ToString("yyyy-MM-dd"), endDate.Date.ToString("yyyy-MM-dd"));

            string exepath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string filePath = exepath + "MyData\\donelist.xlsx";
            DataTable dt = Read_Excel(filePath);

            foreach (DataRow dr in dt.Rows)
            {
                TaskInfo task = new TaskInfo();
                task.AttorneySeries = dr["我方文号"].ToString();
                task.CasedocumentName = dr["案件名称"].ToString();
                task.ClaimNum = (int)dr["权项总数"];
                task.AppType = dr["申请类型"].ToString();
                task.TaskName = dr["任务名称"].ToString();
                task.Attorney= dr["承办人"].ToString();
                task.Department = dr["承办人所在部门(导出)"].ToString();
                task.ClientName = dr["客户名称"].ToString();
                task.DoneDate = (DateTime)dr["送官方日期"];

                donelist.Add(task);
            }

            return donelist;
        }

        public List<TaskInfo> GetFirstVersionList(List<string> names, DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> firstVersionlist = new List<TaskInfo>();



            return firstVersionlist;
        }

        public List<TaskInfo> GetFirstVersionList(DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> firstVersionlist = new List<TaskInfo>();



            return firstVersionlist;
        }

        public List<TaskInfo> GetGrantedList(List<string> names, DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> grantedlist = new List<TaskInfo>();



            return grantedlist;
        }

        public List<TaskInfo> GetGrantedList(DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> grantedlist = new List<TaskInfo>();



            return grantedlist;
        }

        public List<TaskInfo> GetRejectedList(List<string> names, DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> rejectedlist = new List<TaskInfo>();



            return rejectedlist;
        }

        public List<TaskInfo> GetRejectedList(DateTime startDate, DateTime endDate)
        {
            List<TaskInfo> rejectedlist = new List<TaskInfo>();



            return rejectedlist;
        }

        private static string Surfing(string uri, string cookie_str, string postData)
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

        public List<Department> GetDepartmentList()
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
                System.Windows.MessageBox.Show("登录过期，请重新登录", "出错了", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return departments;
        }

        //下载递交清单 日期格式：2019-01-01
        private  void DownloadDonelist(string startdate, string enddate)
        {
            string uri = "http://www.acip.vip/ajax/search.ashx";
            // call: ExportSearchList
            // param:{ "search_range_default":false,"C14":"II","P1":"89EEDA70-E934-41C3-8858-39A909DB3EF9","P16":"false;2019-01-01;2019-03-31"}
            // export_type: 0
            // search_form_code: case_search
            // search_type:proc
            // user_export_id:1603d1ae - c68f - 4a86 - a0ed - 5e5c84bdb0b4
            // is_export_related:
            //----------------------------------------------------------------------------------------------------------------------------------
            //string startdate = "2019-01-01";
            //string enddate = "2019-03-31";
            string param = "%7B%22search_range_default%22%3Afalse%2C%22C14%22%3A%22II%22%2C%22P1%22%3A%2289EEDA70-E934-41C3-8858-39A909DB3EF9%22%2C%22P16%22%3A%22false%3B" + startdate + "%3B" + enddate + "%22%7D";
            string postData = string.Format("call={0}&param={1}&export_type={2}&search_form_code={3}&search_type={4}&user_export_id={5}&is_export_related={6}",
                                            "ExportSearchList", param, "0", "case_search", "proc", "c2f23f86-01d3-41ac-a6e4-f0be8fc4e5bc", "");
            string fileName = "donelist.xlsx";

            HttpDownload(uri, postData, fileName, cookie);

            MessageBox.Show("下载成功!");
        }

        class DownloadFileInfo
        {
            public string FileCode { get; set; }
            public string FileSize { get; set; }
        }
        private  DownloadFileInfo GetFileCode(string uri, string cookie_str, string postData)
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
            if(jo["file_size"] != null) downloadFileInfo.FileSize = jo["file_size"].ToString();

            return downloadFileInfo;
        }
        private  bool HttpDownload(string uri, string postData, string fileName, string cookie_str)
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

                //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                //Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                //Microsoft.Office.Interop.Excel.Workbook wbk = wbks.Add(filePath);
                //Microsoft.Office.Interop.Excel.Sheets sheets = wbk.Sheets;
                //Microsoft.Office.Interop.Excel.Worksheet wsh = sheets["sheet1"];

                //wsh.SaveAs(filePath);
                //wbk.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //读取excel到datatable里
        public static DataTable Read_Excel(string excelPath)
        {
            DataTable dt = new DataTable();
            try
            {
                string fileExt = System.IO.Path.GetExtension(excelPath);
                string connectionstring = "";

                if (fileExt == ".xls")
                {
                    connectionstring = "Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + excelPath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'";
                }
                else
                {
                    connectionstring = "Provider = Microsoft.ACE.OLEDB.12.0 ; Data Source =" + excelPath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1'";
                }

                OleDbConnection conn = new OleDbConnection(connectionstring);
                OleDbDataAdapter da = new OleDbDataAdapter("select * from [sheet1$]", conn);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return dt;
        }
    }
}
