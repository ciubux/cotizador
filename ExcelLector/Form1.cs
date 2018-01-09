using BusinessLayer;
using Model;
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

namespace ExcelLector
{
    public partial class Form1 : Form
    {
        public Form1()
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
            for (int row = 1; row <= 6044; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    try
                    {
                        ClienteStaging clienteStaging = new ClienteStaging();
                    //    IRow irow = sheet.GetRow(row);
                       // Type type = sheet.GetRow(row).GetCell(2).GetType();

                        clienteStaging.PlazaId = sheet.GetRow(row).GetCell(0).ToString();
                        clienteStaging.Id = sheet.GetRow(row).GetCell(1).ToString();
                        clienteStaging.nombre = sheet.GetRow(row).GetCell(2).ToString();
                        clienteStaging.documento = sheet.GetRow(row).GetCell(3).ToString();
                        clienteStaging.codVe = sheet.GetRow(row).GetCell(4).ToString();
                        clienteStaging.nombreComercial = sheet.GetRow(row).GetCell(5).ToString();
                        clienteStaging.domicilioLegal = sheet.GetRow(row).GetCell(6).ToString();
                        clienteStaging.distrito = sheet.GetRow(row).GetCell(7).ToString();
                        clienteStaging.direccionDespacho = sheet.GetRow(row).GetCell(8).ToString();
                        clienteStaging.distritoDespacho = sheet.GetRow(row).GetCell(9).ToString();
                        clienteStaging.rubro = sheet.GetRow(row).GetCell(10).ToString();

                        ClienteBL clienteBL = new ClienteBL();
                        clienteBL.setClienteStaging(clienteStaging);

                    } catch (Exception ee)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ee);


                    }


                   







                    //MessageBox.Show(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));
                }
            }
        }
    }
}
