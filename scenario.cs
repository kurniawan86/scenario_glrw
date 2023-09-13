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
    public string section;
    public string component;
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

        var loop = 0;
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

        //Console.WriteLine("config.bit depth : "+config.Bit_depth);
        //Console.WriteLine("doMax : " + doMax);
        if (config.Bit_depth > doMax)
        {
            adX[7] = config.Bit_depth;
        }
        //Console.WriteLine("adx[7] : "+ adX[7]);
        //Console.WriteLine("adx[6] : " + adX[6]);

        if (adX[2] != 0.0m && (adX[1] >= adX[2] || adX[1] > adX[0] || adX[2] < adX[0]))
        {
            return result;
        }

        //foreach(var val in wellTrajectoryDetails)
        //{
        //    Debug.WriteLine(val.Mdepth[3]);
        //}

        DrawWell(ref arYWell, ref adX, ref ptsWell, ref maxWell, ref maxdWell, ref result, wellTrajectoryDetails);

        var gBitDepth = config.Bit_depth;
        var gDP1Depth = config.Dp1_length;
        var gDCDepth = config.Dc_lengt;

        var gDP2Depth = gBitDepth - gDCDepth - gDP1Depth;
        if (gDP2Depth < 0)
        {
            return result;
        }
        gDP1Depth += gDP2Depth;
        gDCDepth += gDP1Depth;
        var gCasingShoe = config.Casing_shoe_depth;
        var gTopLinerDepth = config.Top_liner_depth;
        var gCasing = config.Casing_id;
        var gBitSize = config.Bit_size;
        var gLinerID = 0m;
        if (gTopLinerDepth == 0 || gTopLinerDepth == 0m)
        {
            gTopLinerDepth = -1;
            gLinerID = gCasing;
        }
        else
        {
            gLinerID = config.Liner_id;
        }

        var gDP2OD = config.Dp2_od;
        var gDP1OD = config.Dp1_od;
        var gDCOD = config.Dc_od;

        var garOD = new decimal[] { gCasing, gLinerID, gBitSize };
        var garID = new decimal[] { gDP2OD, gDP1OD, gDCOD };

        var gMaxOD = gDCOD;

        if (gDP1OD > gMaxOD)
        {
            gMaxOD = gDP1OD;
        }
        else
        {
            gMaxOD = gDP2OD;
        }

        var i_row = 0;
        var gAbsTemp = this.absTemp;
        var gRoughSteel = this.rougnessSteel;
        var gRoughHole = this.rougnessHole;

        var rt0 = gAbsTemp + 460;

        decimal rD_old = 0, rI_old = 0, rV_old = 0, rD = 0, rI = 0, rV = 0, diff_rD, radian_rI,
            rRoughness = 0, r2, r3, rt, bECD = 0, rDpx = 0, rPfr1x = 0, rPfr2x = 0, d7x = 0, d8x = 0,
            rDepth = 0, rPfr1 = 0, rPfr2 = 0, rPpsi = 0, ra_fix1 = 0.0188m, MinMV = decimal.MaxValue, Kinm = decimal.MaxValue;

        var gps = this.surfacePres[loop] * (144);
        var rTemp0 = gAbsTemp + 460;
        var rPsi = gps / 144;
        var rSP = rPsi;
        var rP = gps;

        var rLiquid = this.gpm[loop];
        if (this.gpm[loop] < 0)
        {
            bECD = 1;
        }

        Console.WriteLine("testing result len result : " + result.Count);
        while (i_row < 1000)
        {
            Console.WriteLine("i row : "+i_row);
            try
            {
                rD = result[i_row].m_depth;
                rI = result[i_row].inc_deg;
                int ci2, ci3;

                if (i_row == 0)
                {
                    rV = rD;
                }
                else
                {
                    diff_rD = rD - rD_old;
                    radian_rI = Convert.ToDecimal(Math.PI / 180) * ((rI + rI_old) / 2);
                    rV = diff_rD * Convert.ToDecimal(Math.Cos(Convert.ToDouble(radian_rI))) + rV_old;
                }

                if (rD > gBitDepth)
                {
                    break;
                }

                rRoughness = gRoughSteel;

                if (rD < gCasingShoe)
                {
                    ci2 = 0;
                }
                else if (rD < gTopLinerDepth)
                {
                    ci2 = 0;
                }
                else
                {
                    ci2 = 2;
                    rRoughness = gRoughHole;
                }


                if (rD < (gDP2Depth + 0.00000001m))
                {
                    ci3 = 0;
                }
                else if (rD < gDP1Depth + 0.00000001m)
                {
                    ci3 = 1;
                }
                else
                {
                    ci3 = 2;
                }

                r2 = garOD[ci2];
                r3 = garID[ci3];
                rt = rt0 + rV * this.tempGrad;

                string[] arCasing = { "Casing", "Liner", "OpenHole" };
                string[] arComp = { "DP2", "DP1", "DC" };
                var annulus_section = arCasing[ci2];
                var str_comp = arComp[ci3];

                if (result.Count() > i_row)
                {
                    result[i_row].m_depth = rD;
                    result[i_row].inc_deg = rI;
                    result[i_row].v_depth = rV;
                    result[i_row].temp = rt;
                    result[i_row].section = annulus_section;
                    result[i_row].component = str_comp;
                    result[i_row].rough = rRoughness;
                    result[i_row].od = r2;
                    result[i_row].id = r3;
                }

                else
                {
                    result.Add(new ScenarioResult()
                    {
                        m_depth = rD,
                        inc_deg = rI,
                        v_depth = rV,
                        temp = rt,
                        section = annulus_section,
                        component = str_comp,
                        rough = rRoughness,
                        od = r2,
                        id = r3,
                    });
                }

                var d7 = r2;
                var d8 = r3;

                if ((d7 != d7x) || (d8 != d8x))
                {

                    rDpx = rDepth;
                    rPfr1x += rPfr1;
                    rPfr2x += rPfr2;
                    d7x = d7;
                    d8x = d8;
                }

                rDepth = rD;
                var rVDepth = rV;
                var rTemp = rt;
                var rAnnOD = r2;
                var rAnnID = r3;

                var ra = rAnnOD * rAnnOD - rAnnID * rAnnID;

                if (ra == 0)
                {
                    return result;
                }

                var rFlowArea = Convert.ToDecimal(Math.PI / 4 * Convert.ToDouble(ra));
                var rFlowDiameter = (rAnnOD - rAnnID) / 12;

                var rT = rTemp + rTemp0;

                if (rT == 0)
                {
                    return result;
                }

                var gSolidSG = this.solidSG;
                var gROP = this.ROP[loop];
                var gMW = this.MW;
                var gFormFluidSG = this.formationFluidSG;
                var gFormInfluxRate = this.influxRate[loop];
                var gGasSG = this.gasSG;
                var rAir = this.scfm[loop];
                var gGravity = this.grafity;

                ra = (
                    (0.00139m * gBitSize * gBitSize * gSolidSG * gROP)
                    + (0.246m * gMW * rLiquid)
                    + (1.43m * gFormFluidSG * gFormInfluxRate)
                    + (ra_fix1 * gGasSG * rAir)
                ) / (rT / 2 * rAir);

                var rb = ((0.033m * rLiquid) + (0.023m * gFormInfluxRate)) / (rT / 2 * rAir);
                var rc = 9.77m * rT / 2 * rAir / rFlowArea;
                var rd = ((0.33m * rLiquid) + (0.22m * gFormInfluxRate)) / rFlowArea;

                if (rVDepth == 0)
                {
                    Found(
                            i_row,
                            ref result,
                            ref MinMV,
                            ref Kinm,
                            this.gpm[loop],
                            bECD,
                            rP,
                            rSP,
                            rDepth,
                            rVDepth,
                            rRoughness,
                            rFlowArea,
                            ra,
                            rb,
                            rc,
                            rd,
                            gGravity
                        );
                }
                else
                {
                    var rLeft = ra * rVDepth;
                    var wIter = 0;

                    while (wIter < 100)
                    {
                        var rRight = 144 * rb * (rSP - rPsi) + Convert.ToDecimal(Math.Log(Convert.ToDouble(rSP / rPsi)));
                        var rDelta = Math.Abs(rRight - rLeft);
                        if (rDelta < 1e-9m)
                        {
                            break;
                        }
                        var rDerif = 144 * rb + (1 / rSP);
                        rSP = rSP - (rRight - rLeft) / rDerif;
                        wIter += 1;
                    }


                    r3 = 4.07m * rT * rAir;
                    var r4 = (rLiquid / 7.48m) + (5.615m * gFormInfluxRate / 60);

                    if (r4 == 0)
                    {
                        return result;
                    }

                    var r5 = Math.Pow(
                        1.74 - 2 * Math.Log10(Convert.ToDouble(2 * rRoughness / rFlowDiameter)), 2
                    );

                    if (r5 == 0)
                    {
                        return result;
                    }

                    var r6 = 2 * gGravity * rFlowDiameter;
                    if (r6 == 0)
                    {
                        return result;
                    }

                    var r7 = 0.632447m * Convert.ToDecimal(Math.Log10(Convert.ToDouble(rVDepth))) - 1.6499m;

                    var rx2 = ra * rc * rd * (rDepth - rDpx);
                    var rx3 = ra * rc * rc * (rDepth - rDpx);

                    rPfr2 = 0;
                    rPfr1 = 0;
                    var bFlag = 1;
                    for (int i = 1; i < 8; i++)
                    {
                        wIter = 0;
                        while (wIter < 100)
                        {
                            rP = 144 * (rSP + rPfr1 + rPfr1x + rPfr2 + rPfr2x);
                            var rGLR = r3 / ((gps + rP) * r4);
                            var rFLHU = Math.Pow(Convert.ToDouble(rGLR), Convert.ToDouble(r7));
                            var rf = Convert.ToDecimal(rFLHU / r5);
                            var re = rf / r6;
                            decimal rRight, rDerif;
                            if (bFlag == 1)
                            {
                                rRight = 5184 * rb * rPfr1 * rPfr1 + 72 * rPfr1;
                                rLeft = rx2 * re;
                                rDerif = (rRight - rLeft) / ((10368 * rb * rPfr1) + 72);
                            }
                            else
                            {
                                rRight = (
                                    1e6m * rb * Convert.ToDecimal(Math.Pow(Convert.ToDouble(rPfr2), 3))
                                    + 10368 * rPfr2 * rPfr2
                                );
                                rLeft = rx3 * re;
                                rDerif = (rRight - rLeft) / (
                                    192 * rPfr2 * (15625 * rb * rPfr2 + 108)
                                );
                            }

                            var rDelta = Math.Abs(rRight - rLeft);
                            if (rDelta < 1e-9m)
                            {
                                break;
                            }

                            if (bFlag == 1)
                            {
                                rPfr1 = rPfr1 - rDerif;
                            }
                            else
                            {
                                rPfr2 = rPfr2 - rDerif;
                            }
                            wIter += 1;
                        }

                        if (i == 1)
                        {
                            rPfr2 = rPfr1;
                        }
                        bFlag = 3 - bFlag;
                    }

                    Found(
                        i_row,
                        ref result,
                        ref MinMV,
                        ref Kinm,
                        this.gpm[loop],
                        bECD,
                        rP,
                        rSP,
                        rDepth,
                        rVDepth,
                        rRoughness,
                        rFlowArea,
                        ra,
                        rb,
                        rc,
                        rd,
                        gGravity
                    );
                }
            }
            catch ( Exception e )
            {
                break;
            }

            i_row++;
            rV_old = rV;
            rD_old = rD;
            rI_old = rI;
        }

        Console.WriteLine("APES");
        for ( int apes = 0; apes < result.Count; apes++)
        {
            Console.WriteLine(apes+" - M_depth :" + result[apes].m_depth);
            Console.WriteLine(apes+" - inc Deg :" + result[apes].inc_deg);
            Console.WriteLine(apes + " - inc Deg :" + result[apes].component);
        }
        return result;
        Console.WriteLine("==============");
        Console.WriteLine(result.Count);
    }

    private void Found(int i_row, ref List<ScenarioResult> wellResult, ref decimal MinMV, ref decimal Kinm, decimal gpm, decimal bECD, decimal rP, decimal rSP, decimal rDepth, decimal rVDepth, decimal rRoughness, decimal rFlowArea, decimal ra, decimal rb, decimal rc, decimal rd, decimal gGravity)
    {
        var rPpsi = rP / 144;
        decimal rECD = 0, rESD = 0, rECSD = 0, GLRWDynamic;
        if (rVDepth != 0)
        {
            rECD = rPpsi / (0.052m * rVDepth);
            rESD = rSP / 0.052m / rVDepth;
            rECSD = rECD - rESD;
        }

        var rMixDen = ra * rP / (rb * rP + 1);
        var rMixVel = (rc / rP) + rd;

        if (rMixVel < MinMV)
        {
            MinMV = rMixVel;
        }

        var rKin = rMixDen * rMixVel * rMixVel / (2 * gGravity);

        if (rKin < Kinm)
        {
            Kinm = rKin;
        }

        wellResult[i_row].rough = rRoughness;
        wellResult[i_row].p_dynamic = rPpsi;
        wellResult[i_row].ecd = rECD;
        wellResult[i_row].density = rMixDen / 7.48m;
        wellResult[i_row].velocity = rMixVel * 60;
        wellResult[i_row].kinetic = rKin;
        wellResult[i_row].p_static = rSP;
        wellResult[i_row].p_dynstat = rPpsi - rSP;
        wellResult[i_row].esd = rESD;
        wellResult[i_row].ecd_esd = rECSD;
        wellResult[i_row].cut_depth_lfr = rFlowArea;

        if (bECD == 0)
        {
            GLRWDynamic = rPpsi;
        }
        else
        {
            GLRWDynamic = rECD;
        }

        var GLRWStatic = rSP;
        var GLRWKin = Kinm;
        var GLRWECD = rECD;
        var GLRWESD = rESD;

        decimal newrFlowArea = 0;
        var newMinMV = MinMV * 60;
        var gMudFR = gpm;

        var n2 = i_row + 1;
        decimal r3 = 0m, r5 = 0m, r6 = 0m;

        while (n2 > 0)
        {
            n2--;
            var rDpx = wellResult[n2].m_depth;

            if (n2 > 0)
            {
                r3 = rDpx - wellResult[n2 - 1].m_depth;
            }
            else
            {
                r3 = 0;
            }

            var r4 = wellResult[n2].cut_depth_lfr * r3 * 12 * 0.004329m;
            newrFlowArea = newrFlowArea + r4;

            if (newMinMV != 0)
            {
                r5 = (rDepth - rDpx) / newMinMV;
            }
            else
            {
                r5 = 0;
            }
            if (gMudFR != 0)
            {
                r6 = newrFlowArea / gMudFR;
            }
            else
            {
                r6 = 0;
            }

            if (i_row == wellResult.Count() - 1)
            {
                wellResult[n2].cut_depth_lfr = r6;
            }
            wellResult[n2].cut_depth_vel = r5;
        }
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
        for (var lp = 0; lp < wellResult.Count;lp++)
        {
            Console.WriteLine(wellResult[lp].m_depth);
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
