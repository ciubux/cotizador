using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace Cotizador.ExcelExport
{
    public class SummaryClass
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class PedidoSearch
    {

        public void generateExcel()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID");
            dt.Columns.Add("Name");

            DataRow dr = dt.NewRow();
            dr["ID"] = "1";
            dr["Name"] = "Test";

            dt.Rows.Add(dr);

            // Declare HSSFWorkbook object for create sheet  
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("Pedidos");

            // Convert datatable into json  
            string JSON = JsonConvert.SerializeObject(dt);

            // Convert json into SummaryClass class list  
            var items = JsonConvert.DeserializeObject<List<SummaryClass>>(JSON);

            // Set column name this column name use for fetch data from list  
            var columns = new[] { "ID", "Name" };

            // Set header name this header use for set name in excel first row  
            var headers = new[] { "ID", "Name" };

            var headerRow = sheet.CreateRow(0);

            //Below loop is create header  
            for (int i = 0; i < columns.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
            }

            //Below loop is fill content  
            for (int i = 0; i < items.Count; i++)
            {
                var rowIndex = i + 1;
                var row = sheet.CreateRow(rowIndex);

                for (int j = 0; j < columns.Length; j++)
                {
                    var cell = row.CreateCell(j);
                    var o = items[i];
                    cell.SetCellValue(o.GetType().GetProperty(columns[j]).GetValue(o, null).ToString());
                }
            }

            // Declare one MemoryStream variable for write file in stream  
            var stream = new MemoryStream();
            workbook.Write(stream);

            string FilePath = "SetYourFileSavePath - With File Name";

            //Write to file using file stream  
            FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
            stream.WriteTo(file);
            file.Close();
            stream.Close();
        }
    }
}