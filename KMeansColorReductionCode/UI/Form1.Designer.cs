
namespace KMeansColorReductionUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FileDialogButton = new System.Windows.Forms.Button();
            this.SelectedFileTextBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.StartButton = new System.Windows.Forms.Button();
            this.ColorCountTextBox = new System.Windows.Forms.TextBox();
            this.outputFileDialogButton = new System.Windows.Forms.Button();
            this.SelectedOutputFileTextBox = new System.Windows.Forms.TextBox();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.inPictureBox = new System.Windows.Forms.PictureBox();
            this.outPictureBox = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.inPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FileDialogButton
            // 
            this.FileDialogButton.Location = new System.Drawing.Point(713, 13);
            this.FileDialogButton.Name = "FileDialogButton";
            this.FileDialogButton.Size = new System.Drawing.Size(75, 23);
            this.FileDialogButton.TabIndex = 0;
            this.FileDialogButton.Text = "...";
            this.FileDialogButton.UseVisualStyleBackColor = true;
            this.FileDialogButton.Click += new System.EventHandler(this.FileDialogButton_Click);
            // 
            // SelectedFileTextBox
            // 
            this.SelectedFileTextBox.Location = new System.Drawing.Point(12, 14);
            this.SelectedFileTextBox.Name = "SelectedFileTextBox";
            this.SelectedFileTextBox.Size = new System.Drawing.Size(695, 23);
            this.SelectedFileTextBox.TabIndex = 1;
            this.SelectedFileTextBox.Text = "Input File";
            this.SelectedFileTextBox.Click += new System.EventHandler(this.SelectedFileTextBox_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(713, 70);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // ColorCountTextBox
            // 
            this.ColorCountTextBox.Location = new System.Drawing.Point(12, 71);
            this.ColorCountTextBox.Name = "ColorCountTextBox";
            this.ColorCountTextBox.Size = new System.Drawing.Size(694, 23);
            this.ColorCountTextBox.TabIndex = 3;
            this.ColorCountTextBox.Text = "Output Color Count";
            this.ColorCountTextBox.Click += new System.EventHandler(this.ColorCountTextBox_Click);
            // 
            // outputFileDialogButton
            // 
            this.outputFileDialogButton.Location = new System.Drawing.Point(713, 42);
            this.outputFileDialogButton.Name = "outputFileDialogButton";
            this.outputFileDialogButton.Size = new System.Drawing.Size(75, 23);
            this.outputFileDialogButton.TabIndex = 4;
            this.outputFileDialogButton.Text = "...";
            this.outputFileDialogButton.UseVisualStyleBackColor = true;
            this.outputFileDialogButton.Click += new System.EventHandler(this.outputFileDialogButton_Click);
            // 
            // SelectedOutputFileTextBox
            // 
            this.SelectedOutputFileTextBox.Location = new System.Drawing.Point(11, 42);
            this.SelectedOutputFileTextBox.Name = "SelectedOutputFileTextBox";
            this.SelectedOutputFileTextBox.Size = new System.Drawing.Size(695, 23);
            this.SelectedOutputFileTextBox.TabIndex = 5;
            this.SelectedOutputFileTextBox.Text = "Output File";
            this.SelectedOutputFileTextBox.Click += new System.EventHandler(this.SelectedOutputFileTextBox_Click);
            // 
            // statusTextBox
            // 
            this.statusTextBox.Location = new System.Drawing.Point(13, 101);
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ReadOnly = true;
            this.statusTextBox.Size = new System.Drawing.Size(693, 23);
            this.statusTextBox.TabIndex = 6;
            this.statusTextBox.Text = "Waiting for Input";
            // 
            // inPictureBox
            // 
            this.inPictureBox.Location = new System.Drawing.Point(11, 168);
            this.inPictureBox.Name = "inPictureBox";
            this.inPictureBox.Size = new System.Drawing.Size(383, 307);
            this.inPictureBox.TabIndex = 7;
            this.inPictureBox.TabStop = false;
            // 
            // outPictureBox
            // 
            this.outPictureBox.Location = new System.Drawing.Point(405, 168);
            this.outPictureBox.Name = "outPictureBox";
            this.outPictureBox.Size = new System.Drawing.Size(383, 307);
            this.outPictureBox.TabIndex = 8;
            this.outPictureBox.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(13, 131);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(693, 23);
            this.progressBar1.TabIndex = 9;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 487);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.outPictureBox);
            this.Controls.Add(this.inPictureBox);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.SelectedOutputFileTextBox);
            this.Controls.Add(this.outputFileDialogButton);
            this.Controls.Add(this.ColorCountTextBox);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.SelectedFileTextBox);
            this.Controls.Add(this.FileDialogButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.inPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FileDialogButton;
        private System.Windows.Forms.TextBox SelectedFileTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TextBox ColorCountTextBox;
        private System.Windows.Forms.Button outputFileDialogButton;
        private System.Windows.Forms.TextBox SelectedOutputFileTextBox;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.PictureBox inPictureBox;
        private System.Windows.Forms.PictureBox outPictureBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

