using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using WorkAssistMVVM.ViewModels;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Prism.Regions;
using System.Windows;

namespace WorkAssistMVVM.ViewModels
{
    class UCDonePointViewModel:BindableBase, INavigationAware
    {
        private string _title = "递交权值";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private static string Cookie { get; set; }

        private string year;
        public string Year
        {
            get { return year; }
            set { SetProperty(ref year, value); }
        }

        private string month;
        public string Month
        {
            get { return month; }
            set { SetProperty(ref month, value); }
        }

        private ObservableCollection<DonePointViewModel> pekingDonePoints;
        public ObservableCollection<DonePointViewModel> PekingDonePoints
        {
            get { return pekingDonePoints; }
            set { SetProperty(ref pekingDonePoints, value); }
        }

        private ObservableCollection<DonePointViewModel> shanghaiDonePoints;
        public ObservableCollection<DonePointViewModel> ShanghaiDonePoints
        {
            get { return shanghaiDonePoints; }
            set { SetProperty(ref shanghaiDonePoints, value); }
        }

        private ObservableCollection<DonePointViewModel> guangzhouDonePoints;
        public ObservableCollection<DonePointViewModel> GuangzhouDonePoints
        {
            get { return guangzhouDonePoints; }
            set { SetProperty(ref guangzhouDonePoints, value); }
        }
        private ObservableCollection<DonePointViewModel> shenzhenDonePoints;
        public ObservableCollection<DonePointViewModel> ShenzhenDonePoints
        {
            get { return shenzhenDonePoints; }
            set { SetProperty(ref shenzhenDonePoints, value); }
        }
        private ObservableCollection<DonePointViewModel> changshaDonePoints;
        public ObservableCollection<DonePointViewModel> ChangshaDonePoints
        {
            get { return changshaDonePoints; }
            set { SetProperty(ref changshaDonePoints, value); }
        }
        private ObservableCollection<DonePointViewModel> suzhouDonePoints;
        public ObservableCollection<DonePointViewModel> SuzhouDonePoints
        {
            get { return suzhouDonePoints; }
            set { SetProperty(ref suzhouDonePoints, value); }
        }
        private ObservableCollection<DonePointViewModel> xiAnDonePoints;
        public ObservableCollection<DonePointViewModel> XiAnDonePoints
        {
            get { return xiAnDonePoints; }
            set { SetProperty(ref xiAnDonePoints, value); }
        }

        public DelegateCommand<object[]> QueryDonePointsCommand { get; private set; }

        public UCDonePointViewModel()
        {
            Year = DateTime.Now.Year.ToString();
            Month = DateTime.Now.Month.ToString();
            QueryDonePointsCommand = new DelegateCommand<object[]>(QueryDonePoints);
        }

