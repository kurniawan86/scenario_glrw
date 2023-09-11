using System;

namespace scenario_glrw
{
    class ScenarioGLRW
    {
        public ScenarioGLRW()
        {
            Console.WriteLine("tes constructore");
        }
        
        static void Main()
        {
            ScenarioGLRW s = new ScenarioGLRW();
            Scenario1 sc7 = new Scenario1();
            Configuration config = new Configuration();
            config.readExcelWellTrajectory();
            config.readExcelReservoil();

            //pola 7
            sc7.solidSG = 2.65m;
            sc7.MW = 8.7m;
            sc7.formationFluidSG = 0.9m;
            sc7.grafity = 32.17405m;
            sc7.gasSG = 1;
            sc7.absTemp = 80;
            sc7.tempGrad = 0.05m;
            sc7.rougnessSteel = 0.0018m;
            sc7.rougnessHole = 0.048m;

        }
    }
}

