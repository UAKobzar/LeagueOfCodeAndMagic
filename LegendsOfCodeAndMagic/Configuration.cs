using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class Configuration
    {
        public static double[] DamageCost => new double[] { 0.538605881408985, 0.700744447985581, -0.616757560590087, 0, };
        public static double[] HealthCost => new double[] { 0.472076187175127, 0.329644066468276, -0.0412775080451156, -0.702175160089566, };
        public static double[] BreakthroughCost => new double[] { 0.174210749990308, 0.0640366415544007, 0.0220183273985539, 0, };
        public static double[] ChargeCost => new double[] { 0.310918020175352, 0.146047254408051, 0.0220183273985539, 0, };
        public static double[] DrainCost => new double[] { 0.181710337282304, 0.0724615230722244, 0.0220183273985539, 0, };
        public static double[] GuardCost => new double[] { 0.0898405136454066, 0.0890588076412464, -0.167302072062705, 0, };
        public static double[] LethalCost => new double[] { 0.174206815970283, 0.369193689347353, 0.0220183273985539, 0, };
        public static double[] WardCost => new double[] { 0.203701292075281, 0.577715207780982, 0.0220183273985539, 0, };
        public static double[] PlayerHPCost => new double[] { 0.218243991143658, 0.242302147897888, 0, 0.352177998922807, };
        public static double[] EnemyHpCost => new double[] { -0.215770919213012, 0, -0.520133017388523, -0.444016649193787, };
        public static double[] CardDrawCost => new double[] { 0.142196394721955, 0.273721805360335, 1.31073602443807, 0.500520392032127, };
        public static double[] InitCostCost => new double[] { 0.0993329123289335, 0.719592791722195, 1.27685889525721, 0.56099951575609, };

    }
}
