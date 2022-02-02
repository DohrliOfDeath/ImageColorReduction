using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageColorReductionLib
{
    public static class Clustering
    {
        /// <summary>
        /// new clustering to get to the correct amount of colors
        /// </summary>
        /// <param name="newClusters">input Clusters that are built by the MainClustering Method</param>
        /// <returns>newClusters, finished calculating</returns>
        public static List<Cluster> SecondClustering(List<Cluster> newClusters)
        {
            List<List<byte>> distMatrix = ClusteringHelper.GetDistanceMatrix(newClusters);
            int lastMin = 2;
            while (newClusters.Count > Config.ColorCount + 1)
            {
                (int, int) min_index = ClusteringHelper.GetMinDistFromMatrix(distMatrix, ref lastMin);
                //merge 2 clusters, then delete the cloned one
                newClusters[min_index.Item1].Counter += newClusters[min_index.Item2].Counter;
                newClusters[min_index.Item1].Element = ClusteringHelper.MergeColors(
                    newClusters[min_index.Item1].Element,
                    newClusters[min_index.Item2].Element,
                    newClusters[min_index.Item1].Counter,
                    newClusters[min_index.Item2].Counter);

                newClusters.RemoveAt(min_index.Item2);
                distMatrix = ClusteringHelper.ParallelRecalculateDistMatrix(distMatrix, min_index, newClusters);
                Console.WriteLine(newClusters.Count + " : " + min_index);
            }

            return newClusters;
        }

        /// <summary>
        /// where the main clusters are built in chunks => parallelised
        /// </summary>
        /// <param name="clusterChunks">clusterchunks to process</param>
        /// <returns>list of new clusters</returns>
        public static List<Cluster> MainClustering(List<IEnumerable<Cluster>> clusterChunks)
        {
            List<Cluster> newClusters = new();
            Parallel.For(0, clusterChunks.Count, x =>
            {
                List<Cluster> currentClusters = clusterChunks[x].ToList();
                List<List<byte>> dist_matrix = ClusteringHelper.GetDistanceMatrix(currentClusters);
                int last_min = 2;
                while (currentClusters.Count > Config.PreClusterCount)
                {
                    (int, int) min_index = ClusteringHelper.GetMinDistFromMatrix(dist_matrix, ref last_min);
                    //merge 2 clusters, then delete the cloned one
                    currentClusters[min_index.Item1].Counter += currentClusters[min_index.Item2].Counter;
                    currentClusters[min_index.Item1].Element = ClusteringHelper.MergeColors(
                        currentClusters[min_index.Item1].Element,
                        currentClusters[min_index.Item2].Element,
                        currentClusters[min_index.Item1].Counter,
                        currentClusters[min_index.Item2].Counter);

                    currentClusters.RemoveAt(min_index.Item2);
                    dist_matrix = ClusteringHelper.RecalculateDistMatrix(dist_matrix, min_index, currentClusters);
                }
                newClusters.AddRange(currentClusters);
                double percent = Math.Round(Convert.ToDouble(newClusters.Count) / Convert.ToDouble(clusterChunks.Count * Config.PreClusterCount) * 100, 2);
                Console.WriteLine($"{percent, 5}%| current chunk: {x, 6} newClusterCount: {newClusters.Count}");
            });
            return newClusters;
        }
    }
}
