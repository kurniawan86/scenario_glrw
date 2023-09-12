using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using System;
using System.Diagnostics;
using System.Net;

public class ScenarioResult
{
    public decimal m_depth;
    public decimal inc_deg;
    public decimal v_depth;
    public decimal temp;
    public decimal section;
    public decimal component;
    public decimal rough;
    public decimal od;
    public decimal id;
    public decimal p_dynamic;
    public decimal ecd;
    public decimal density;
    public decimal velocity;
    public decimal kinetic;
    public decimal p_static;
    public decimal p_dynstat;
    public decimal esd;
    public decimal ecd_esd;
    public decimal cut_depth_lfr;
    public decimal cut_depth_vel;
    public string Message="";
}

public class WellTrajectoryDetail
{
    public List<decimal> Mdepth;
    public List<decimal> incl_deg;
}
public class Scenario
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
    Configuration config;

    public Scenario(Configuration config)
    {
        this.config = config;
    }
    public List<ScenarioResult> Calculate()
    {
        List<ScenarioResult> result = new List<ScenarioResult>();
        List<WellTrajectoryDetail> wellTrajectoryDetails = new List<WellTrajectoryDetail>();
        WellTrajectoryDetail well = new WellTrajectoryDetail();
        well.Mdepth = this.config.MDepth;
        well.incl_deg = this.config.incDeg;
        wellTrajectoryDetails.Add(well);

        var maxdWell = 0.0m;
        var maxWell = 0.0m;
        decimal[] arYWell = { 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, };
        decimal[] ptsWell = { 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, };
        int[] bCX = { 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, };
        decimal[] aredWell = {
        config.Casing_shoe_depth,
        config.Top_liner_depth,
        config.Liner_shoe,
        config.Hole_depth,
        config.Hole_depth,
        config.Dp1_length,
        config.Dc_lengt,
        config.Bit_depth,
        config.Dp2_od,
        config.Dp1_od,
        config.Dc_od
        };

        var doMax = 0.0m;
        decimal[] adX = { 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m };

        foreach (var item in aredWell.Select((value, i) => new { i, value }))
        {
            var dx = item.value;
            if (dx == 0.0m)
            {
                dx = 0.0m;
                if (bCX[item.i] == 1)
                {
                    return result;
                }
            }
            else if (item.i >= 8 && dx > doMax)
            {
                doMax = dx;
            }
            Console.WriteLine(item.i);
            adX[item.i] = dx;
        }

        Console.WriteLine("config.bit depth : "+config.Bit_depth);
        Console.WriteLine("doMax : " + doMax);
        if (config.Bit_depth > doMax)
        {
            adX[7] = config.Bit_depth;
        }
        Console.WriteLine("adx[7] : "+ adX[7]);
        Console.WriteLine("adx[6] : " + adX[6]);

        if (adX[2] != 0.0m && (adX[1] >= adX[2] || adX[1] > adX[0] || adX[2] < adX[0]))
        {
            ScenarioResult det_result = new ScenarioResult();
            det_result.Message = "Invalid Casing Shoe / Top Liner Depth / Liner Shoe value(s)!";
            result.Add(det_result);
            //this.result = "Invalid Casing Shoe / Top Liner Depth / Liner Shoe value(s)!";
            return result;
        }

        //foreach(var val in wellTrajectoryDetails)
        //{
        //    Debug.WriteLine(val.Mdepth[3]);
        //}

        DrawWell(ref arYWell, ref adX, ref ptsWell, ref maxWell, ref maxdWell, ref result, wellTrajectoryDetails);

        return result;
    }

    private void DrawWell(ref decimal[] arYWell, ref decimal[] adX, ref decimal[] ptsWell, ref decimal maxWell, ref decimal maxdWell, ref List<ScenarioResult> wellResult, List<WellTrajectoryDetail> wellTrajectoryDetails)
    {
        Console.WriteLine("adX1 : " + adX[1]);
        arYWell[0] = 0;
        arYWell[1] = adX[0];
        arYWell[2] = adX[1];
        arYWell[3] = adX[2];

        if (adX[2] != 0.0m)
        {
            arYWell[4] = adX[2];
        }
        else
        {
            arYWell[4] = adX[0];

        }

        arYWell[5] = adX[3];
        arYWell[6] = 0;
        arYWell[9] = adX[7] - adX[6];
        //Debug.WriteLine("<<<<<<<<<<<<<<<<<<<<<<");
        //Debug.WriteLine(arYWell[9]);
        //Debug.WriteLine(adX[7]);
        //Debug.WriteLine(adX[6]);
        //Debug.WriteLine("<<<<<<<<<<<<<<<<<<<<<<");
        arYWell[8] = arYWell[9] - adX[5];
        arYWell[7] = arYWell[8];
        arYWell[10] = arYWell[9];
        arYWell[11] = adX[7];

        //if (wX < 0)
        //{
        ptsWell[0] = adX[1];
        Console.WriteLine("ptsWell : " + ptsWell[0]);
        ptsWell[1] = adX[2];
        ptsWell[2] = adX[0];
        ptsWell[3] = arYWell[8];
        ptsWell[4] = arYWell[9];
        ptsWell[5] = adX[7];
        maxWell = adX[7];
        for (int i = 0; i < 5; i++)
        {
            var r2 = ptsWell[i];
            Console.WriteLine(" r2 : " + r2);
            var io = -1;
            for (int j = i + 1; j < 6; j++)
            {
                var r3 = ptsWell[j];
                Console.WriteLine(" r3 : " + r3);
                if (r3 < r2)
                {
                    io = j;
                    r2 = r3;
                }
            }
            Console.WriteLine("io : " + io);

            if (io > 0)
            {
                ptsWell[io] = ptsWell[i];
                ptsWell[i] = r2;
            }
        }
        CreateWellData(ref wellResult, ref maxdWell, ptsWell, maxWell, wellTrajectoryDetails);
        Console.WriteLine("-------------------");
        foreach (var w in wellResult)
        {
            Console.WriteLine(w.m_depth);
        }
        Console.WriteLine("-------------------");
    }

    private void CreateWellData(ref List<ScenarioResult> wellResult, ref decimal maxdWell, decimal[] ptsWell, decimal maxWell, List<WellTrajectoryDetail> wellTrajectoryDetails)
    {
        var r2x = 0m;
        var r3x = 0m;
        var iW = 0;
        var i2 = 0;
        var rAdd = ptsWell[0];
        
        Debug.WriteLine("******");
        foreach (var hasil in wellTrajectoryDetails)
        {

            Debug.WriteLine(maxWell);
            for (int index=0;index<hasil.Mdepth.Count;index++) 
            {
                var r2 = hasil.Mdepth[index];

                if (r2 > maxWell) 
                { 
                    break; 
                }

                var r3 = hasil.incl_deg[index];

                while ((i2 < ptsWell.Count()) && (r2 > rAdd))
                {
                    Console.WriteLine("TRUE - "+ wellTrajectoryDetails.Count());
                    Console.WriteLine("TRUE");
                    if ((rAdd != 0m) && (r2 != rAdd))
                    {
                        var dY1 = 100 * (r3x + (rAdd - r2x) / (r2 - r2x) * (r3 - r3x)) + 0.5m;
                        dY1 = (dY1) / 100;

                        //decimal[] temp = { rAdd, dY1, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m };
                        var scenarioResult = new ScenarioResult()
                        {
                            m_depth = rAdd,
                            inc_deg = dY1
                        };

                        if (rAdd != r2x)
                        {
                            wellResult.Add(scenarioResult);
                            iW++;
                        }
                        else
                        {
                            wellResult[iW - 1] = scenarioResult;
                        }
                    }
                    i2++;
                    rAdd = ptsWell[i2];
                }
                wellResult.Add(new ScenarioResult()
                {
                    m_depth = r2,
                    inc_deg = r3
                });
                iW++;
                r2x = r2;
                r3x = r3;
            }
            Console.WriteLine(" i2 : "+ i2);
            while (i2 < 6)
            {
                rAdd = ptsWell[i2];
                Console.WriteLine("rAdd : " + rAdd);
                Console.WriteLine("r2x : " + r2x);
                if (rAdd != r2x)
                {
                    Console.WriteLine("True");
                    wellResult.Add(new ScenarioResult()
                    {
                        m_depth = rAdd,
                        inc_deg = r3x
                    });
                    iW++;
                }
                i2++;
            }
            maxdWell = iW;
        }
    }
    public static void main()
    {
        Console.WriteLine("hello class");
    }
}
