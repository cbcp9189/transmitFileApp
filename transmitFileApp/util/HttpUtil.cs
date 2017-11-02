using ConsoleConvertExcelApp.entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.constant;
using transmitFileApp.entity;
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

        public static PdfData getPdfStreamDataByPipeLining(int limit)
        {
            StringBuilder url = new StringBuilder(PathUtil.selectUrl);
            url.Append("?programName=");
            url.Append(SystemConstant.PROGRAMNAME);
            url.Append("&limit=");
            url.Append(limit);
            Console.WriteLine(url.ToString());
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
            req.Method = "GET";
            req.ContentType = "application/json";
            StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
            string str = responseReader.ReadToEnd();   //url返回的值
            Console.WriteLine(str);
            PdfStreamObj _list = null;
            if (str != null && !str.Equals(""))
            {
                _list = JsonConvert.DeserializeObject<PdfStreamObj>(str);
            }
            //LogHelper.WriteLog(typeof(HttpUtil), str);
            responseReader.Close();
            PdfData pdfData = new PdfData();
            if (_list.data != null && _list.data.Count > 0)
            {

                foreach (PdfStreamInfo info in _list.data)
                {
                    PdfStream stream = new PdfStream();
                    parseStreamToStreamInfo(stream, info);
                    pdfData.data.Add(stream);
                }
            }
            return pdfData;
        }

        public static PdfData getPdfStreamDataByUserUpload(int limit)
        {
            StringBuilder url = new StringBuilder(PathUtil.userUploadUrl);
            url.Append("?programName=");
            url.Append(SystemConstant.PROGRAMNAME);
            url.Append("&docType=");
            url.Append(PathUtil.testDocType);
            url.Append("&limit=");
            url.Append(limit);
            Console.WriteLine("url-"+url);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
            req.Method = "GET";
            req.ContentType = "application/json";
            StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
            string str = responseReader.ReadToEnd();   //url返回的值
            Console.WriteLine(str);
            PdfStreamObj _list = null;
            if (str != null && !str.Equals(""))
            {
                _list = JsonConvert.DeserializeObject<PdfStreamObj>(str);
            }
            //LogHelper.WriteLog(typeof(HttpUtil), str);
            responseReader.Close();
            PdfData pdfData = new PdfData();
            if (_list.data != null && _list.data.Count > 0)
            {

                foreach (PdfStreamInfo info in _list.data)
                {
                    PdfStream stream = new PdfStream();
                    parseStreamToStreamInfo(stream, info);
                    pdfData.data.Add(stream);
                }
            }
            return pdfData;
        }

        public static void parseStreamToStreamInfo(PdfStream stream, PdfStreamInfo info)
        {
            stream.id = info.id;
            stream.doc_id = info.docId;
            stream.doc_type = info.docType;
            stream.pdf_path = info.pdfPath;
        }

        public static Boolean updatePdfStreamDataPipeLine(List<PdfStream> pdfdata)
        {
            try
            {
                RequestJson request = new RequestJson();

                if (pdfdata != null && pdfdata.Count > 0)
                {
                    parseStreamToRequestJson(pdfdata[0], request);
                }
                else
                {
                    return false;
                }

                string data = JsonConvert.SerializeObject(request);
                Console.WriteLine("update:"+data);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PathUtil.updateUrl);

                req.Timeout = 1 * 60 * 1000;
                req.Method = "POST";
                req.ContentType = "application/json";
                Stream reqstream = req.GetRequestStream();
                byte[] b = Encoding.ASCII.GetBytes(data);
                reqstream.Write(b, 0, b.Length);
                StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
                string result = responseReader.ReadToEnd();   //url返回的值
                responseReader.Close();
                reqstream.Close();
                Console.WriteLine(result);
                if (result != null && result.Length > 0)
                {
                    Response re = JsonConvert.DeserializeObject<Response>(result);
                    if (re.code == 0)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Boolean updatePdfStreamDataByPipeLineToMultiFlag(List<PdfStream> pdfdata)
        {
            try
            {
                RequestSuccessJson request = new RequestSuccessJson();

                if (pdfdata != null && pdfdata.Count > 0)
                {
                    parseStreamToRequestSuccessJson(pdfdata[0], request);
                }
                else
                {
                    return false;
                }

                string data = JsonConvert.SerializeObject(request);
                Console.WriteLine("send data-"+data);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PathUtil.multiFlagUrl);

                req.Timeout = 1 * 60 * 1000;
                req.Method = "POST";
                req.ContentType = "application/json";
                Stream reqstream = req.GetRequestStream();
                byte[] b = Encoding.ASCII.GetBytes(data);
                reqstream.Write(b, 0, b.Length);
                StreamReader responseReader = new StreamReader(req.GetResponse().GetResponseStream(), System.Text.Encoding.Default);
                string result = responseReader.ReadToEnd();   //url返回的值
                responseReader.Close();
                reqstream.Close();
                Console.WriteLine(result);
                if (result != null && result.Length > 0)
                {
                    Response re = JsonConvert.DeserializeObject<Response>(result);
                    if (re.code == 0)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void parseStreamToRequestJson(PdfStream stream, RequestJson json)
        {
            json.id = stream.id;
            json.isSuccess = stream.ocr_flag.ToString();
            json.programName = SystemConstant.PROGRAMNAME;
            json.errorCode = stream.ocr_flag.ToString();
        }

        public static void parseStreamToRequestSuccessJson(PdfStream stream, RequestSuccessJson json)
        {
            json.id = stream.id;
            json.programNames = new String[]{SystemConstant.PROGRAMNAME};
            json.pdfPath = stream.pdf_path;
        }
    }
}
