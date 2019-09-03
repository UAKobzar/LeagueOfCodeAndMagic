using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class Configuration
    {
        public static double[] DamageCost => new double[] { 0.703 };
        public static double[] HealthCost => new double[] { 0.008 };
        public static double[] BreakthroughCost => new double[] { 0.385 };
        public static double[] ChargeCost => new double[] { 0.684 };
        public static double[] DrainCost => new double[] { 0.321 };
        public static double[] GuardCost => new double[] { 0.460 };
        public static double[] LethalCost => new double[] { 0.529 };
        public static double[] WardCost => new double[] { 0.412 };
        public static double[] PlayerHPCost => new double[] { 0.327 };
        public static double[] EnemyHpCost => new double[] { -0.456 };
        public static double[] CardDrawCost => new double[] { 0.974 };
        public static double[] InitCost => new double[] { 1.057 };
    }
}
