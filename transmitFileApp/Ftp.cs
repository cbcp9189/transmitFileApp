using FluentFTP;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace transmitFileApp
{
    public class Ftp
    {
        static string m_host = "106.75.3.227";
        static string m_user = "root";
        static string m_pass = "ho227boom2uttb";
        public static void mytest()
        {
            using (var client = new ScpClient(m_host, m_user, m_pass))
            {
                try
                {
                    client.Connect();
                    //client.
                    //client.CreateDirectory("/home/sshnet/");
                    //client.up
                        
                    client.Upload(new FileInfo(@"C:\Users\Administrator\Desktop\11\sql.txt"), "/home/sshnet/log/1.txt");
                    client.Disconnect();
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                }
            }
        }

        public static bool upload(string localPath, string remotePath)
        {
            mkdir(remotePath);
            using (var client = new ScpClient(m_host, m_user, m_pass))
            {
                try
                {
                    client.Connect();
                    client.Upload(new FileInfo(localPath), remotePath);
                    client.Disconnect();
                    return true;
                    
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    return false;
                }
            }
            
        }

        public static bool donwload(string localPath, string remotePath)
        {
            using (var client = new ScpClient(m_host, m_user, m_pass))
            {
                try
                {
                    client.RemotePathTransformation = RemotePathTransformation.ShellQuote;
                    client.Connect();
                    client.Download(remotePath, new FileInfo(localPath));
                    client.Disconnect();
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    return false;
                }
            }
            return true;
        }

        public static void mkdir(string remotePath)
        {
            using (var client = new SftpClient(m_host, m_user, m_pass))
            {
                try
                {
                    client.Connect();
                    String[] paths = remotePath.Split('/');
                    StringBuilder sb = new StringBuilder("/");
                    foreach (String a in paths)
                    {
                        if (!a.Equals("") && !a.Equals(paths[paths.Length - 1]))
                        {
                            sb.Append(a);
                            Console.WriteLine(sb.ToString());
                            //m_sshCp.
                            if (!client.Exists(sb.ToString()))
                            {
                                client.CreateDirectory(sb.ToString());
                                
                            }
                            sb.Append("/");
                        }

                    }
                    
                    client.Disconnect();
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                }
            }
        }

        public static bool checkFile(string remotePath)
        {
            using (var client = new SftpClient(m_host, m_user, m_pass))
            {
                try
                {
                    client.Connect();
                    if (client.Exists(remotePath))
                    {
                        client.Disconnect();
                        return true;
                    }
                    else
                    {
                        client.Disconnect();
                        return false;
                    }
                    
                    
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.Message);
                    return false;
                }
            }
        }
    }
}
