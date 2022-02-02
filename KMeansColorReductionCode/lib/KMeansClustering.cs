using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ImageColorReductionLib
{
    public static class KMeansClustering
    {

        /// <summary>
        /// is here because it's comparatively very fast
        /// this needs about half a second for the 1920x1920 picture, GetNewImage() takes over 5 minutes
        /// </summary>
        /// <param name="distinctColors">the unique colors</param>
        /// <param name="inputArray">the original image that is overwritten</param>
        /// <param name="centroids">the clusters</param>
        /// <returns></returns>
        public static byte[,,] GetNewImageFast(List<Cluster> distinctColors, byte[,,] inputArray, byte[,] centroids)
        {

            Parallel.For(0, inputArray.GetLength(0), x => //looping through every pixel
                                                          // for(int x = 0; x < inputArray.GetLength(0); x++)
            {

                for (var y = 0; y < inputArray.GetLength(1); y++)
                {
                    //searching for closest centroid, should work more or less the same, but faster:
                    int correctIndex = 0;
                    byte closest = byte.MaxValue;
                    for (int i = 0; i < centroids.GetLength(0); i++)
                    {
                        byte dist = Config.distancemetric.Calc((centroids[i, 0], centroids[i, 1], centroids[i, 2]),
                            (inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]));

                        if (closest < dist) continue;
                        closest = dist;
                        correctIndex = i;
                    }
                    //assigning
                    inputArray[x, y, 0] = centroids[correctIndex, 0];
                    inputArray[x, y, 1] = centroids[correctIndex, 1];
                    inputArray[x, y, 2] = centroids[correctIndex, 2];
                }
            });
            return inputArray;
        }

        /// <summary>
        /// Get random centroids
        /// </summary>
        /// <param name="distinctColors">from unique colors</param>
        /// <returns>random centroids</returns>
        public static byte[,] GetCentroidsRandom(List<Cluster> distinctColors)
        {
            Random rnd = new();
            var centroids = new byte[Config.ColorCount, 3];
            for (var i = 0; i < centroids.GetLength(0); i++)
            {
                int rndNumber = rnd.Next(0, distinctColors.Count);
                centroids[i, 0] = Convert.ToByte(distinctColors[rndNumber].Element.Item1);
                centroids[i, 1] = Convert.ToByte(distinctColors[rndNumber].Element.Item2);
                centroids[i, 2] = Convert.ToByte(distinctColors[rndNumber].Element.Item3);
            }

            return centroids;
        }

        /// <summary>
        /// Assigns the unique colors to the according centroids
        /// Looks for the nearest centroid and writes its index to distinctColors[i].Counter
        /// </summary>
        /// <param name="distinctColors">the clusters to assign</param>
        /// <param name="centroids">the newly moved centroids</param>
        /// <returns>new assignments of the new clusters</returns>
        public static List<Cluster> AssignColors(List<Cluster> distinctColors, byte[,] centroids)
        {
            //assign colors to centroids
            //foreach (var color in distinctColors)
            Parallel.ForEach(distinctColors, color =>
            {
                // getting shortest distance to centroid
                var minDist = double.MaxValue;
                var minDistIndex = 0;
                for (var i = 0; i < centroids.GetLength(0); i++)
                {
                    byte dist = Config.distancemetric.Calc((centroids[i, 0], centroids[i, 1], centroids[i, 2]),
                        color.Element);
                    if (!(dist < minDist)) continue;
                    minDist = dist;
                    minDistIndex = i;
                }

                color.Counter = minDistIndex;
            });

            return distinctColors;
        }

        /// <summary>
        /// moves the centroids to the new mean value of the new clusters
        /// </summary>
        /// <param name="centroids">the centroids that are overwritten</param>
        /// <param name="distinctColors">the newly assigned unique clusters</param>
        /// <returns>the new centroids</returns>
        public static byte[,] MoveCentroids(byte[,] centroids, List<Cluster> distinctColors)
        {
            
            Random rnd = new();
            //move centroids to the center
            //for (var i = 0; i < centroids.GetLength(0); i++)
            Parallel.For(0, centroids.GetLength(0), i =>
            {
                (double, double, double) sums = (0, 0, 0);
                var counter = 0;
                foreach (var currentColor in distinctColors.Where(currentColor => currentColor.Counter == i))
                {
                    sums.Item1 += currentColor.Element.Item1;
                    sums.Item2 += currentColor.Element.Item2;
                    sums.Item3 += currentColor.Element.Item3;
                    counter++;
                }

                if (counter != 0)
                {
                    centroids[i, 0] = Convert.ToByte(sums.Item1 / counter);
                    centroids[i, 1] = Convert.ToByte(sums.Item2 / counter);
                    centroids[i, 2] = Convert.ToByte(sums.Item3 / counter);
                }
                else
                {
                    int rndNumber = rnd.Next(0, distinctColors.Count);
                    centroids[i, 0] = Convert.ToByte(distinctColors[rndNumber].Element.Item1);
                    centroids[i, 1] = Convert.ToByte(distinctColors[rndNumber].Element.Item2);
                    centroids[i, 2] = Convert.ToByte(distinctColors[rndNumber].Element.Item3);
                }
            });
            return centroids;
        }

        /// <summary>
        /// slower, but correct version of GetNewImageFast()
        /// </summary>
        /// <param name="distinctColors">the unique colors</param>
        /// <param name="inputArray">the original image that is overwritten</param>
        /// <param name="centroids">the clusters</param>
        /// <returns>the changed image</returns>
        public static byte[,,] GetNewImage(List<Cluster> distinctColors, byte[,,] inputArray, byte[,] centroids)
        {
            //hashing distinct colors for more speed
            ulong[] arr = distinctColors
                .Select(x => CalculateHash($"{x.Element.Item1}{x.Element.Item2}{x.Element.Item3}"))
                .ToArray();
            Parallel.For(0, inputArray.GetLength(0), x => //looping through every pixel
                                                          // for(int x = 0; x < inputArray.GetLength(0); x++)
            {
                var stp = new Stopwatch();
                stp.Start();
                for (var y = 0; y < inputArray.GetLength(1); y++)
                {
                    //searching for the according color
                    int correctIndex =
                        distinctColors[Array.IndexOf(arr,
                            CalculateHash($"{inputArray[x, y, 0]}{inputArray[x, y, 1]}{inputArray[x, y, 2]}"))].Counter;

                    //assigning
                    inputArray[x, y, 0] = centroids[correctIndex, 0];
                    inputArray[x, y, 1] = centroids[correctIndex, 1];
                    inputArray[x, y, 2] = centroids[correctIndex, 2];
                }

                stp.Stop();
                Console.WriteLine(stp.Elapsed);
            });
            return inputArray;
        }

        /// <summary>
        /// calculates the hash => is much faster than comparing tuples of 3 bytes
        /// </summary>
        /// <param name="read">the input string that the hash is calculated of</param>
        /// <returns>returns hash in UInt64</returns>
        private static ulong CalculateHash(string read)
        {
            var hashedValue = 3074457345618258791ul;
            foreach (char c in read)
            {
                hashedValue += c;
                hashedValue *= 3074457345618258799ul;
            }

            return hashedValue;
        }
    }
}