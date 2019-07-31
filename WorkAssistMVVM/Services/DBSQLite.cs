using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WorkAssistMVVM.Services
{
    class DBSQLite
    {
        //定义连接对象
        static SQLiteConnection conn = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        static DBSQLite()
        {
            //实例化连接对象
            conn = new SQLiteConnection(GetConnectstring());
            if (conn != null)
            {
                //MessageBox.Show("数据库连接成功！");
            }
            else
            {
                MessageBox.Show(GetConnectstring() + "数据库无法访问！");
            }
        }

        /// <summary>
        /// 通过查询得到dataset
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static DataTable GetDataTableBySQL(string strSQL)
        {
            DataTable dt = new DataTable();
            try
            {
                conn.Open();//打开连接
                //定义适配器对象
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(strSQL, conn);
                //填充表
                adapter.Fill(dt);
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                conn.Close();//关闭连接
            }
            //返回dataset
            return dt;
        }

        public int ExcuteSQLite(string sql)
        {
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            int result = cmd.ExecuteNonQuery();
            conn.Close();
            return result;
        }


        private static string GetConnectstring()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration("WorkAssistMVVM.exe");
            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
            ConnectionStringSettings cs = section.ConnectionStrings[1];

            return cs.ConnectionString;
        }

        public List<string> GetHeaderByTableName(string tablename)
        {
            List<string> list = new List<string>();
            string sql = "PRAGMA  table_info('" + tablename + "')";
            conn.Open();
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                list.Add(reader[1].ToString());

            }
            reader.Close();
            conn.Close();
            return list;
        }

        public void Batch(string sql, List<SQLiteParameter[]> parameters)
        {
            conn.Open();
            SQLiteTransaction tran = conn.BeginTransaction();
            try
            {
                foreach (SQLiteParameter[] para in parameters)
                {
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    cmd.Parameters.AddRange(para);
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                tran.Dispose();
                conn.Close();

            }
        }
    }
}
