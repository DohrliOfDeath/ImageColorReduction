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
using _42Entwickler.ImageLib;
using Extensions;

namespace ImageColorReductionUI
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
            if(ColorCountTextBox.Text != "" && int.TryParse(ColorCountTextBox.Text, out int txt))
            {
                Config.ColorCount = txt;
            }
            return ArrayImage.ReadAs3DArray(Config.FileName);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void FileDialogButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && (_ = openFileDialog1.OpenFile()) != null)
                SelectedFileTextBox.Text = openFileDialog1.FileName;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            byte[,,] inputArray = ReadData(); 
            inPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            MyImage = new Bitmap(Config.FileName);
            inPictureBox.Image = MyImage;

            statusTextBox.Text = inputArray.GetLength(0) + " <X [Image Size] Y> " + inputArray.GetLength(1);

            statusTextBox.Text = "input image color count: " + GetColorCount(inputArray);
            // clusters is list of cluster
            // cluster consists of the color (Element) and the count of the pixels inside (Counter)
            statusTextBox.Text = "Flattening Image to List... ";
            var rawPixels = ParallelEnumerable.Range(0,
                inputArray.GetLength(0))
                .SelectMany(x => ParallelEnumerable.Range(0, inputArray.GetLength(1))
                .Select(y => (inputArray[x, y, 0], inputArray[x, y, 1], inputArray[x, y, 2]))).ToList();

            statusTextBox.Text = "ok.Getting clusters by distinct colors... ";
            //get clusters by distinct colors
            var distinctClusters = rawPixels.GroupBy(x => x)
                            .Where(g => g.Any())
                            .Select(y => new Cluster(y.Key, y.Count())).Randomize().ToList();

            statusTextBox.Text = "ok.Getting chunks... ";
            List<IEnumerable<Cluster>> clusterChunks = distinctClusters.Chunk(Config.BatchSize).ToList();

            statusTextBox.Text ="ok.Starting main clustering... ";
            var newClusters = Clustering.MainClustering(clusterChunks);

            statusTextBox.Text = "ok.\nStarting second clustering... ";
            newClusters = Clustering.SecondClustering(newClusters);

            statusTextBox.Text = "ok.Getting new Image... ";
            inputArray = ClusteringHelper.GetClusteredImage(inputArray, newClusters);

            statusTextBox.Text = "ok. Number of colors: " + GetColorCount(inputArray) + "Saving Image... ";
            ArrayImage.Save(Config.OutputFileName, inputArray);
            statusTextBox.Text = "finished. saved to: " + Config.OutputFileName;

            outPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            MyImage = new Bitmap(Config.OutputFileName);
            outPictureBox.Image = MyImage;


        }

        private void outputFileDialogButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && (_ = openFileDialog1.OpenFile()) != null)
                SelectedOutputFileTextBox.Text = openFileDialog1.FileName;
        }
    }
}
