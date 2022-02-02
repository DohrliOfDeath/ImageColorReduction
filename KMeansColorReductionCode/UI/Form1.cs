using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageColorReductionLib;
using System.Windows.Forms;
using System.Threading;
using _42Entwickler.ImageLib;
using Extensions;

namespace KMeansColorReductionUI
{
    public partial class Form1 : Form
    {
        private Bitmap MyImage;
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
        byte[,,] ReadData()
        {
            if (SelectedOutputFileTextBox.Text != "")
                Config.OutputFileName = "out" + Config.FileName[5] + "_" + Config.ColorCount + ".jpg";
            else
                Config.OutputFileName = SelectedOutputFileTextBox.Text;
            if (SelectedFileTextBox.Text != "")
                Config.FileName = SelectedFileTextBox.Text;
            if (ColorCountTextBox.Text != "" && int.TryParse(ColorCountTextBox.Text, out int txt))
            {
                Config.ColorCount = txt;
            }
            return ArrayImage.ReadAs3DArray(Config.FileName);
        }

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;  //reporting progress for progressbar
        }

        /// <summary>
        /// opens the file dialog and sets the corresponding settings (iamge)
        /// </summary>
        private void FileDialogButton_Click(object sender, EventArgs e)
        { 
            if (openFileDialog1.ShowDialog() == DialogResult.OK && (_ = openFileDialog1.OpenFile()) != null)
                SelectedFileTextBox.Text = openFileDialog1.FileName;

            inPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            MyImage = new Bitmap(openFileDialog1.FileName);
            inPictureBox.Image = MyImage;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();  // starts the calculation
        }

        /// <summary>
        /// opens another file dialog
        /// </summary>
        private void outputFileDialogButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && (_ = openFileDialog1.OpenFile()) != null)
                SelectedOutputFileTextBox.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// the following methods clear the corresponding textboxes onclick
        /// this makes entering new paths easier
        /// </summary>
        private void SelectedFileTextBox_Click(object sender, EventArgs e)=>
            SelectedFileTextBox.Text = "";
        private void SelectedOutputFileTextBox_Click(object sender, EventArgs e) =>
            SelectedOutputFileTextBox.Text = "";
        private void ColorCountTextBox_Click(object sender, EventArgs e) =>
            ColorCountTextBox.Text = "";

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[,,] inputArray = ReadData(); // gets data from input
            //rearrange the byte array to a usable list
            var rawPixels = ParallelEnumerable.Range(0,
                inputArray.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, inputArray.GetLength(1))
                .Select(y => (inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]))).ToList();

            //get distinct colors
            var distinctColors = rawPixels.GroupBy(x => x)
                            .Where(g => g.Any())
                            .Select(y => new Cluster(y.Key, y.Count())).Randomize().ToList();

            byte[,] centroids = KMeansClustering.GetCentroidsRandom(distinctColors);

            Console.WriteLine("ok. centroid count: " + centroids.GetLength(0));
            var reps = 0;
            var div = int.MaxValue;
            // this uses reps < 5. this means, regardless of the output or input, the algorithm has 5 iterations.
            // this is faster, looks the same, and doesn't have problems with infinite loops
            while (reps < 5) //(div > 60)
            {
                backgroundWorker1.ReportProgress(reps);
                var oldSum = 0;
                for (var i = 0; i < centroids.GetLength(0); i++)
                    oldSum += centroids[i, 0] + centroids[i, 1] + centroids[i, 2];

                Console.Write("\n--------------------" + reps++ + "--------------------\n" + "Starting assignment... ");

                distinctColors = KMeansClustering.AssignColors(distinctColors, centroids);
                Console.Write("ok.\nstarting moving centroids... ");

                centroids = KMeansClustering.MoveCentroids(centroids, distinctColors);
                Console.WriteLine("ok.");

                var sum = 0;
                for (var i = 0; i < centroids.GetLength(0); i++)
                    sum += centroids[i, 0] + centroids[i, 1] + centroids[i, 2];

                Console.WriteLine("diff: " + Math.Abs(sum - oldSum));
                div = Math.Abs(sum - oldSum);
            }

            Console.Write("getting new image now... ");
            inputArray = KMeansClustering.GetNewImageFast(distinctColors, inputArray, centroids);

            ArrayImage.Save(Config.OutputFileName, inputArray);
           // OutputPicture = inputArray;
        }
        
        // starting the progressbar
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 0)
                progressBar1.Style = ProgressBarStyle.Marquee;
        }

        // stopping to progressbar
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            outPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            MyImage = new Bitmap(Config.OutputFileName);
            outPictureBox.Image = MyImage;
        }
    }
}
