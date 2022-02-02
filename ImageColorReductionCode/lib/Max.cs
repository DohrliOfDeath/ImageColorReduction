using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KILibrary
{
    public class Max : IDistanceMetric
    {
        public double Calc(double[] vector1, double[] vector2)
        {
            double lastMax = 0;
            for (int i = 0; i < vector1.Length; i++)
                if (vector1[i] - vector2[i] > lastMax)
                    lastMax = Math.Abs(vector1[i] - vector2[i]);

            return lastMax;
        }
    }
}
