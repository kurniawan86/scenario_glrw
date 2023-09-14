using System;

public class GLRW
{
	public decimal gROP;
	public decimal gMudFR;
	public decimal gFormInfluxRate;
	public decimal gGasFR;
	public decimal gPS;
	public decimal dL;
	public decimal dA;

	public decimal minSP;
	public decimal washL;
	public decimal minK;
	public int cbx;

	public decimal rl1;
	public decimal rl2;
	public decimal ra1;
	public decimal ra2;

	public decimal maxP;
	public decimal maxECD;

	int height = 753;
	int width = 909;
	public GLRW(Scenario scenario)
	{
		this.gROP = scenario.ROP[0];
		this.gMudFR = scenario.gpm[0];
		this.gFormInfluxRate = scenario.influxRate[0];
		this.gGasFR = scenario.scfm[0];
		this.gPS = scenario.surfacePres[0];
	}

	public void calculateGLRW()
	{
		gPS = gPS * 144;
		int beECD;

		if (cbx == 0)
		{
			if (maxECD > 0)
			{
				maxECD = 0;
			}
			beECD = 0;
		}
		if (cbx == 1)
		{
			if (maxP > 0)
			{
				maxP = 0;
			}
			beECD = 1;
		}

		var minL = decimal.ToInt16(rl1);
		var maxL = decimal.ToInt16(rl2);
		var minA = decimal.ToInt16(ra1);
		var maxA = decimal.ToInt16(ra2);
		var iWX = width;
		var iWY = height;

        var dxGLRW = ((int)(0.5 + (maxA - minA) / (iWX / 8)));
        var dyGLRW = ((int)(0.5 + (maxL - minL) / (iWY / 8)));
		
		if (dxGLRW < 1)
		{
			dxGLRW = 1;
		}
        if (dyGLRW < 1)
        {
            dyGLRW = 1;
        }
        if (minA == 0)
		{
			minA = (short)dxGLRW;
		}
		if (minL == 0)
		{
			minL = (short)dyGLRW;
		}

		var bmX = (int)(0.5 + 7 * iWX / (maxA - minA)*dxGLRW/10);
        var bmY = (int)(0.5 + 7 * iWY / (maxL - minL) * dyGLRW / 10);

		while (dL < maxL)
		{
			

			while (dA < maxA)
			{

                Console.WriteLine("X axis : " + dA);
                dA = dA + dxGLRW;

			}
            Console.WriteLine("Y axis : " + dL);
            dL = dL + dyGLRW;
		}
        Console.WriteLine("X axis : " + dA);
        Console.WriteLine("Y axis : " + dL);
    }
}
