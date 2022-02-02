using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KILibrary
{
    public class Euclid : IDistanceMetric
    {
        public double Calc(double[] vector1, double[] vector2)
        {
            double sum = 0.0;
            for(int i = 0; i < vector1.Length; i++)
            {
                sum += Math.Pow(vector1[i] - vector2[i], 2);
            }
            return Math.Sqrt(sum);
        }
    }
}
