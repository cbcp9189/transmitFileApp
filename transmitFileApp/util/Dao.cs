using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace transmitFileApp.util
{
    public class Dao
    {
        public MySqlConnection getmysqlcon()
        {
            string M_str_sqlcon = "server=106.75.3.227;user id=root;password=az%ZCr1pCl;database=scrapy"; // 227scrapy服务器
            return new MySqlConnection(M_str_sqlcon);
        }


        public void insert(long pdfStreamId,int docType) {
            try
            {
                MySqlConnection con = getmysqlcon();
                con.Open();
                String sql = "INSERT ocr_monitor(pdfstream_id,doc_type) VALUES(@id,@type)";
                MySqlCommand mysqlcom = con.CreateCommand();
                mysqlcom.Parameters.AddWithValue("@id", pdfStreamId);
                mysqlcom.Parameters.AddWithValue("@type", docType);
                mysqlcom.CommandText = sql;
                mysqlcom.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("insert exception:"+ex.Message);
            }
            
        }

        public Boolean select(long pdfStreamId)
        {
            Boolean isExist = false;
            try
            {
                MySqlConnection con = getmysqlcon();
                con.Open();
                StringBuilder sql = new StringBuilder("SELECT * FROM ocr_monitor where pdfstream_id = ");
                sql.Append(pdfStreamId);
                MySqlCommand mysqlcom = new MySqlCommand(sql.ToString(), con);
                MySqlDataReader reader = mysqlcom.ExecuteReader();
               
                while (reader.Read())
                {
                    isExist = true;
                }
                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("insert exception:" + ex.Message);
            }
            return isExist;
        }


        public void update(long pdfStreamId, int doc_type, int status)
        {
            try{
                //更新pdf_stream表中的excel_flag和version字段
                StringBuilder sql = new StringBuilder("UPDATE ocr_monitor SET status= ");
                sql.Append(status);
                sql.Append(" WHERE pdfstream_id = ");
                sql.Append(pdfStreamId);
                sql.Append(" and doc_type = ");
                sql.Append(doc_type);
                MySqlConnection con = getmysqlcon();
                con.Open();
                MySqlCommand mysqlcom1 = new MySqlCommand(sql.ToString(), con);
                mysqlcom1.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("update exception:"+ex.Message);
            }
        }
    }
}
