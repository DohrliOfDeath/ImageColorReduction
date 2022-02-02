using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KILibrary;

namespace ImageColorReductionLib
{
    public static class Config
    {
        /// <summary>
        /// basically using the IDistanceMetric Interface to Manhattan as a Singleton
        /// </summary>
        public static readonly Manhattan distancemetric = new();
        /// <summary>
        /// InputOptions, also possible to input from args[]
        /// ColorCount: The Amount of different Colors the output should have
        /// BatchSize: How big one chunk is. Optimal for speed and memory efficiency turned out to be about 1000
        /// PreClusterCount: How many Clusters are used per chunk. Can determine how good the selected colors are.
        /// </summary>
        public static int ColorCount { get; set; } = 16;
        public static int BatchSize { get; set; } = 1000;
        public static int PreClusterCount { get; set; } = 10;
        public static string FileName { get; set; } = "image0.jpg";
        public static string OutputFileName { get; set; } = "out0.jpg";
    }
}
