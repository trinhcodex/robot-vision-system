using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Imaging;
using System.Net.Http;

namespace WinFormCVU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            pictureBox1.AllowDrop = true;
            pictureBox2.AllowDrop = true;
            textBox2.Text = "0.4";
            textBox5.Text = "-1";
            textBox4.Text = "1";
            textBox10.Text = "5";
        }

        string targetImagePath;
        string templatePath;
        //string pythonPath = "/home/kratos/.conda/envs/capstone/bin/python";

        string defaultDirectory = $"{Application.StartupPath}\\";
        string stream_folder = "Stream_camera";
        string output_folder = "Output";

        //string shell = @"C:\windows\system32\cmd.exe";
        //string shell = "wsl.exe";

        private bool isSelecting = false;
        private Rectangle rect;
        private Point startPoint;
        private int cornerSize = 7;

        private bool isResizing = false;

        async Task<string> SendRequest(string url, Dictionary<string, string> formFields)
        {
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    foreach (var field in formFields)
                    {
                        formData.Add(new StringContent(field.Value), field.Key);
                    }

                    var response = await client.PostAsync(url, formData);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string imagePath = files[0];
                pictureBox1.ImageLocation = imagePath;
                string fullTemplatePath = imagePath;
                templatePath = get_relativePath(fullTemplatePath, defaultDirectory);

                string templatePathCopy = Path.Combine(Path.GetDirectoryName(templatePath), "copy_" + Path.GetFileName(templatePath));
                File.Copy(templatePath, templatePathCopy, true);

                ShowImage(templatePathCopy, pictureBox1);

                File.Delete(templatePathCopy);
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void pictureBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (file.Length == 1 && (Path.GetExtension(file[0]).ToLower() == ".png" || Path.GetExtension(file[0]).ToLower() == ".jpg" || Path.GetExtension(file[0]).ToLower() == ".jpeg" || Path.GetExtension(file[0]).ToLower() == ".bmp" || Path.GetExtension(file[0]).ToLower() == ".gif"))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (file.Length > 0 && file != null)
            {
                string imagePath = file[0];
                string fullTargetImagePath = imagePath;
                targetImagePath = get_relativePath(fullTargetImagePath, defaultDirectory);

                string targetImagePathCopy = Path.Combine(Path.GetDirectoryName(targetImagePath), "copy_" + Path.GetFileName(targetImagePath));
                File.Copy(targetImagePath, targetImagePathCopy, true);

                ShowImage(targetImagePathCopy, pictureBox2);

                File.Delete(targetImagePathCopy);
            }
        }

        private void LoadCsvToDataGridView(string csvFilePath, DataGridView dataGridView)
        {
            if (!File.Exists(Path.Combine(defaultDirectory, csvFilePath)))
            {
                throw new ArgumentException("The specified csv file does not exist.", nameof(csvFilePath));
            }

            // Create a new DataTable
            DataTable dataTable = new DataTable();

            // Read the CSV file line by line
            string[] csvLines = File.ReadAllLines(Path.Combine(defaultDirectory, csvFilePath));

            // Add the column headers to the DataTable
            string[] headers = csvLines[0].Split(',');
            foreach (string header in headers)
            {
                dataTable.Columns.Add(header);
            }

            // Add the data rows to the DataTable
            for (int i = 1; i < csvLines.Length; i++)
            {
                string[] fields = csvLines[i].Split(',');
                dataTable.Rows.Add(fields);
            }

            // Bind the DataTable to the DataGridView
            dataGridView.DataSource = dataTable;
        }

        private void ShowImage(string imagePath, PictureBox pictureBox)
        {
            if (!File.Exists(Path.Combine(defaultDirectory, imagePath)))
            {
                throw new ArgumentException("The specified image file does not exist.", nameof(imagePath));
            }

            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
                pictureBox.Image = null;
            }

            using (Image image = Image.FromFile(Path.Combine(defaultDirectory, imagePath)))
            {
                if (image.PropertyIdList.Contains(0x0112)) // Check if the image has orientation metadata
                {
                    int orientation = (int)image.GetPropertyItem(0x0112).Value[0];
                    switch (orientation)
                    {
                        case 2: // Flip horizontally
                            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3: // Rotate 180 degrees
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4: // Flip vertically
                            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 5: // Rotate 90 degrees clockwise and flip horizontally
                            image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6: // Rotate 90 degrees clockwise
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7: // Rotate 90 degrees clockwise and flip vertically
                            image.RotateFlip(RotateFlipType.Rotate90FlipY);
                            break;
                        case 8: // Rotate 270 degrees clockwise
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        default:
                            break;
                    }
                }

                pictureBox.Image = new Bitmap(image);
                percentageScale(pictureBox);
            }
        }


        private void percentageScale(PictureBox pictureBox) 
        {
            if (pictureBox.Name != "pictureBox2")
            {
                return;
            }

            float originalWidth = pictureBox.Image.Width;
            float originalHeight = pictureBox.Image.Height;
            float aspectRatio = originalWidth / originalHeight;

            float resizedWidth = pictureBox.Width;
            float resizedHeight = pictureBox.Height;

            if (resizedWidth / resizedHeight > aspectRatio)
            {
                resizedWidth = resizedHeight * aspectRatio;
            }
            else
            {
                resizedHeight = resizedWidth / aspectRatio;
            }

            float widthPercentage = resizedWidth / originalWidth * 100;
            float heightPercentage = resizedHeight / originalHeight * 100;

            textBox3.Text = $"{(int)Math.Round(widthPercentage)}%";
        }

        //private void runScriptPython(string command, string shell)
        //{
        //    Process process = new Process();
        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    startInfo.FileName = shell;
        //    startInfo.WorkingDirectory = defaultDirectory;
        //    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    //startInfo.Arguments = $"/c \"{command}\"";
        //    startInfo.Arguments = command;
        //    process.StartInfo = startInfo;

        //    process.Start();
        //    process.WaitForExit();
        //}

        private string get_relativePath(string path, string defaultPaht)
        {
            Uri fullUri = new Uri(path);
            Uri defaultUri = new Uri(defaultPaht);
            string relativePath = Uri.UnescapeDataString(defaultUri.MakeRelativeUri(fullUri).ToString());
            return relativePath;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //string scriptPath = "cvu_program/cvu_program.exe";
            //string scriptPath = "main.py";
            //string threshold = textBox1.Text;
            //string overlap = textBox2.Text;
            //string method = comboBox1.Text;
            //string min_modify = textBox5.Text;
            //string max_modify = textBox4.Text;
            //string enhance_algorithms_path = textBox8.Text;
            //string representation_algorithms_path = textBox9.Text;
            //string command = $"\"{scriptPath}\" --img_path \"{targetImagePath}\" --template_path \"{templatePath}\" --threshold {threshold} --overlap {overlap} --method \"{method}\" --min_modify {min_modify} --max_modify {max_modify} --enhance \"{enhance_algorithms_path}\" --representation \"{representation_algorithms_path}\"";
            //string command = $"{pythonPath} \"{scriptPath}\" --img_path \"{targetImagePath}\" --template_path \"{templatePath}\" --threshold {threshold} --overlap {overlap} --method \"{method}\" --min_modify {min_modify} --max_modify {max_modify} --enhance \"{enhance_algorithms_path}\" --representation \"{representation_algorithms_path}\"";
            //MessageBox.Show(command);
            //runScriptPython(command, shell);

            MessageBox.Show(templatePath);

            var formFields = new Dictionary<string, string>
            {
                {"api_folder", defaultDirectory},
                {"img_path", targetImagePath},
                {"template_path", templatePath},
                {"threshold", textBox1.Text},
                {"overlap", textBox2.Text},
                {"method", comboBox1.Text},
                {"min_modify", textBox5.Text},
                {"max_modify", textBox4.Text},
                {"enhance", textBox8.Text},
                {"representation", textBox9.Text},
                {"output_folder", output_folder}
            };

            var _ = await SendRequest("http://127.0.0.1:5000/my_cvu_api", formFields);

            string resultImagePath = Path.Combine(output_folder, "output.jpg");
            string csvPath = Path.Combine(output_folder, "result.csv");

            try
            {
                ShowImage(resultImagePath, pictureBox2);
                LoadCsvToDataGridView(csvPath, dataGridView1);
            }
            catch (Exception)
            {
                MessageBox.Show("No detection found");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string cameraPath = Path.Combine(stream_folder, "input_image.png");

            //string cameraPath = "Stream_camera/input_image.png";
            //string scriptPath = "camera_program/camera_program.exe";
            //string scriptPath = "camera.py";
            //string ip_address = textBox6.Text;
            //string command = $"{pythonPath} \"{scriptPath}\" --ip \"{ip_address}\"";
            //MessageBox.Show(command);
            //runScriptPython(command, shell);

            var formFields = new Dictionary<string, string>
            {
                {"api_folder", defaultDirectory},
                {"output_folder", stream_folder},
                {"ip_address", textBox6.Text}
            };

            var _ = await SendRequest("http://127.0.0.1:5001/my_camera_api", formFields);

            try
            {
                ShowImage(cameraPath, pictureBox2);
            }
            catch (Exception)
            {
                MessageBox.Show("No connection with camera");
            }

            targetImagePath = cameraPath;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            string streamTemplatePath = Path.Combine(stream_folder, "template.png");
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = false;

                // save the selected area to file
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Bitmap bitmap = new Bitmap(pictureBox2.ClientSize.Width, pictureBox2.ClientSize.Height);
                    pictureBox2.DrawToBitmap(bitmap, pictureBox2.ClientRectangle);

                    Bitmap croppedBitmap = bitmap.Clone(rect, bitmap.PixelFormat);
                    croppedBitmap.Save(Path.Combine(defaultDirectory, streamTemplatePath), ImageFormat.Png);
                }
            }
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                int x = Math.Min(startPoint.X, e.Location.X);
                int y = Math.Min(startPoint.Y, e.Location.Y);
                int width = Math.Abs(startPoint.X - e.Location.X);
                int height = Math.Abs(startPoint.Y - e.Location.Y);

                rect = new Rectangle(x, y, width, height);
                pictureBox2.Invalidate();
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (isSelecting && rect.Width > 0 && rect.Height > 0)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, rect);

                    int rectX = rect.X;
                    int rectY = rect.Y;
                    int rectWidth = rect.Width;
                    int rectHeight = rect.Height;

                    // Draw small rectangles at each corner of the rect
                    e.Graphics.FillRectangle(Brushes.White, rectX - cornerSize, rectY - cornerSize, cornerSize * 2, cornerSize * 2);
                    e.Graphics.FillRectangle(Brushes.Black, rectX - cornerSize + 1, rectY - cornerSize + 1, cornerSize * 2 - 2, cornerSize * 2 - 2);
                    e.Graphics.FillRectangle(Brushes.White, rectX + rectWidth - cornerSize, rectY - cornerSize, cornerSize * 2, cornerSize * 2);
                    e.Graphics.FillRectangle(Brushes.Black, rectX + rectWidth - cornerSize + 1, rectY - cornerSize + 1, cornerSize * 2 - 2, cornerSize * 2 - 2);
                    e.Graphics.FillRectangle(Brushes.White, rectX - cornerSize, rectY + rectHeight - cornerSize, cornerSize * 2, cornerSize * 2);
                    e.Graphics.FillRectangle(Brushes.Black, rectX - cornerSize + 1, rectY + rectHeight - cornerSize + 1, cornerSize * 2 - 2, cornerSize * 2 - 2);
                    e.Graphics.FillRectangle(Brushes.White, rectX + rectWidth - cornerSize, rectY + rectHeight - cornerSize, cornerSize * 2, cornerSize * 2);
                    e.Graphics.FillRectangle(Brushes.Black, rectX + rectWidth - cornerSize + 1, rectY + rectHeight - cornerSize + 1, cornerSize * 2 - 2, cornerSize * 2 - 2);
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            string streamTemplatePath = Path.Combine(stream_folder, "template.png");

            try
            {
                ShowImage(streamTemplatePath, pictureBox1);
            }
            catch (Exception)
            {
                MessageBox.Show("Please select a region of interest first");
            }

            templatePath = streamTemplatePath;
        }

        private void updateScrollBarPositions()
        {
            // Calculate the difference between the old and new size of the picture box
            int deltaX = pictureBox2.Width - hScrollBar1.Maximum;
            int deltaY = pictureBox2.Height - vScrollBar1.Maximum;

            // Check if the picture box is smaller than the panel and adjust the scroll bar values accordingly
            if (deltaX < 0)
            {
                hScrollBar1.Value = 0;
                deltaX = 0;
            }
            if (deltaY < 0)
            {
                vScrollBar1.Value = 0;
                deltaY = 0;
            }

            // Calculate the new position of the scroll bars based on the difference
            int newHValue = Math.Max(-pictureBox2.Location.X, 0);
            int newVValue = Math.Max(-pictureBox2.Location.Y, 0);

            // Update the scrollbars' maximum values to reflect the new size of the picturebox
            hScrollBar1.Maximum = Math.Max(pictureBox2.Width - panel13.Width, 0);
            vScrollBar1.Maximum = Math.Max(pictureBox2.Height - panel13.Height, 0);

            // Make sure the scrollbars' values are still within their maximum range
            hScrollBar1.Value = Math.Min(newHValue, hScrollBar1.Maximum);
            vScrollBar1.Value = Math.Min(newVValue, vScrollBar1.Maximum);

            // Update the position of the picturebox based on the scrollbar values
            pictureBox2.Location = new Point(Math.Max(-hScrollBar1.Value, 6), Math.Max(-vScrollBar1.Value, 21));
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox2.Top = -e.NewValue;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox2.Left = -e.NewValue;
        }

        private void pictureBox2_Resize(object sender, EventArgs e)
        {
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            updateScrollBarPositions();
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            pictureBox2.Left = -hScrollBar1.Value;
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            pictureBox2.Top = -vScrollBar1.Value;
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        { 

            isResizing = true;
            int scaleFactor = Convert.ToInt32(textBox10.Text);
            while (isResizing)
            {
                float aspectRatio = (float)pictureBox2.Image.Width / (float)pictureBox2.Image.Height;
                int newWidth = pictureBox2.Width - (int)(scaleFactor * 1);
                int newHeight = (int)(newWidth / aspectRatio);

                Point oldCenter = new Point(pictureBox2.Location.X + pictureBox2.Width / 2,
                                             pictureBox2.Location.Y + pictureBox2.Height / 2);

                pictureBox2.Size = new Size(newWidth, newHeight);

                Point newCenter = new Point(oldCenter.X + (pictureBox2.Width - newWidth) / 2,
                                             oldCenter.Y + (pictureBox2.Height - newHeight) / 2);

                pictureBox2.Location = new Point(newCenter.X - pictureBox2.Width / 2,
                                                 newCenter.Y - pictureBox2.Height / 2);

                percentageScale(pictureBox2);
                updateScrollBarPositions();
                Application.DoEvents();
            }
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {   
            isResizing = true;
            int scaleFactor = Convert.ToInt32(textBox10.Text);
            while (isResizing)
            {
                float aspectRatio = (float)pictureBox2.Image.Width / (float)pictureBox2.Image.Height;
                int newWidth = pictureBox2.Width + (int)(scaleFactor * 1);
                int newHeight = (int)(newWidth / aspectRatio);

                Point oldCenter = new Point(pictureBox2.Location.X + pictureBox2.Width / 2,
                                             pictureBox2.Location.Y + pictureBox2.Height / 2);

                pictureBox2.Size = new Size(newWidth, newHeight);

                Point newCenter = new Point(oldCenter.X - (newWidth - pictureBox2.Width) / 2,
                                             oldCenter.Y - (newHeight - pictureBox2.Height) / 2);

                pictureBox2.Location = new Point(newCenter.X - pictureBox2.Width / 2,
                                                 newCenter.Y - pictureBox2.Height / 2);

                percentageScale(pictureBox2);
                updateScrollBarPositions();
                Application.DoEvents();
            }
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of the OpenFileDialog class
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set properties for the dialog
            openFileDialog1.InitialDirectory = defaultDirectory;
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            // Show the dialog and check if the user clicked the OK button
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the file name and do whatever you need to do with it
                string fileName = openFileDialog1.FileName;
                string relativePath = get_relativePath(fileName, defaultDirectory);
                textBox8.Text = relativePath;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Create an instance of the OpenFileDialog class
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set properties for the dialog
            openFileDialog1.InitialDirectory = defaultDirectory;
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            // Show the dialog and check if the user clicked the OK button
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the file name and do whatever you need to do with it
                string fileName = openFileDialog1.FileName;
                string relativePath = get_relativePath(fileName, defaultDirectory);
                textBox9.Text = relativePath;
            }
        }

        private void ResizePictureBox(float percentage)
        {
            float originalWidth = pictureBox2.Image.Width;
            float originalHeight = pictureBox2.Image.Height;
            float aspectRatio = originalWidth / originalHeight;

            float resizedWidth = originalWidth * (percentage / 100);
            float resizedHeight = resizedWidth / aspectRatio;

            pictureBox2.Width = (int)resizedWidth;
            pictureBox2.Height = (int)resizedHeight;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(textBox3.Text.Replace("%", ""), out float percentage))
            {
                ResizePictureBox(percentage);
            }
        }
    }
}
