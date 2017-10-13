using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleConvertExcelApp.entity
{
    public class PdfStream
    {
        public long id;
        public String pdf_path = "";
        public long doc_id;
        public int doc_type;
        public int excel_flag;
        public int ocr_flag;
    }
}
