using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WindowsFormsApplication1.entity;

namespace transmitFileApp.util
{
    class PathUtil
    {
        public static String needOcrDestFilePath = "";
        public static String ocrSuccessDestFilePath = "";
        public static String selectUrl = "";
        public static String updateUrl = "";
        public static String tempSoucePath = "";
        public static String tempFinishPath = "";
        public static String multiFlagUrl = "";
        public static String userUploadUrl = "";
        public static String testDocType = "";
        public static String sendMsgPath = "";
        public static String serverName = "";
        public static List<PdfModel> mappingList = new List<PdfModel>();

        public static void getFilePath()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(@"config.xml");
                XmlNode xn = xmlDoc.SelectSingleNode("configs");
                XmlNodeList xnl = xn.ChildNodes;
                foreach (XmlNode xn1 in xnl)
                {
                    XmlElement xe = (XmlElement)xn1;
                    if (xe.Name.Equals("descPath"))
                    {
                        needOcrDestFilePath = xe.InnerText;
                    }
                    else if (xe.Name.Equals("ocrSuccessPath"))
                    {
                        ocrSuccessDestFilePath = xe.InnerText;
                    }
                    else if (xe.Name.Equals("selectUrl"))
                    {
                        selectUrl = xe.InnerText;
                    }
                    else if (xe.Name.Equals("updateUrl"))
                    {
                        updateUrl = xe.InnerText;
                    }
                    else if (xe.Name.Equals("tempSoucePath"))
                    {
                        tempSoucePath = xe.InnerText;
                    }
                    else if (xe.Name.Equals("tempFinishPath"))
                    {
                        tempFinishPath = xe.InnerText;
                    }
                    else if (xe.Name.Equals("multiFlagUrl"))
                    {
                        multiFlagUrl = xe.InnerText;
                    }
                    else if (xe.Name.Equals("userUploadUrl"))
                    {
                        userUploadUrl = xe.InnerText;
                    }
                    else if (xe.Name.Equals("docType"))
                    {
                        testDocType = xe.InnerText;
                    }
                    else if (xe.Name.Equals("smsUrl"))
                    {
                        sendMsgPath = xe.InnerText;
                    }
                    else if (xe.Name.Equals("serverName"))
                    {
                        serverName = xe.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(typeof(PathUtil), ex);
                Console.WriteLine(ex.Message);
            }
            
        }

        public static void getPathList()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(@"PathFile.xml");
                XmlNode xn = xmlDoc.SelectSingleNode("pdflist");
                XmlNodeList xnl = xn.ChildNodes;
                foreach (XmlNode xn1 in xnl)
                {
                    PdfModel model = new PdfModel();
                    XmlElement xe = (XmlElement)xn1;
                    XmlNodeList xnl0 = xe.ChildNodes;
                    model.doctype = int.Parse(xnl0.Item(0).InnerText);
                    model.year = xnl0.Item(1).InnerText;
                    model.pdfPath = xnl0.Item(2).InnerText;
                    model.excelPath = xnl0.Item(3).InnerText;
                    model.excelHead = xnl0.Item(4).InnerText;
                    mappingList.Add(model);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(typeof(PathUtil), ex);
            }
        }

        //获取pdf绝对路径
        public static String getAbsolutePdfPath(String pdfPath, int type)
        {
            foreach (PdfModel mo in mappingList)
            {
                if (mo.doctype == type && pdfPath.Contains("2017/") && mo.year.Equals("2017") && pdfPath.Contains("GSGGFWB"))
                {
                    pdfPath = mo.pdfPath + pdfPath.Replace(mo.excelHead, "").Replace("cninfo", "");
                    break;
                }
                else if (mo.doctype == type && pdfPath.Contains("2016/") && mo.year.Equals("2016") && pdfPath.Contains("GSGGFWB"))
                {
                    pdfPath = mo.pdfPath + pdfPath.Replace(mo.excelHead, "").Replace("cninfo", "");
                    break;
                }
                else if (mo.doctype == type && !pdfPath.Contains("2016/") && !pdfPath.Contains("2017/") && mo.year.Equals("other") && pdfPath.Contains("GSGGFWB"))
                {
                    pdfPath = mo.pdfPath + pdfPath.Replace(mo.excelHead, "").Replace("cninfo", "");
                    break;
                }
                else if (mo.doctype == type && pdfPath.Contains("cninfo") && mo.year.Equals("cninfo"))
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("userupload"))
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("testuserupload"))
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("article"))  //微信
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("report") && pdfPath.Contains("report"))  //研报
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("luobo") && pdfPath.Contains("luobo"))  //萝卜
                {
                    pdfPath = mo.pdfPath + pdfPath.Replace(mo.excelHead, "");
                    break;
                }
                else if (mo.doctype == type && mo.year.Equals("hkexnews"))  //港股
                {
                    pdfPath = mo.pdfPath + pdfPath;
                    break;
                }

            }
            //LogHelper.WriteLog(typeof(PathUtil), pdfPath);
            return pdfPath;
        }

        public static void initPathData()
        {
            Console.WriteLine("start.................");
            getFilePath();  //获取需要ocr到具体文件夹的路径
            
            //删除needOrc和ocrsuccess文件夹中的数据

            DirectoryInfo needOcrDir = new DirectoryInfo(PathUtil.needOcrDestFilePath);
            if (!needOcrDir.Exists)
            {
                Directory.CreateDirectory(PathUtil.needOcrDestFilePath);
            }
            FileInfo[] needOcrFiles = needOcrDir.GetFiles();
            foreach (var item in needOcrFiles)
            {
                File.Delete(item.FullName);
            }
            DirectoryInfo successOrcDir = new DirectoryInfo(PathUtil.ocrSuccessDestFilePath);
            if (!successOrcDir.Exists)
            {
                Directory.CreateDirectory(PathUtil.ocrSuccessDestFilePath);
            }
            FileInfo[] successOrcFile = successOrcDir.GetFiles();
            foreach (var item in successOrcFile)
            {
                File.Delete(item.FullName);
            }

            DirectoryInfo sourceOcrDir = new DirectoryInfo(PathUtil.tempSoucePath);
            if (!sourceOcrDir.Exists)
            {
                Directory.CreateDirectory(PathUtil.tempSoucePath);
            }

            FileInfo[] souceOcrFiles = sourceOcrDir.GetFiles();
            foreach (var item in souceOcrFiles)
            {
                File.Delete(item.FullName);
            }

            //清空idmapping.txt
            TxtUtil.removeAllIdMapping();
        }
    }
}
