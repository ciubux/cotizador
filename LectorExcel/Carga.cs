using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LectorExcel
{
    public partial class Carga : Form
    {
        public Carga()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(@"C:\PENTAHO\PRODUCTOS\Clientes.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("Hoja1");
            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    String valor = sheet.GetRow(row).GetCell(0).StringCellValue;

                    //MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));
                }
            }

        }
    }
}