        private void QueryDonePoints(object[] parameters)
        {
            DownloadBill(Cookie, Year, Month);
        }
        private void DownloadBill(string cookie_str, string year, string month)
        {
            string[] deptIDs = { "75926ed8-2f2f-4011-b206-6dc36a8632d0", "5f29f6e7-4015-4a2a-b4ab-8a0332b636d8", "2d7bd187-ad1f-47b9-940b-b8dcd86c942f",
                                 "2cf87c40-1e70-4ff7-9565-5f112e22446c", "69f3aa4e-7ce2-4b52-8b73-d5320f849069", "4821a871-783c-47fd-b208-49ab8863bc6b",
                                 "6f4f80a1-c4cd-4915-a393-95f418e8e375","b422e0f7-e21b-49dd-83de-5f162f3abfb8","bdd383d2-07e0-49f2-ab8b-4840ed97226e" };

            //string year = DateTime.Now.Year.ToString();
            //string month = (DateTime.Now.Month - 1).ToString();

            PekingDonePoints = new ObservableCollection<DonePointViewModel>();
            ShanghaiDonePoints = new ObservableCollection<DonePointViewModel>();
            GuangzhouDonePoints = new ObservableCollection<DonePointViewModel>();
            ShenzhenDonePoints = new ObservableCollection<DonePointViewModel>();
            ChangshaDonePoints = new ObservableCollection<DonePointViewModel>();
            SuzhouDonePoints = new ObservableCollection<DonePointViewModel>();
            XiAnDonePoints = new ObservableCollection<DonePointViewModel>();

            foreach (string deptID in deptIDs)
            {
                switch (deptID)
                {
                    case "75926ed8-2f2f-4011-b206-6dc36a8632d0":
                        //"深圳";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            ShenzhenDonePoints.Add(donepoint);
                        } 
                        break;
                    case "5f29f6e7-4015-4a2a-b4ab-8a0332b636d8":
                        //bill.Zone = "北京";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            PekingDonePoints.Add(donepoint);
                        }
                        break;
                    case "2d7bd187-ad1f-47b9-940b-b8dcd86c942f":
                        //bill.Zone = "上海";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            ShanghaiDonePoints.Add(donepoint);
                        }
                        break;
                    case "2cf87c40-1e70-4ff7-9565-5f112e22446c":
                        //bill.Zone = "广州";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            GuangzhouDonePoints.Add(donepoint);
                        }
                        break;
                    case "69f3aa4e-7ce2-4b52-8b73-d5320f849069":
                        //bill.Zone = "长沙";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            ChangshaDonePoints.Add(donepoint);
                        }
                        break;
                    case "4821a871-783c-47fd-b208-49ab8863bc6b":
                        //bill.Zone = "苏州";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            SuzhouDonePoints.Add(donepoint);
                        }
                        break;
                    case "6f4f80a1-c4cd-4915-a393-95f418e8e375":
                        //bill.Zone = "西安";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            XiAnDonePoints.Add(donepoint);
                        }
                        break;
                    case "b422e0f7-e21b-49dd-83de-5f162f3abfb8":
                        //bill.Zone = "广州";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            GuangzhouDonePoints.Add(donepoint);
                        }
                        break;
                    case "bdd383d2-07e0-49f2-ab8b-4840ed97226e":
                        //bill.Zone = "深圳";
                        foreach (DonePointViewModel donepoint in GetBill(deptID, year, month, cookie_str))
                        {
                            ShenzhenDonePoints.Add(donepoint);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        private ObservableCollection<DonePointViewModel> GetBill(string deptID, string year, string month, string cookie_str)
        {
            string uri = "http://www.acip.vip/ajax/bill_info.ashx";
            //string cookie_str = "UM_distinctid=16788d9cef9fd-0bcc9649da0e84-6313363-384000-16788d9cefd2c6; CNZZDATA1271442956=358702909-1544188911-null%7C1545309842; Hm_lvt_f5df380d5163c1cc4823c8d33ec5fa49=1545656171,1546693742,1546778220,1547003236; Hm_lvt_82131f194bfafb51664235f31934ebe0=1546693806,1547003365; iplatform1.0=user_name=H00669; ASP.NET_SessionId=wp0nsp2l5s5xl2s33ddm2fnp; Hm_lvt_bfc6c23974fbad0bbfed25f88a973fb0=1558523577,1559140092,1559220113,1559346861; acip.iplatform=652C40812A5FE2E60672AB0149118789104C146080C57E0B82FE6E320212EA1D5A6C950AA95CAF7AF7BAD2D1E1A6961F28C3EAAFEABA6FD6DFAE70416E6A653D049DFEAF855FED8F69EA88AF329DA62CFD41332723E40F5AE10849FC78304AC3E29C76D20C3C4C9398D6F13CCCD115726EC87804186C1A66EC9AE57D215C483C1A09348DBAD68E50546FBEE0846D77ABF5F7286B0A76D3E4E4C6B27ECEA68C75215B02FE96EC981B3142EA53409ACF5AD901DB5193BC10FE910AAB13CA1FE407; Hm_lpvt_bfc6c23974fbad0bbfed25f88a973fb0=1559361125";
            //string postData = string.Format("userid ={0}&password={1}","guset","123");
            //string postData = "dept_id=75926ed8-2f2f-4011-b206-6dc36a8632d0&year=2019&month=5&search_key=&call=GetDeptBonusList&page_size=10&page_index=0&get_total=true&key_id=id&id=&sort=cn_name+asc";
            string postData = string.Format("dept_id={0}&year={1}&month={2}&search_key={3}&call={4}&page_size={5}&page_index={6}&get_total={7}&key_id={8}&id={9}&sort={10}",
                                                deptID, year, month, "", "GetDeptBonusList", "20", "0", "true", "id", "", "cn_name+asc");

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

            ObservableCollection<DonePointViewModel> bills = new ObservableCollection<DonePointViewModel>();
            string content = reader.ReadToEnd();
            if (!content.Contains("登录信息失效，请重新登陆！"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(content);
                string table_str = jo["table_rows"].ToString();
                if (table_str != "null")
                {
                    JArray table = JArray.Parse(table_str);
                    DonePointViewModel billTotal = new DonePointViewModel();
                    billTotal.Name = "总计";
                    foreach (JObject row in table)
                    {
                        DonePointViewModel bill = new DonePointViewModel();
                    
                        bill.Name = row["cn_name"].ToString();
                        //bill.CN_Point = (double)row["real_point"];
                        if (row["real_point"].ToString() != "")
                        {
                            bill.DonePoint += (double)row["real_point"];
                        }

                        if (row["f_real_point"].ToString() != "")
                        {
                            bill.DonePoint += (double)row["f_real_point"];
                        }
                        billTotal.DonePoint += bill.DonePoint;
                        bill.Level = row["cn_grade"].ToString();

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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Cookie = navigationContext.Parameters["cookie"] as string;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
