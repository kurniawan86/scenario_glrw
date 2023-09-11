using Microsoft.Office.Interop.Excel;
using System;

public class Scenario1
{
    List<int> MDepth = new List<int>();
    public void readExcelWellTrajectory()
	{
        Console.WriteLine("data input");
        Application excelApp = new Application();
        Workbook excelBook = excelApp.Workbooks.Open(
            "C:\\Users\\kurniawan\\source\\repos\\scenario_glrw\\well_trajectory.xlsx");
        _Worksheet excelSheet = excelBook.Sheets[1];

        Microsoft.Office.Interop.Excel.Range excelRange = excelSheet.UsedRange;
        int rowCount = excelRange.Rows.Count;
        int colCount = excelRange.Columns.Count;
        string dpt="";
        for (int i = 1; i <= rowCount; i++)
        {
            //create new line
            Console.Write("\r\n");
            for (int j = 1; j <= colCount; j++)
            {
                //write the console
                if (excelRange.Cells[i, j] != null && excelRange.Cells[i, j].Value2 != null)
                {
                    dpt = excelRange.Cells[1, j].Value2.ToString();
                    MDepth.Add(int.Parse(dpt));
                }
            }
        }
        int angka = int.Parse(dpt);
        excelApp.Quit();
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

        foreach (int num in MDepth)
        {
            Console.WriteLine(num);
        }
    }

    public static void main()
    {
        Console.WriteLine("hello class");
    }
}
