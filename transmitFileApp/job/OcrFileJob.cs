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
using WindowsFormsApplication1.util;

namespace transmitFileApp.job
{
    [DisallowConcurrentExecutionAttribute]
    class OcrFileJob : IJob
    {
        //将需要OCR成功的文件复制到指定的文件中
        
        public void Execute(IJobExecutionContext context)
        {
            
            DirectoryInfo sucessFileFolder = new DirectoryInfo(PathUtil.ocrSuccessDestFilePath); //获取ocr成功目录中所有的文件
            FileInfo[] list = sucessFileFolder.GetFiles();
            Console.WriteLine("success  count:" + list.Length);
            foreach (FileInfo ocrFile in list)
            {
                String fileName = Path.GetFileName(ocrFile.FullName);
                String fileDirectory = Path.GetDirectoryName(ocrFile.FullName);
                String tempFullName = Path.Combine(fileDirectory,fileName+".temp");
                try
                {
                    File.Move(ocrFile.FullName, tempFullName);  //如果能够重命名，说明文件没有被占用
                    File.Move(tempFullName,ocrFile.FullName);   //重新把文件名命名回去
                    Console.WriteLine("重命名成功");
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message + "-" + fileName);
                    continue;
                }
                
                File.Move(ocrFile.FullName, Path.Combine(PathUtil.tempFinishPath, fileName));
                List<PdfStream> pdfStreamList = new List<PdfStream>();
                PdfStream pdfStream = new PdfStream();
                //将状态设置为ocr完成 2
                //查看该文件是否执行失败
                String num = Path.GetFileNameWithoutExtension(fileName);
                if (num.Contains(SystemConstant.ERROR_STRING))
                {
                    pdfStream.ocr_flag = OcrConstant.OCR_FAIL;
                    num = num.Replace(SystemConstant.ERROR_STRING, "");
                }
                else {
                    pdfStream.ocr_flag = OcrConstant.OCR_FINISH;
                }
                
                long id = TxtUtil.getId(int.Parse(num));   //获取真实Id
                pdfStream.id = id;
                pdfStreamList.Add(pdfStream);
                try
                {
                    HttpUtil.updatePdfStreamData(pdfStreamList);  //更新到数据库
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "-更新时出错");
                }
                finally 
                {
                    File.Delete(Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                    TxtUtil.removeId(int.Parse(num));
                    Console.WriteLine("删除needOcr文件" + Path.Combine(PathUtil.needOcrDestFilePath, fileName));
                }
            }
        }
    }
}
