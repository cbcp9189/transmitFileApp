using ConsoleConvertExcelApp.entity;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.constant;
using transmitFileApp.entity;
using transmitFileApp.util;
using WindowsFormsApplication1;
using WindowsFormsApplication1.util;

namespace transmitFileApp.job
{
    [DisallowConcurrentExecutionAttribute]
    class OcrFileJob : IJob
    {
        //将需要OCR成功的文件复制到指定的文件中
        
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DirectoryInfo sucessFileFolder = new DirectoryInfo(PathUtil.ocrSuccessDestFilePath); //获取ocr成功目录中所有的文件
                FileInfo[] list = sucessFileFolder.GetFiles();

                foreach (FileInfo ocrFile in list)
                {
                    Console.WriteLine("准备上传文件...");
                    String fileName = Path.GetFileName(ocrFile.FullName);
                    String fileDirectory = Path.GetDirectoryName(ocrFile.FullName);
                    String tempFullName = Path.Combine(fileDirectory, fileName + ".temp");
                    try
                    {
                        File.Move(ocrFile.FullName, tempFullName);  //如果能够重命名，说明文件没有被占用
                        File.Move(tempFullName, ocrFile.FullName);   //重新把文件名命名回去
                        Console.WriteLine("重命名成功" + fileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("重命名失败,下次重命名:" + ex.Message + "-" + fileName);
                        continue;
                    }
                    PdfStream pdfStream = new PdfStream();
                    //将状态设置为ocr完成 2
                    //查看该文件是否执行失败
                    String num = Path.GetFileNameWithoutExtension(fileName);
                    if (num.Contains(SystemConstant.ERROR_STRING))
                    {
                        pdfStream.ocr_flag = OcrConstant.OCR_FAIL;
                        num = num.Replace(SystemConstant.ERROR_STRING, "");
                    }
                    else
                    {
                        pdfStream.ocr_flag = OcrConstant.OCR_FINISH;
                    }
                    PdfVal pv = null;
                    try
                    {
                        pv = getPdfValByNum(num);
                        if (pv == null)
                        {
                            Console.WriteLine("获取id失败,请坚持该条数据" + fileName);
                            File.Delete(ocrFile.FullName);
                            File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                            //TxtUtil.removeId(int.Parse(num));
                            if (fileName.Contains(SystemConstant.ERROR_STRING))
                            {
                                String errorFileName = fileName.Replace(SystemConstant.ERROR_STRING, "");
                                Console.WriteLine("删除错误的文件名:" + errorFileName);
                                File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, errorFileName));
                            }
                            Console.WriteLine("删除needOcr文件" + Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("获取id失败,请坚持该条数据" + fileName);
                        File.Delete(ocrFile.FullName);
                        File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                        //TxtUtil.removeId(int.Parse(num));
                        if (fileName.Contains(SystemConstant.ERROR_STRING))
                        {
                            String errorFileName = fileName.Replace(SystemConstant.ERROR_STRING, "");
                            Console.WriteLine("删除错误的文件名:" + errorFileName);
                            File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, errorFileName));
                        }
                        Console.WriteLine("删除needOcr文件" + Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                        continue;
                    }
                   
                    String remotePath;
                    //微信文章跟其他类型的报告路径不一样
                    if (pv.type == SystemConstant.ARTICLE_TYPE)
                    {
                        remotePath = Path.Combine(SystemConstant.NEW_ARTICLE_UPLOAD_PATH, getAbsolutePdfPath(pv.path));
                    }
                    else
                    {
                        remotePath = Path.Combine(SystemConstant.NEW_UPLOAD_PATH, getAbsolutePdfPath(pv.path));
                    }
                    Console.WriteLine(remotePath);
                    //如果是excel直接上传，如果是PDF执行下面的操作
                    if (fileName.Contains(SystemConstant.EXCEL_EXT))
                    {
                        //上传

                        bool uploadExcelIsSuccess = Ftp.upload(ocrFile.FullName, Path.ChangeExtension(remotePath, "xlsx"));
                        if (uploadExcelIsSuccess)
                        {
                            File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                            File.Delete(ocrFile.FullName);
                        }
                        continue;
                    }

                    bool uploadPdfIsSuccess = Ftp.upload(ocrFile.FullName, remotePath);
                    if (!uploadPdfIsSuccess)
                    {
                        continue;
                    }
                    Console.WriteLine("上传成功。。。" + pv.id);
                    List<PdfStream> pdfStreamList = new List<PdfStream>();
                    
                    try
                    {
                        pdfStream.id = pv.id;
                        pdfStream.pdf_path = getPdfPath(pv.path); // 
                        pdfStreamList.Add(pdfStream);
                        if (pdfStream.ocr_flag == OcrConstant.OCR_FINISH)
                        {
                            HttpUtil.updatePdfStreamDataByPipeLineToMultiFlag(pdfStreamList);  //通过multiFlag接口更新
                        }
                        else
                        {
                            HttpUtil.updatePdfStreamDataPipeLine(pdfStreamList);  //更新到数据库
                        }
                        
                        Console.WriteLine("-更新数据库成功");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "-更新时出错");
                    }
                    finally
                    {
                        File.Delete(ocrFile.FullName);
                        File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                        if (fileName.Contains(SystemConstant.ERROR_STRING))
                        {
                            String errorFileName = fileName.Replace(SystemConstant.ERROR_STRING, "");
                            Console.WriteLine("删除错误的文件名:" + errorFileName);
                            File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, errorFileName));
                        }
                        //TxtUtil.removeId(int.Parse(num));
                        Console.WriteLine("删除needOcr文件" + Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public PdfVal getPdfValByNum(String num)
        {
            
            return TxtUtil.getId(int.Parse(num));   //获取真实Id
        }

        public String getPdfPath(String pdfPath)
        {
            if (!pdfPath.Contains(SystemConstant.DECRYPTION_PATH))
            {
                return Path.Combine(SystemConstant.DECRYPTION_PATH, pdfPath);
            }
            return pdfPath;
        }

        public String getAbsolutePdfPath(String pdfPath)
        {
            if (pdfPath.Contains(SystemConstant.DECRYPTION_PATH))
            {
                return pdfPath.Replace(SystemConstant.DECRYPTION_PATH, "");

            }
            else
            {
                return pdfPath;

            }
        }
    }
}
