using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;

namespace Excel2Xsd
{
    class Program
    {
        static void Main(string[] args)
        {
            List<InterfaceObj> interfaceObjs = new List<InterfaceObj>();
            FileStream stream = File.Open(args[0], FileMode.Open, FileAccess.Read);


            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                excelReader.IsFirstRowAsColumnNames = true;
                //4. DataSet - Create column names from first row
                DataSet result = excelReader.AsDataSet();
                var tables = result.Tables;
                //遍历每一个sheet
                for (int i = 0; i < tables.Count; i++)
                {
                    InterfaceObj interfaceObj = new InterfaceObj(tables[i]);
                    interfaceObjs.Add(interfaceObj);
                }
                var str = interfaceObjs.Aggregate("", (s, obj) => s += obj.BuildDto());
                var hehe = File.Open(Directory.GetCurrentDirectory() + "\\temp.xml", FileMode.OpenOrCreate);
                var buffer = Encoding.UTF8.GetBytes(str);
                hehe.Write(buffer, 0, buffer.Length);
                hehe.Close();
                //foreach (var interfaceObj in interfaceObjs)
                //{
                //    Console.WriteLine(interfaceObj.BuildXsd());

                //}
            }
             
        }
    }

    public static class BuildConfig
    {
        
    }
}
