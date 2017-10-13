using ConsoleConvertExcelApp.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.util;
using WindowsFormsApplication1.entity;

namespace WindowsFormsApplication1.util
{
    public class HttpUtil
    {

        public static PdfData getPdfStreamData()
        {
            WebRequest wRequest = WebRequest.Create(PathUtil.selectUrl);
            wRequest.Method = "GET";
            wRequest.ContentType = "text/html;charset=UTF-8";
            WebResponse wResponse = wRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            string str = reader.ReadToEnd();   //url返回的值  
            Console.WriteLine(str);
            PdfData _list = null;
            if (str != null && !str.Equals("")) {
                //String jsonText = "{\"data\":[{\"id\":6014621,\"doc_id\":1482324,\"doc_type\":2,\"pdf_path\":\"luobo/2017/09/07/0000000000000j2gb1.pdf\",\"excel_flag\":0},{\"id\":6014621,\"doc_id\":14812324,\"doc_type\":2,\"pdf_path\":\"luobo/2017/09/07/0000000000000j2gb1.pdf\",\"excel_flag\":0}]}";
                 _list = JsonConvert.DeserializeObject<PdfData>(str);
            }
            //LogHelper.WriteLog(typeof(HttpUtil), str);
            reader.Close();
            wResponse.Close();
            return _list;

        }

        public static Boolean updatePdfStreamData(List<PdfStream> pdfdata)
        {
            try { 
                //如果需要POST数据     
                string data =  JsonConvert.SerializeObject(pdfdata);

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PathUtil.updateUrl);
                req.Method = "POST";
                req.ContentType = "application/json";
                Stream reqstream = req.GetRequestStream();
                byte[] b = Encoding.ASCII.GetBytes(data);
                reqstream.Write(b,0,b.Length);
                StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
                string result = responseReader.ReadToEnd();   //url返回的值
                responseReader.Close();
                reqstream.Close();
                Console.WriteLine(result);
                if (result != null && result.Length > 0)
                {
                    ResultEntity re = JsonConvert.DeserializeObject<ResultEntity>(result);
                    if (re.success && re.count >= 1) {
                        return true;
                    }
                    return false;
                }
                else {
                    return false;
                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static PdfData getPdfStreamDataByLimit(int limit)
        {
            //如果需要POST数据     
            Json j = new Json();
            j.limit = limit;
            string data = JsonConvert.SerializeObject(j);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PathUtil.selectUrl);
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream reqstream = req.GetRequestStream();
            byte[] b = Encoding.ASCII.GetBytes(data);
            reqstream.Write(b, 0, b.Length);
            StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
            string str = responseReader.ReadToEnd();   //url返回的值
            Console.WriteLine(str);
            PdfData _list = null;
            if (str != null && !str.Equals(""))
            {
                _list = JsonConvert.DeserializeObject<PdfData>(str);
            }
            //LogHelper.WriteLog(typeof(HttpUtil), str);
            responseReader.Close();
            reqstream.Close();
            return _list;
        }
    }
}
