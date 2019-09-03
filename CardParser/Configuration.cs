using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class Configuration
    {
        public static double[] DamageCost => new double[] { {0}  };
        public static double[] HealthCost => new double[] { {1}  };
        public static double[] BreakthroughCost => new double[] { {2} };
        public static double[] ChargeCost => new double[] { {3} };
        public static double[] DrainCost => new double[] { {4} };
        public static double[] GuardCost => new double[] { {5} };
        public static double[] LethalCost => new double[] { {6} };
        public static double[] WardCost => new double[] { {7} };
        public static double[] PlayerHPCost => new double[] { {8} };
        public static double[] EnemyHpCost => new double[] { {9} };
        public static double[] CardDrawCost => new double[] { {10} };
        public static double[] InitCost => new double[] { {11} };
    }
}
