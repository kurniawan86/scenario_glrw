using System;
using Microsoft.Office.Interop.Excel;

public class Configuration
{
    public List<decimal> MDepth = new List<decimal>();
    public List<decimal> incDeg = new List<decimal>();
    public List<decimal> ftMD = new List<decimal>();
    public List<decimal> ftTVD = new List<decimal>();
    public List<decimal> Pressure = new List<decimal>();
    public List<decimal> EMW = new List<decimal>();
    public decimal Casing_shoe_depth;
    public decimal Top_liner_depth;
    public decimal Liner_shoe;
    public decimal Hole_depth;
    public decimal Dp1_length;
    public decimal Dc_lengt;
    public decimal Bit_depth;
    public decimal Dp2_od;
    public decimal Dp1_od;
    public decimal Dc_od;
    public decimal Casing_id;
    public decimal Bit_size;
    public decimal Liner_id;

    public void readExcelReservoil()
    {
        Application excelApp = new Application();
        Workbook excelBook = excelApp.Workbooks.Open(
            "C:\\Users\\mucha\\Source\\Repos\\kurniawan86\\scenario_glrw\\resevoil.xlsx");
        _Worksheet excelSheet = excelBook.Sheets[1];

        Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;
        int rowCount = excelRange.Rows.Count;
        int colCount = excelRange.Columns.Count;
        //Console.WriteLine(rowCount);
        //Console.WriteLine(colCount);
        string dpt;
        for (int i = 1; i <= rowCount; i++)
        {
            //create new line
            //Console.Write("\r\n");
            for (int j = 1; j <= 1; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 1].Value2.ToString();
                    ftMD.Add(Convert.ToDecimal(dpt));
                }
            }

            for (int j = 2; j <= 2; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 2].Value2.ToString();
                    ftTVD.Add(Convert.ToDecimal(dpt));
                }
            }

            for (int j = 3; j <= 3; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 3].Value2.ToString();
                    Pressure.Add(Convert.ToDecimal(dpt));
                }
            }

            for (int j = 4; j <= 4; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 4].Value2.ToString();
                    EMW.Add(Convert.ToDecimal(dpt));
                }
            }
        }
        //int angka = int.Parse(dpt);
        excelApp.Quit();
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        //Console.WriteLine("------------");
        //Console.WriteLine(MDepth.Count);

        //foreach (double num in deg)
        //{
        //    Console.WriteLine(num);
        //}
    }

    public void readExcelWellTrajectory()
    {
        Application excelApp = new Application();
        Workbook excelBook = excelApp.Workbooks.Open(
            "C:\\Users\\mucha\\Source\\Repos\\kurniawan86\\scenario_glrw\\well_trajectory.xlsx");
        _Worksheet excelSheet = excelBook.Sheets[1];

        Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;
        int rowCount = excelRange.Rows.Count;
        int colCount = excelRange.Columns.Count;
        //Console.WriteLine(rowCount);
        //Console.WriteLine(colCount);
        string dpt;
        for (int i = 1; i <= rowCount; i++)
        {
            //create new line
            //Console.Write("\r\n");
            for (int j = 1; j <= 1; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 1].Value2.ToString();
                    MDepth.Add(Convert.ToDecimal(dpt));
                }
            }

            for (int j = 2; j <= 2; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[i, 2].Value2.ToString();
                    incDeg.Add(Convert.ToDecimal(dpt));
                }
            }
        }
        //int angka = int.Parse(dpt);
        excelApp.Quit();
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        //Console.WriteLine("------------");
        //Console.WriteLine(MDepth.Count);

        //foreach (double num in deg)
        //{
        //    Console.WriteLine(num);
        //}
    }
}
