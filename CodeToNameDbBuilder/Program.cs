using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;

namespace CodeToNameDbBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var resultSql =
                "insert into airportinfo(cityName, cityNameEN, cityNamePY, cityNameJP, cityCode, airportCode, airportName, firstLetter, cityAirportName, DataChange_LastTime)\nVALUES";
            FileStream stream = File.Open(args[0], FileMode.Open, FileAccess.Read);


            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                //4. DataSet - Create column names from first row
                DataSet result = excelReader.AsDataSet();
                var table = result.Tables[0];
                var rows = table.AsEnumerable().ToList();
                //遍历每一个sheet
                for (int i = 1; i < rows.Count; i++)
                {
                    var cityName = rows[i][2];
                    var cityNameEN = rows[i][3];
                    var cityNamePY = rows[i][4];
                    var cityNameJP = rows[i][5];
                    var cityCode = rows[i][6];
                    var airportCode = rows[i][7];
                    var airportName = rows[i][8];
                    var firstLetter = rows[i][9];
                    var cityAirportName = rows[i][17];
                    var DataChange_LastTime = DateTime.Now.ToString("u").TrimEnd('Z');
                    resultSql += String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'),\n", cityName, cityNameEN, cityNamePY, cityNameJP, cityCode, airportCode, airportName, firstLetter, cityAirportName, DataChange_LastTime);
                }
                resultSql = resultSql.TrimEnd('\n');
                resultSql = resultSql.TrimEnd(',');
                resultSql += ";";
                var hehe = File.Open(Directory.GetCurrentDirectory() + "\\temp.xml", FileMode.OpenOrCreate);
                var buffer = Encoding.UTF8.GetBytes(resultSql);
                hehe.Write(buffer, 0, buffer.Length);
                hehe.Close();
            }
        }
    }
}
