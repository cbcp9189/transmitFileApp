using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using transmitFileApp.entity;

namespace transmitFileApp.util
{
    class TxtUtil
    {
        public static List<PdfVal> getIdMapingList()
        {
            List<PdfVal> pdfValList = new List<PdfVal>();
            string[] idArray = File.ReadAllLines(@"idmapping.txt");
            foreach (String id in idArray) 
            {
                String[] numArray = id.Split('-');
                if (numArray.Length == 4) 
                {
                    PdfVal p = new PdfVal();
                    p.num = int.Parse(numArray[0]);
                    p.id = long.Parse(numArray[1]);
                    p.type = int.Parse(numArray[2]);
                    p.path = numArray[3];
                    pdfValList.Add(p);
                }
            }
            return pdfValList;
        }
        public static PdfVal getId(int num)
        {
            List<PdfVal> idMap = getIdMapingList();
            foreach (PdfVal p in idMap)
            {
                if (p.num == num)
                {
                    return p;
                }
            }
            return null;
        }

        public static void removeId(int num)
        {
            List<PdfVal> idMap = getIdMapingList();
            foreach (PdfVal p in idMap)
            {
                if (p.num == num)
                {
                    idMap.Remove(p);
                    string[] txtArray = new string[idMap.Count];
                    int index = 0;
                    foreach (var item in idMap)
                    {
                        txtArray[index] = item.num + "-" + item.id+"-"+item.type+"-"+item.path;
                        index++;
                    }
                    File.WriteAllLines(@"idmapping.txt", txtArray);
                    break;
                }
            }
            
        }

        public static void addId(int num,long pdfStreamId,int type,String path)
        {
            try
            {
                List<PdfVal> idMap = getIdMapingList();
                PdfVal p = new PdfVal();
                p.num = num;
                p.id = pdfStreamId;
                p.type = type;
                p.path = path;
                idMap.Add(p);
                string[] txtArray = new string[idMap.Count];
                int index = 0;
                foreach (var item in idMap)
                {
                    txtArray[index] = item.num + "-" + item.id + "-" + item.type + "-" + item.path;
                    index++;
                }
                File.WriteAllLines(@"idmapping.txt", txtArray);

            }
            catch (Exception ex)
            {
                Console.WriteLine("插入数据："+ex.Message);
            }
            
        }

        public static void removeAllIdMapping()
        {
            FileStream fs = new FileStream(@"idmapping.txt", FileMode.Create);
        }
    }
}
