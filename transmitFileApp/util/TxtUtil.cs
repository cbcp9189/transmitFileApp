using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace transmitFileApp.util
{
    class TxtUtil
    {
        public static Dictionary<int, long> getIdMapList()
        {
            Dictionary<int, long> idMap = new Dictionary<int, long>();
            string[] idArray = File.ReadAllLines(@"idmapping.txt");
            foreach (String id in idArray) 
            {
                String[] numArray = id.Split('-');
                if (numArray.Length == 2) 
                {
                    idMap.Add(int.Parse(numArray[0]), long.Parse(numArray[1]));
                }
                
            }
            return idMap;
        }
        public static long getId(int num) {
            Dictionary<int, long> idMap = getIdMapList();
            if (idMap.ContainsKey(num))
            {
                return idMap[num];
            }
            else 
            {
                return 0;
            }
        }

        public static void removeId(int num)
        {
            Dictionary<int, long> idMap = getIdMapList();
            if (idMap.Remove(num))
            {
                string[] txtArray = new string[idMap.Count];
                int index = 0;
                foreach (var item in idMap) 
                {
                    txtArray[index] = item.Key + "-" + item.Value;
                    index++;
                }
                File.WriteAllLines(@"idmapping.txt", txtArray); 
            }
        }

        public static void addId(int num,long pdfStreamId)
        {
            Dictionary<int, long> idMap = getIdMapList();
            idMap.Add(num, pdfStreamId);
            string[] txtArray = new string[idMap.Count];
            int index = 0;
            foreach (var item in idMap)
            {
                txtArray[index] = item.Key + "-" + item.Value;
                index++;
            }
            File.WriteAllLines(@"idmapping.txt", txtArray);
        }

        public static void removeAllIdMapping()
        {
            FileStream fs = new FileStream(@"idmapping.txt", FileMode.Create);
        }
    }
}
