using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using _42Entwickler.ImageLib;
using KILibrary;

namespace ImageColorReductionOld
{
    class Cluster
    {
        public (byte, byte, byte) Element { get; set; } 
        public int Counter { get; set; }
        public Cluster((byte, byte, byte) element, int counter)
        {
            Counter = counter;
            Element = element;
        }
    }
    class Program
    {
        static Manhattan distancemetric = new Manhattan();
        static int ColorCount { get; set; } = 155;
        static string FileName { get; set; } = "image1.jpg";
        static string OutputFileName { get; set; } = "out0.jpg";
        static int GetColorCount(byte[,,] picture)
        {
            List<(byte, byte, byte)> ans = new List<(byte, byte, byte)>();
           
            for (int x = 0; x < picture.GetLength(0); x++)
                for (int y = 0; y < picture.GetLength(1); y++)
                    ans.Add((picture[x, y, 0], picture[x, y, 1], picture[x, y, 2]));
            return ans.Distinct().Count();
        }
        static void MainOld(string[] args)
        {
            if (args.Length == 2)
            {
                ColorCount = Convert.ToInt32(args[0]);
                FileName = args[1];
            }
            ColorCount += 2;
            byte[,,] inputArray = ArrayImage.ReadAs3DArray(FileName);
            Console.WriteLine(inputArray.GetLength(0) + " <X [Image Size] Y> " + inputArray.GetLength(1));

            Console.WriteLine("input image color count: " + GetColorCount(inputArray));
            // clusters is list of cluster
            // cluster consists of the color (Element) and the count of the pixels inside (Counter)
            List<(byte, byte, byte)> rawPixels = new();
            for (int x = 0; x < inputArray.GetLength(0); x++)
                for (int y = 0; y < inputArray.GetLength(1); y++)
                    rawPixels.Add((inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]));

            //get clusters by distinct colors
            List<Cluster> c = rawPixels.GroupBy(x => x)
                            .Where(g => g.Any())
                            .Select(y => new Cluster(y.Key, y.Count()))
                            .ToList();
            //without distance matrix: a lot slower (more than 2 hours for 200x300 image)
            //But it doesn't use that much RAM => comparatively nothing, dist_matrix needs about 100GB for the 1920x1920 picture
            /*while(c.Count > 5000)
            {
                (int minimum, (int, int) min_index) = GetMinimumDist(c);
                //merge 2 clusters, then delete the cloned one
                c[min_index.Item1].Counter += c[min_index.Item2].Counter;
                c[min_index.Item1].Element = MergeColors(c[min_index.Item1].Element, c[min_index.Item2].Element);
                c.RemoveAt(min_index.Item2);

                Console.WriteLine(c.Count + " : " + minimum + " " + min_index);
            }*/
            List<List<byte>> dist_matrix = GetDistanceMatrix(c);
            int last_min = 2;
            while (c.Count > ColorCount)
            {
                (int, int) min_index = GetMinDistFromMatrix(dist_matrix, ref last_min);
                //merge 2 clusters, then delete the cloned one
                c[min_index.Item1].Counter += c[min_index.Item2].Counter;
                c[min_index.Item1].Element = MergeColors(c[min_index.Item1].Element, c[min_index.Item2].Element, c[min_index.Item1].Counter, c[min_index.Item2].Counter);
                c.RemoveAt(min_index.Item2);
                dist_matrix = RecalculateDistMatrix(dist_matrix, min_index, c);
                Console.WriteLine(c.Count + " : " + " " + min_index);
            }


            inputArray = GetClusteredImage(inputArray, c);
            Console.WriteLine(GetColorCount(inputArray));
            ArrayImage.Save(OutputFileName, inputArray);
        }

        private static byte[,,] GetClusteredImage(byte[,,] inputArray, List<Cluster> c)
        {
            for (int x = 0; x < inputArray.GetLength(0); x++)
            {
                for (int y = 0; y < inputArray.GetLength(1); y++)
                {
                    int closestClusterIndex = 0;
                    byte minValue = byte.MaxValue;
                    for (int i = 0; i < c.Count; i++)
                    {
                        byte b = distancemetric.Calc((inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]), c[i].Element);
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
            }
            return inputArray;
        }

        private static List<List<byte>> RecalculateDistMatrix(List<List<byte>> distMatrix, (int, int) minIndex, List<Cluster> clusters)
        {
            distMatrix.RemoveAt(minIndex.Item2);
            Parallel.For(0, distMatrix.Count, x =>
            {
                distMatrix[x][minIndex.Item1] = distancemetric.Calc(clusters[x].Element, clusters[minIndex.Item1].Element);
                distMatrix[x].RemoveAt(minIndex.Item2);
            });
            // spalte von Item1 neu berechnen:
            Parallel.For(0, distMatrix.Count, y =>
            {
                distMatrix[minIndex.Item1][y] = distancemetric.Calc(clusters[minIndex.Item1].Element, clusters[y].Element);
            });
            return distMatrix;
        }

        private static (byte, byte, byte) MergeColors((byte, byte, byte) color1, (byte, byte, byte) color2, int count1, int count2)
            => (Convert.ToByte((color1.Item1*count1 + count2*color2.Item2) / (2 * (count1 + count2))),
                Convert.ToByte((color1.Item2*count1 + count2*color2.Item2) / (2 * (count1 + count2))), 
                Convert.ToByte((color1.Item1*count1 + count2*color2.Item2) / (2 * (count1 + count2))));

        /* without distance matrix
        private static (int, (int, int)) GetMinDist(List<Cluster> c)
        {
            int minimum = int.MaxValue;
            (int, int) min_index = (0, 0);
            for (int x = 0; x < c.Count; x++)
            {

                Parallel.For(0, c.Count, (y, pls) =>
                {
                    int d = distancemetric.Calc(c[x].Element, c[y].Element);
                    if (d < minimum && d != 0)
                    {
                        minimum = d;
                        min_index = (x, y);
                    }
                });
                if (minimum < 2)
                    break;
            }
            return (minimum, min_index);
        }
        */
        static List<List<byte>> GetDistanceMatrix(List<Cluster> clusters)
        {
            List<List<byte>> dist_matrix = new List<List<byte>>();
            for (int i = 0; i < clusters.Count; i++)
                dist_matrix.Add(new List<byte>(new byte[clusters.Count]));
            Parallel.For(0, clusters.Count, x =>
            {
                for (int y = 0; y < clusters.Count; y++)
                {
                    dist_matrix[x][y] = distancemetric.Calc(clusters[x].Element, clusters[y].Element);
                }
            });
            return dist_matrix;
        }
        static (int, int) GetMinDistFromMatrix(List<List<byte>> dist_matrix, ref int lastMin)
        {
            byte minimum = byte.MaxValue;
            (int, int) min_index = (0, 0);
            for (int x = 0; x < dist_matrix.Count; x++)
            {
                byte local_min = dist_matrix[x].Where(x => x != 0).Min();
                if (local_min < minimum)
                {
                    minimum = local_min;
                    min_index = (x, dist_matrix[x].IndexOf(local_min));
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