using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KILibrary
{
    public class Manhattan : IDistanceMetric
    {
        public double Calc(double[] vector1, double[] vector2)
        {
            double sum = 0.0;
            for (int i = 0; i < vector1.Length; i++)
                sum += Math.Abs(vector1[i] - vector2[i]);
            
            return sum;
        }
        public byte Calc((byte, byte, byte) vector1, (byte, byte, byte) vector2)
        {
            int sum = 0;
            sum += Math.Abs(vector1.Item1 - vector2.Item1);
            sum += Math.Abs(vector1.Item2 - vector2.Item2);
            sum += Math.Abs(vector1.Item3 - vector2.Item3);

            return Convert.ToByte(sum / 3);
        }
    }
}
