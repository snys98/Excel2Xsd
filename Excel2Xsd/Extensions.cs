using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2Xsd
{
    public static partial class Extensions
    {
        //public static IEnumerable<DataRow> GetValidRows(this DataTable table, int columnIndex)
        //{
        //    return table.AsEnumerable().Where(item => item[columnIndex] != string.Empty);
        //}
        /// <summary>
        /// rowNum,columnNum,name,type,meta,remark
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<(int rowNum, int level, string name, string type, string metadata, string range,string remark)> GetRowNumValuePairsOfColumn(this DataTable table)
        {
            var results =
                table.AsEnumerable()
                    .Select(
                        myRow =>
                        {
                            var rowItems = myRow.ItemArray.ToList();
                            return (rowNum: table.Rows.IndexOf(myRow),
                                level: rowItems.FindIndex(item => item.ToString() != string.Empty),
                                name: rowItems.FirstOrDefault(item => item.ToString() != string.Empty)?.ToString(),
                                type: myRow["Type"]?.ToString(),
                                metadata: myRow["Metadata"]?.ToString(),
                                range: myRow["Range"]?.ToString(),
                                remark: myRow["Remark"]?.ToString());
                        });
        return results.ToList();
        }

    }
}
