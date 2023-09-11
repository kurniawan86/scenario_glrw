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
            Scenario1 gl = new Scenario1();
            gl.readExcelWellTrajectory();
        }
    }
}

