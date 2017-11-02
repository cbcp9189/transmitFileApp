using ConsoleConvertExcelApp.entity;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.constant;
using transmitFileApp.util;
using WindowsFormsApplication1;
using WindowsFormsApplication1.util;

namespace transmitFileApp.job
{
    [DisallowConcurrentExecutionAttribute]
    class NeedOcrFileJob : IJob
    {
        public static int fileNameVal = 0;


        //将需要OCR的文件复制到指定的文件中
        public void Execute(IJobExecutionContext context)
        {
            DirectoryInfo theNeedOcrFolder = new DirectoryInfo(PathUtil.needOcrDestFilePath);
            if (theNeedOcrFolder.GetFiles().Length >= SystemConstant.FILE_COUNT)  //
            {
                Console.WriteLine(DateTime.Now+":需要ocr的目录中文件个数大于等于2,暂时不添加数据");
                return;
            }
            
            try
            {
                //从接口获取需要ocr数据
                PdfData pdfData = HttpUtil.getPdfStreamDataByPipeLining(SystemConstant.LIMIT);
                foreach (PdfStream pdfInfo in pdfData.data)
                {
                    String needOcrSourcePath = Path.Combine(PathUtil.tempSoucePath + Path.GetFileName(pdfInfo.pdf_path));
                    String remotePath = "";
                    //微信文章跟其他类型的报告路径不一样
                    if (pdfInfo.doc_type == SystemConstant.ARTICLE_TYPE)
                    {
                        remotePath = Path.Combine(SystemConstant.ARICLE_PATH, pdfInfo.pdf_path);
                    }
                    else
                    {
                        remotePath = Path.Combine(SystemConstant.NORMAL_PATH, pdfInfo.pdf_path);
                    }
                    bool isExist = Ftp.checkFile(remotePath);
                    if (!isExist) {
                        Console.WriteLine("服务器文件不存在");
                        pdfInfo.ocr_flag = OcrConstant.SERVER_FILE_NO_EXIST;
                        updatePdfStreamOcrStatus(pdfInfo);
                        return;
                    }
                    bool isSuccess = Ftp.donwload(needOcrSourcePath, remotePath);
                    if (!isSuccess) {
                        pdfInfo.ocr_flag = OcrConstant.DOWNLOAD_FAIL;
                        Console.WriteLine("服务器文件下载失败");
                        updatePdfStreamOcrStatus(pdfInfo);
                        return;
                    }

                    if (!File.Exists(needOcrSourcePath))   //将标识更新为文件不存在 -2
                    {
                        pdfInfo.ocr_flag = OcrConstant.FILE_NO_EXIST;
                        updatePdfStreamOcrStatus(pdfInfo);
                        return;
                    }
                    fileNameVal++;
                    String fileName = Path.GetFileName(pdfInfo.pdf_path);  //获取文件名
                    //将文件名做一个映射关系
                    String descFilePath = Path.Combine(PathUtil.needOcrDestFilePath, fileNameVal + Path.GetExtension(fileName));
                    Console.WriteLine(descFilePath);
                    try
                    {
                        TxtUtil.addId(fileNameVal, pdfInfo.id,pdfInfo.doc_type,pdfInfo.pdf_path);
                        File.Move(needOcrSourcePath,descFilePath);
                        Console.WriteLine("复制成功...");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("复制失败...");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void updatePdfStreamOcrStatus(PdfStream pdfInfo)
        {
            try
            {
                List<PdfStream> pdfDataList = new List<PdfStream>();
                pdfDataList.Add(pdfInfo);
                HttpUtil.updatePdfStreamDataPipeLine(pdfDataList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void demo() {


            FileInfo file = new FileInfo(PathUtil.tempSoucePath);
            FileInfo[] list = file.Directory.GetFiles();
            foreach (FileInfo f in list)
            {
                String fileName = Path.GetFileName(f.FullName);
                String filePath = Path.Combine(PathUtil.needOcrDestFilePath, fileName);
                File.Copy(f.FullName, filePath);
            }
        }
    }
}
