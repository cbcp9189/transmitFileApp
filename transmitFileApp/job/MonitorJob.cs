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
    class MonitorJob : IJob
    {
        public static int count = 0;
        public void Execute(IJobExecutionContext context)
        {
            count++;
            if (count == 1)
            {
                return;
            }
            
            DateTime dt = DateTime.Now;

            TimeSpan d3 = dt.Subtract(NeedOcrFileJob.defaultDate);
            Console.WriteLine("间隔时间: " + d3);
            if (d3.Minutes > SystemConstant.TIME_SPAN)  //如果大于30分钟，则发送短信
            {
                Console.WriteLine("发送短信: " + d3);
                HttpUtil.sendMessage(PathUtil.serverName, "30分钟没有生成新文件", "ocr");
            }
        }

    }
}
