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
        //static string m_pass = "ho227boom2uttb";
        public static ConnectionInfo getConnect()
        {
            try
            {
                var keyFile = new PrivateKeyFile(@"C:\Users\Administrator\.ssh\id_rsa");
                var keyFiles = new[] { keyFile };
                var methods = new List<AuthenticationMethod>();
                methods.Add(new PrivateKeyAuthenticationMethod(m_user, keyFiles));
                //methods.Add(new PasswordAuthenticationMethod(m_user, "chenbo"));  //加入密码
                var con = new ConnectionInfo(m_host, 22, m_user, methods.ToArray());
                return con;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "-ftp");
            }
            return null;
                
        }
        public static void mytest()
        {

            var keyFile = new PrivateKeyFile(@"C:\Users\Administrator\.ssh\id_rsa");
            var keyFiles = new[] { keyFile };

            try
            {
                var methods = new List<AuthenticationMethod>();
                methods.Add(new PrivateKeyAuthenticationMethod(m_user, keyFiles));
                //methods.Add(new PasswordAuthenticationMethod(m_user, "chenbo"));  //加入密码

                var con = new ConnectionInfo(m_host, 22, m_user, methods.ToArray());
                using (var client = new ScpClient(con))
                {
                    String remotePath = "/data/dearMrLei/data/user_test/2017/06/16/1.pdf";
                    client.Connect();
                    
                    // Do some stuff below
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //C:\Users\Administrator\.ssh\id_rsa
            //var connectionInfo = new ConnectionInfo(m_host,
                                        //m_user,
                                        //new PrivateKeyAuthenticationMethod(@"C:\Users\Administrator\.ssh\id_rsa"));
           
            
        }

        public static bool upload(string localPath, string remotePath)
        {
            mkdir(remotePath);
            using (var client = new ScpClient(getConnect()))
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
            using (var client = new ScpClient(getConnect()))
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
            using (var client = new SftpClient(getConnect()))
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
            using (var client = new SftpClient(getConnect()))
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
