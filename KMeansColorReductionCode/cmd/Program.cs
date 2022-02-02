using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using _42Entwickler.ImageLib;
using Extensions;
using ImageColorReductionLib;

namespace KMeansColorReduction
{
    internal class Program
    {
        /// <summary>
        ///     Calls the imagelib to read a picture into a 3d byte array
        ///     and sets the properties to the correct inputted values
        /// </summary>
        /// <param name="cmdOptions">args[]</param>
        /// <returns></returns>
        private static byte[,,] ReadData(string[] cmdOptions)
        {
            switch (cmdOptions.Length)
            {
                case 1:
                    Config.ColorCount = Convert.ToInt32(cmdOptions[0]);
                    goto case 2;
                case 2:
                    Config.FileName = cmdOptions[1];
                    goto case 3;
                case 3:
                    Config.BatchSize = Convert.ToInt32(cmdOptions[2]);
                    goto case 4;
                case 4:
                    Config.PreClusterCount = Convert.ToInt32(cmdOptions[3]);
                    break;
            }

            Config.OutputFileName = "out" + Config.FileName[5] + "_" + Config.ColorCount + ".jpg";
            return ArrayImage.ReadAs3DArray(Config.FileName);
        }

        /// <summary>
        ///     This Function is for validating how many different colors there are in this picture
        /// </summary>
        /// <param name="picture">3d byte array of the picture to validate</param>
        /// <returns>count of different colors</returns>
        private static int GetColorCount(byte[,,] picture)

        {
            var ans = ParallelEnumerable.Range(0,
                    picture.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, picture.GetLength(1))
                    .Select(y => (picture[x, y, 0], picture[x, y, 1], picture[x, y, 2]))).ToList();
            return ans.Distinct().Count();
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("started program, reducing to: " + Config.ColorCount);
            byte[,,] inputArray = ReadData(args);


            Console.WriteLine("input image color count: " + GetColorCount(inputArray));
            Console.Write("Flattening Image to List... ");
            var rawPixels = ParallelEnumerable.Range(0,
                    inputArray.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, inputArray.GetLength(1))
                    .Select(y => (inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]))).ToList();

            Console.Write("ok.\nGetting clusters by distinct colors... ");
            //get distinct colors
            var distinctColors = rawPixels.GroupBy(x => x)
                .Where(g => g.Any())
                .Select(y => new Cluster(y.Key, y.Count())).Randomize().ToList();
            //distinctColors.Counter will equal de index of the current assigned centroid
            //calculating random centroids
            byte[,] centroids = KMeansClustering.GetCentroidsRandom(distinctColors);

            Console.WriteLine("ok. centroid count: " + centroids.GetLength(0));
            var reps = 0;
            var div = int.MaxValue;
            // this uses reps < 5. this means, regardless of the output or input, the algorithm has 5 iterations.
            // this is faster, looks the same, and doesn't have problems with infinite loops
            while (reps < 5) //(div > 60)
            {
                var oldSum = 0;
                for (var i = 0; i < centroids.GetLength(0); i++)
                    oldSum += centroids[i, 0] + centroids[i, 1] + centroids[i, 2];

                Console.Write("\n--------------------" + reps++ + "--------------------\n" + "Starting assignment... ");
                Stopwatch stp1 = new();
                stp1.Start();
                distinctColors = KMeansClustering.AssignColors(distinctColors, centroids);
                Console.Write("ok. Took: " + stp1.Elapsed + "\nstarting moving centroids... ");
                stp1.Restart();
                centroids = KMeansClustering.MoveCentroids(centroids, distinctColors);
                Console.WriteLine("ok. Took: " + stp1.Elapsed);

                var sum = 0;
                for (var i = 0; i < centroids.GetLength(0); i++)
                    sum += centroids[i, 0] + centroids[i, 1] + centroids[i, 2];

                Console.WriteLine("diff: " + Math.Abs(sum - oldSum));
                div = Math.Abs(sum - oldSum);
            }

            Console.Write("getting new image now... ");

            Stopwatch stp = new();
            stp.Start();
            //inputArray = GetNewImage(distinctColors, inputArray, centroids);
            inputArray = KMeansClustering.GetNewImageFast(distinctColors, inputArray, centroids);
            Console.WriteLine("ok. Took: " + stp.Elapsed);

            Console.Write("Number of colors: " + GetColorCount(inputArray) + "\nSaving Image... ");
            ArrayImage.Save(Config.OutputFileName, inputArray);
            Console.WriteLine("ok. saved to: " + Config.OutputFileName);
        }
    }
}