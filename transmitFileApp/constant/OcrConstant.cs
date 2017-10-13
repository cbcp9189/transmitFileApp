using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmitFileApp.constant
{
    public class OcrConstant
    {
        public static int SERVER_FILE_NO_EXIST = -4;  //服务器文件不存在
        public static int DOWNLOAD_FAIL = -3;  //下载失败
        public static int FILE_NO_EXIST = -2;
        public static int OCR_FAIL = -1;
        public static int DEFAULT_STATUS = 0;
        public static int COPY_FINISH = 1;  //需要OCR
        public static int OCR_FINISH = 2;  //OCR完成
        
    }
}
