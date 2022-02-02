using System;
using System.Collections.Generic;
using System.Linq;
using _42Entwickler.ImageLib;
using Extensions;
using ImageColorReductionLib;

namespace ImageColorReduction
{
    
    class Program
    {
        /// <summary>
        /// This Function is for validating how many different colors there are in this picture 
        /// </summary>
        /// <param name="picture">3d byte array of the picture to validate</param>
        /// <returns>count of different colors</returns>
        static int GetColorCount(byte[,,] picture)

        {
            var ans = ParallelEnumerable.Range(0, 
                picture.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, picture.GetLength(1))
                .Select(y => (picture[x, y, 0], picture[x, y, 1], picture[x, y, 2]))).ToList();
            return ans.Distinct().Count();
        }
        /// <summary>
        /// Calls the imagelib to read a picture into a 3d byte array
        /// and sets the properties to the correct inputted values
        /// </summary>
        /// <param name="cmdOptions">args[]</param>
        /// <returns></returns>
        static byte[,,] ReadData(string[] cmdOptions)
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


        static void Main(string[] args)
        {
            byte[,,] inputArray = ReadData(args);
            Console.WriteLine(inputArray.GetLength(0) + " <X [Image Size] Y> " + inputArray.GetLength(1));

            Console.WriteLine("input image color count: " + GetColorCount(inputArray));
            // clusters is list of cluster
            // cluster consists of the color (Element) and the count of the pixels inside (Counter)
            Console.Write("Flattening Image to List... ");
            var rawPixels = ParallelEnumerable.Range(0,
                inputArray.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, inputArray.GetLength(1))
                .Select(y => (inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]))).ToList();

            Console.Write("ok.\nGetting clusters by distinct colors... ");
            //get clusters by distinct colors
            var distinctClusters = rawPixels.GroupBy(x => x)
                            .Where(g => g.Any())
                            .Select(y => new Cluster(y.Key, y.Count())).Randomize().ToList();

            Console.Write("ok.\nGetting chunks... ");
            List<IEnumerable<Cluster>> clusterChunks = distinctClusters.Chunk(Config.BatchSize).ToList();

            Console.WriteLine("ok.\nStarting main clustering... ");
            var newClusters = Clustering.MainClustering(clusterChunks);

            Console.WriteLine("ok.\nStarting second clustering... ");
            newClusters = Clustering.SecondClustering(newClusters);

            Console.Write("ok.\nGetting new Image... ");
            inputArray = ClusteringHelper.GetClusteredImage(inputArray, newClusters);

            Console.Write("ok. Number of colors: "+ GetColorCount(inputArray) + "\nSaving Image... ");
            ArrayImage.Save(Config.OutputFileName, inputArray);
            Console.WriteLine("ok. saved to: " + Config.OutputFileName);
            Console.WriteLine("press 'y' for outputtting the remaining clusters");
            if (Console.ReadKey().KeyChar == 'y')
                ClusteringHelper.OutputClusteredColors(newClusters);
        }
       
    }
}
