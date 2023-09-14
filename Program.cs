using Microsoft.Extensions.Configuration;
using System;

namespace scenario_glrw
{
    class ScenarioGLRW
    {
        public ScenarioGLRW()
        {
            //Console.WriteLine("tes constructore");
        }
        
        static void Main()
        {
            ScenarioGLRW s = new ScenarioGLRW();
            
            Configuration config = new Configuration();
            config.readExcelWellTrajectory();
            config.readExcelReservoil();
            config.Casing_shoe_depth = 2624.67m;
            config.Top_liner_depth = 2460.63m;
            config.Liner_shoe = 6233.6m;
            config.Hole_depth = 6236.88m;
            config.Dp1_length = 118.77m;
            config.Dc_lengt = 34.35m;
            config.Bit_depth = 6236.88m;
            config.Dp2_od = 5;
            config.Dp1_od = 6.5m;
            config.Dc_od = 8;
            config.Casing_id = 12.415m;
            config.Bit_size = 9.875m;
            config.Liner_id = 10.05m;

            //pola 7
            Scenario sc7 = new Scenario(config);
            sc7.solidSG = 2.65m;
            sc7.MW = 8.7m;
            sc7.formationFluidSG = 0.9m;
            sc7.grafity = 32.17405m;
            sc7.gasSG = 1;
            sc7.absTemp = 80;
            sc7.tempGrad = 0.05m;
            sc7.rougnessSteel = 0.0018m;
            sc7.rougnessHole = 0.048m;
            sc7.ROP.Add(15.5m);
            sc7.ROP.Add(15.5m);
            sc7.ROP.Add(15.5m);
            sc7.ROP.Add(15.5m);
            sc7.ROP.Add(15.5m);
            sc7.gpm.Add(800);
            sc7.gpm.Add(800);
            sc7.gpm.Add(800);
            sc7.gpm.Add(800);
            sc7.gpm.Add(800);
            sc7.influxRate.Add(0);
            sc7.influxRate.Add(0);
            sc7.influxRate.Add(0);
            sc7.influxRate.Add(0);
            sc7.influxRate.Add(0);
            sc7.scfm.Add(1200);
            sc7.scfm.Add(1600);
            sc7.scfm.Add(1800);
            sc7.scfm.Add(2000);
            sc7.scfm.Add(2200);
            sc7.surfacePres.Add(14.7m);
            sc7.surfacePres.Add(14.7m);
            sc7.surfacePres.Add(14.7m);
            sc7.surfacePres.Add(14.7m);
            sc7.surfacePres.Add(14.7m);

            //sc7.calculateAll();

            // object GLWR
            GLRW glrw7 = new GLRW(sc7);
            glrw7.cbx = 0;
            glrw7.gFormInfluxRate = 1995.03m;
            glrw7.maxECD = 6.744m;
            glrw7.minSP = 1695.03m;
            glrw7.washL = 1050;
            glrw7.minK = 3;
            glrw7.rl1 = 0;
            glrw7.rl2 = 1250;
            glrw7.ra1 = 0;
            glrw7.ra2 = 5000;

            glrw7.calculateGLRW();
        }
    }
}

