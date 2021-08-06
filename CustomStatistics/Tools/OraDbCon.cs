using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Windows.Forms;
namespace CustomStatistics
{
    public class OraDbCon
    {
        public string OraDbConn  = ""; //引用System.Configuration.dll


        public static DataTable GetDataTable(string sql,String db)
        {
            OracleConnection conn = new OracleConnection(db);
            try
            {
               
                DataTable MyDT = new DataTable();
                OracleCommand cmd = new OracleCommand(sql, conn)
                {
                    InitialLONGFetchSize = -1
                };
                using (OracleDataAdapter myoda = new OracleDataAdapter(cmd))
                {
                    myoda.Fill(MyDT);
                }
                return MyDT;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + sql, "获取DataTable失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        ///  添加 修改 删除
        /// </summary>
        /// <param name="typeIu"></param>
        /// <returns></returns>
        public int InsOrUpMsg(string sqlStr)
        {
            OracleConnection conn = new OracleConnection(OraDbConn);
            OracleCommand comm;
            int ret = 0;
            try
            {
                conn.Open();
                comm = new OracleCommand(sqlStr, conn);
                OracleDataAdapter productsAdapter;
                productsAdapter = new OracleDataAdapter();
                comm.CommandType = CommandType.Text;
                ret = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                conn.Dispose();
                conn.Close();
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                conn.Dispose();
                conn.Close();
            }
            return ret;
        }


        //测试数据库是否连接成功 
        public bool TestConnTrue()
        {
            try
            {
                OracleConnection conn = new OracleConnection(OraDbConn);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
                throw;
                
            }
        }
    }
}