using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageColorReductionLib
{
    public static class ClusteringHelper
    {
        /// <summary>
        /// rewrite the byte array of the picture from the resulting clusters => colors
        /// </summary>
        /// <param name="inputArray">the original image</param>
        /// <param name="c">the clusters</param>
        /// <returns></returns>
        public static byte[,,] GetClusteredImage(byte[,,] inputArray, List<Cluster> c)
        {
            Parallel.For(0, inputArray.GetLength(0), x =>
            {
                for (int y = 0; y < inputArray.GetLength(1); y++)
                {
                    int closestClusterIndex = 0;
                    byte minValue = byte.MaxValue;
                    for (int i = 0; i < c.Count; i++)
                    {
                        byte b = Config.distancemetric.Calc((inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]), c[i].Element);
                        if (b < minValue)
                        {
                            minValue = b;
                            closestClusterIndex = i;
                        }
                    }
                    inputArray[x, y, 0] = c[closestClusterIndex].Element.Item1;
                    inputArray[x, y, 1] = c[closestClusterIndex].Element.Item2;
                    inputArray[x, y, 2] = c[closestClusterIndex].Element.Item3;
                }
            });
            return inputArray;
        }

        /// <summary>
        /// Outputs the colors of the image and the count respectively
        /// </summary>
        /// <param name="clusters">clusters to output</param>
        public static void OutputClusteredColors(List<Cluster> clusters) 
        { 
            for(int i = 0; i < clusters.Count; i++)
                Console.WriteLine("RGB: " + clusters[i].Element + " PixelCount: " + clusters[i].Counter);
        }

        /// <summary>
        /// Recalculates the distance matrix for the Clustering algorithm, when there are changes to the clusters
        /// as parallel is not needed for the chunk calculation and would only complicate things
        /// </summary>
        /// <param name="distMatrix">distance matrix from before</param>
        /// <param name="minIndex">to get the indices where the distance matrix is updated</param>
        /// <param name="clusters">to recalculate the updated indices</param>
        /// <returns>updated distance matrix</returns>
        public static List<List<byte>> RecalculateDistMatrix(List<List<byte>> distMatrix, (int, int) minIndex, List<Cluster> clusters)
        {
            distMatrix.RemoveAt(minIndex.Item2);
            for (int x = 0; x < distMatrix.Count; x++)
            {
                distMatrix[x][minIndex.Item1] = Config.distancemetric.Calc(clusters[x].Element, clusters[minIndex.Item1].Element);
                distMatrix[x].RemoveAt(minIndex.Item2);
            }
            // spalte von Item1 neu berechnen:
            for (int y = 0; y < distMatrix.Count; y++)
                distMatrix[minIndex.Item1][y] = Config.distancemetric.Calc(clusters[minIndex.Item1].Element, clusters[y].Element);
            return distMatrix;
        }
        /// <summary>
        /// Recalculates the distance matrix for the Clustering algorithm, when there are changes to the clusters
        /// as parallel to speed up the final clustering
        /// </summary>
        /// <param name="distMatrix">distance matrix from before</param>
        /// <param name="minIndex">to get the indices where the distance matrix is updated</param>
        /// <param name="clusters">to recalculate the updated indices</param>
        /// <returns>updated distance matrix</returns>
        public static List<List<byte>> ParallelRecalculateDistMatrix(List<List<byte>> distMatrix, (int, int) minIndex, List<Cluster> clusters)
        {
            distMatrix.RemoveAt(minIndex.Item2);
            Parallel.For(0, distMatrix.Count, x =>
            {
                distMatrix[x][minIndex.Item1] = Config.distancemetric.Calc(clusters[x].Element, clusters[minIndex.Item1].Element);
                distMatrix[x].RemoveAt(minIndex.Item2);
            });
            // spalte von Item1 neu berechnen:
            Parallel.For(0, distMatrix.Count, y =>
            {
                distMatrix[minIndex.Item1][y] = Config.distancemetric.Calc(clusters[minIndex.Item1].Element, clusters[y].Element);
            });
            return distMatrix;
        }

        /// <summary>
        /// Merge 2 clusters with each other
        /// </summary>
        /// <param name="color1">color from cluster 1</param>
        /// <param name="color2">color from cluster 2</param>
        /// <param name="count1">pixelcount from cluster 1</param>
        /// <param name="count2">pixelcount from cluster 2</param>
        /// <returns>new color</returns>
        public static (byte, byte, byte) MergeColors((byte, byte, byte) color1, (byte, byte, byte) color2, int count1, int count2)
            => (Convert.ToByte((color1.Item1 * count1 + count2 * color2.Item2) / (count1 + count2)),
                Convert.ToByte((color1.Item2 * count1 + count2 * color2.Item2) / (count1 + count2)),
                Convert.ToByte((color1.Item1 * count1 + count2 * color2.Item2) / (count1 + count2)));

        /// <summary>
        /// get the distance matrix for the appropiate clusters
        /// </summary>
        /// <param name="clusters">clusters from which the distance matrix should be calculated</param>
        /// <returns>distance matrix</returns>
        public static List<List<byte>> GetDistanceMatrix(List<Cluster> clusters)
        {
            List<List<byte>> dist_matrix = new();
            for (int i = 0; i < clusters.Count; i++)
                dist_matrix.Add(new List<byte>(new byte[clusters.Count]));
            Parallel.For(0, clusters.Count, x =>
            {
                for (int y = 0; y < clusters.Count; y++)
                {
                    dist_matrix[x][y] = Config.distancemetric.Calc(clusters[x].Element, clusters[y].Element);
                }
            });
            return dist_matrix;
        }

        /// <summary>
        /// calculates the index for the minimum in the distance matrix
        /// </summary>
        /// <param name="dist_matrix">the distance matrix from which the index should be found</param>
        /// <param name="lastMin">the last minimum value that was found. speeds up the calculation by a lot</param>
        /// <returns>index where the value is lowest</returns>
        public static (int, int) GetMinDistFromMatrix(List<List<byte>> dist_matrix, ref int lastMin)
        {
            byte minimum = byte.MaxValue;
            (int, int) min_index = (0, 0);
            for (int x = 0; x < dist_matrix.Count; x++)
            {
                byte local_min = dist_matrix[x].Where((v, idx) => idx != x).Min();
                if (local_min < minimum)
                {
                    minimum = local_min;
                    min_index = (x, dist_matrix[x].IndexOf(local_min, x + 1));
                }
                if (minimum <= lastMin)
                    break;
            }
            if (minimum > lastMin) // lastMin is used for breaking up the for loops prematurely when there is no other distance that is less
                lastMin = minimum;
            return min_index;
        }
    }
}
