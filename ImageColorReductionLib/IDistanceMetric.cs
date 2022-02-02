using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KILibrary
{
    interface IDistanceMetric
    {
        public double Calc(double[] vector1, double[] vector2);
    }
}
