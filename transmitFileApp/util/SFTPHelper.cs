using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh;

namespace WindowsFormsApplication1
{
    public class SFTPHelper
    {
        private SshTransferProtocolBase m_sshCp;
        private SFTPHelper()
        {

        }
        public SFTPHelper(SshConnectionInfo connectionInfo)
        {
            m_sshCp = new Sftp(connectionInfo.Host, connectionInfo.User);

            if (connectionInfo.Pass != null)
            {
                m_sshCp.Password = connectionInfo.Pass;
            }

            if (connectionInfo.IdentityFile != null)
            {
                m_sshCp.AddIdentityFile(connectionInfo.IdentityFile);
            }
        }

        public bool Connected
        {
            get
            {
                return m_sshCp.Connected;
            }
        }
        public void Connect()
        {
            if (!m_sshCp.Connected)
            {
                m_sshCp.Connect();
            }
        }
        public void Close()
        {
            if (m_sshCp.Connected)
            {
                m_sshCp.Close();
            }
        }
        public bool Upload(string localPath, string remotePath)
        {
            try
            {
                if (!m_sshCp.Connected)
                {
                    m_sshCp.Connect();
                }
                m_sshCp.Put(localPath, remotePath);

                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool Download(string remotePath, string localPath)
        {
            try
            {
                if (!m_sshCp.Connected)
                {
                    m_sshCp.Connect();
                }

                m_sshCp.Get(remotePath, localPath);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(string remotePath)
        {
            try
            {
                if (!m_sshCp.Connected)
                {
                    m_sshCp.Connect();
                }
               // ((Sftp)m_sshCp).Delete(remotePath);//刚刚新增的Delete方法

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ArrayList GetFileList(string path)
        {
            try
            {
                if (!m_sshCp.Connected)
                {
                    m_sshCp.Connect();
                }
                return ((Sftp)m_sshCp).GetFileList(path);
            }
            catch
            {
                return null;
            }
        }

        //检查文件是否存在
        public Boolean checkFileIsExist(string path)
        {
            try
            {
                if (!m_sshCp.Connected)
                {
                    m_sshCp.Connect();
                }
                ArrayList list = ((Sftp)m_sshCp).GetFileList(path);
                if (list != null && list.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch
            {
                return false;
            }
        }

        public static bool getRemoteFile(String remotePath, String localPath)
        {
            SshConnectionInfo objInfo = new SshConnectionInfo();
            objInfo.User = "root";
            objInfo.Host = "106.75.3.227";
            //objInfo.IdentityFile = "password"; //有2中认证，一种基于PrivateKey,一种基于password
            objInfo.Pass = "ho227boom2uttb"; //基于密码
            SFTPHelper objSFTPHelper = new SFTPHelper(objInfo);
            //ArrayList list = objSFTPHelper.GetFileList("/data/dearMrLei/data/rich/2017/07/01");
            //String remotePath = "/data/dearMrLei/data/rich/2017/07/01/FZLzzR9H8uJCtGcqg.pdf";
            //String localPath = "D:\\test\\pdf\\FZLzzR9H8uJCtGcqg.pdf";
            return objSFTPHelper.Download(remotePath, localPath);
            //Console.WriteLine("download...");
        }

        public static bool checkFile(String remotePath)
        {
            SshConnectionInfo objInfo = new SshConnectionInfo();
            objInfo.User = "root";
            objInfo.Host = "106.75.3.227";
            objInfo.Pass = "ho227boom2uttb"; //基于密码
            SFTPHelper objSFTPHelper = new SFTPHelper(objInfo);
            return objSFTPHelper.checkFileIsExist(remotePath);
        }


    }
}
