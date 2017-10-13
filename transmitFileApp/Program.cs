using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.constant;
using transmitFileApp.job;
using transmitFileApp.util;
using WindowsFormsApplication1;

namespace transmitFileApp
{
    class Program
    {
        //从工厂中获取一个调度器实例化
        static IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        static void Main1(string[] args)
        {
            test1();
        }
        static void Main(string[] args)
        {
            Console.WriteLine("-------------------");
            PathUtil.initPathData();  //初始化数据

            
            scheduler.Start();       //开启调度器

            //==========定时传输需要ocr的文件处理===========

            IJobDetail job1 = JobBuilder.Create<NeedOcrFileJob>()  //创建一个作业
                .WithIdentity("copyNeedOcrFile", "copyNeedOcrGroup")
                .Build();

            ITrigger trigger1 = TriggerBuilder.Create()
                                       .StartNow()                        //现在开始
                                       .WithSimpleSchedule(x => x         //触发时间，30秒一次
                                           .WithIntervalInSeconds(20)
                                           .RepeatForever())              //不间断重复执行
                                       .Build();

            scheduler.ScheduleJob(job1, trigger1);     

            //==========定时将ocr成功的文件传输到一个新的位置===========
            IJobDetail job2 = JobBuilder.Create<OcrFileJob>()  //创建一个作业
                .WithIdentity("copyOcrSuccessFile", "copyOcrSuccessGroup")
                .Build();

            ITrigger trigger2 = TriggerBuilder.Create()
                                       .StartNow()                        //现在开始
                                       .WithSimpleSchedule(x => x         //触发时间，30秒一次。
                                           .WithIntervalInSeconds(30)
                                           .RepeatForever())              //不间断重复执行
                                       .Build();

            scheduler.ScheduleJob(job2, trigger2);      //把作业，触发器加入调度器。
            //Console.ReadKey();
        }
            //结束

        public static void test1() 
        {
            SshConnectionInfo objInfo = new SshConnectionInfo();
            objInfo.User = "root";
            objInfo.Host = "106.75.3.227";
            //objInfo.IdentityFile = "password"; //有2中认证，一种基于PrivateKey,一种基于password
            objInfo.Pass = "ho227boom2uttb"; //基于密码
            SFTPHelper objSFTPHelper = new SFTPHelper(objInfo);
            //ArrayList list = objSFTPHelper.GetFileList("/data/dearMrLei/data/rich/2017/07/01");
            String remotePath = "/data/dearMrLei/data/rich/2017/07/01/FZLzzR9H8uJCtGcqg.pdf";
            String localPath = "D:\\test\\pdf\\FZLzzR9H8uJCtGcqg.pdf";
            objSFTPHelper.Download(remotePath, localPath);
            Console.WriteLine("download...");
        }
    }
}
