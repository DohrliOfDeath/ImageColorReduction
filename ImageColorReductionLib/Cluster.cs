using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageColorReductionLib
{
    /// <summary>
    /// this class should have been a struct, but structs don't work the way they do in c++
    /// </summary>
    public class Cluster
    {
        /// <summary>
        /// Element equals the Color of the cluster, as in a tuple of 3 byte Elements that range from 0 to 255
        /// </summary>
        public (byte, byte, byte) Element { get; set; }
        /// <summary>
        /// Counter is the amount of pixels that are inside this cluster
        /// </summary>
        public int Counter { get; set; }
        public Cluster((byte, byte, byte) color, int pixelcount)
        {
            this.Counter = pixelcount;
            this.Element = color;
        }
    }
}
