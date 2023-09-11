using Microsoft.Office.Interop.Excel;
using System;

public class Scenario1
{
    public decimal solidSG;
    public decimal MW;
    public decimal formationFluidSG;
    public decimal grafity;
    public decimal gasSG;
    public decimal absTemp;
    public decimal tempGrad;
    public decimal rougnessSteel;
    public decimal rougnessHole;
    public List<decimal> ROP = new List<decimal>();
    public List<decimal> gpm = new List<decimal>();
    public List<decimal> influxRate = new List<decimal>();
    public List<decimal> scfm = new List<decimal>();
    public List<decimal> surfacePres = new List<decimal>();
    public static void main()
    {
        Console.WriteLine("hello class");
    }
}
