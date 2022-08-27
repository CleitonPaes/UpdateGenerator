using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Gerador_De_Atualização
{
    public partial class lForm : Form
    {
        private string[] Files;

        public lForm()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            StartBrowsing();

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Files = GetFiles(e.Argument);

            for (int i = 0; i < Files.Length; i++)
            {
                backgroundWorker.ReportProgress(i + 1, GetFileData(Files[i]));
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateResult(e.UserState);

            UpdateProgressBar(ComputeProgress(e.ProgressPercentage));
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableButtons();
        }

        private void DisableButtons()
        {
            Progress.Value = 0;
            Result.Clear();

            browseButton.Enabled = false;
        }

        private void EnableButtons()
        {

            browseButton.Enabled = true;
        }

        public string[] GetFiles(object Path)
        {
            return Directory.GetFiles(Path.ToString(), "*.*", System.IO.SearchOption.AllDirectories);
        }

        public string GetFileData(string File)
        {
            FileInfo fileInfo = new FileInfo(File);

            return File + " " + GetHash(File) + " " + fileInfo.Length;
        }

        private string GetHash(string Name)
        {
            if (Name == string.Empty)
            {
                return null;
            }

            CRC crc = new CRC();

            string Hash = string.Empty;

            try
            {
                using (FileStream fileStream = File.Open(Name, FileMode.Open))
                {
                    foreach (byte b in crc.ComputeHash(fileStream))
                    {
                        Hash += b.ToString("x2").ToLower();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Can't open: " + Name);
            }

            return Hash;
        }

        private void UpdateResult(object Data)
        {
            if (!Result.IsDisposed)
            {
                Result.AppendText(Data.ToString().Replace(@"\", "/") + Environment.NewLine);
            }
        }

        private int ComputeProgress(int Percent)
        {
            return (100 * Percent) / Files.Length;
        }

        private void UpdateProgressBar(int Percent)
        {
            if (Percent < 0 || Percent > 100)
            {
                return;
            }

            if (!Progress.IsDisposed)
            {
                Progress.Value = Percent;
            }
            if (Percent == 100)
            {
                SaveList();
            }
        }

        private void StartBrowsing()
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DisableButtons();

                if (File.Exists(folderBrowserDialog.SelectedPath+"\\update.txt")) // Deleta o arquivo do update anterior
                {
                    try
                    {
                        System.IO.File.Delete(folderBrowserDialog.SelectedPath+"\\update.txt");
                    }
                    catch (System.IO.IOException eita)
                    {
                        Console.WriteLine(eita.Message);
                        return;
                    }
                }

                filePath.Text = folderBrowserDialog.SelectedPath.Replace(@"\", "/") + "/";

                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync(folderBrowserDialog.SelectedPath);
                }
            }
        }

        private void SaveList()
        {

            if (File.Exists("att.txt")) // Deleta arquivo incompleto de um processo interrompido
            {
                try
                {
                    System.IO.File.Delete("att.txt");
                }
                catch (System.IO.IOException eita)
                {
                    Console.WriteLine(eita.Message);
                    return;
                }
            }

            if (File.Exists("update.txt")) // Deleta arquivo incompleto de um processo interrompido
            {
                try
                {
                    System.IO.File.Delete("update.txt");
                }
                catch (System.IO.IOException eita)
                {
                    Console.WriteLine(eita.Message);
                    return;
                }
            }

            using (StreamWriter streamWriter = new StreamWriter("att.txt", true))
            {
                streamWriter.Write(Result.Text);
            }

            StreamReader reader = new StreamReader("att.txt"); // le o arquivo previamente criado
            string input = reader.ReadToEnd();

            using (StreamWriter writer = new StreamWriter("update.txt", true)) // Reescreve o arquivo no formato final
            {
                {
                    string output = input.Replace(filePath.Text, "");
                    writer.Write(output);
                }
                reader.Close();
                writer.Close();
            }

            if (File.Exists("att.txt")) // Deleta o arquivo que foi previamente trabalhado
            {
                try
                {
                    System.IO.File.Delete("att.txt");
                }
                catch (System.IO.IOException eita)
                {
                    Console.WriteLine(eita.Message);
                    return;
                }
            }

            if (File.Exists("update.txt")) // Move o arquivo para a pasta de atualização
            {
                try
                {
                    System.IO.File.Move("update.txt", folderBrowserDialog.SelectedPath+"\\update.txt");
                }
                catch (System.IO.IOException eita)
                {
                    Console.WriteLine(eita.Message);
                    return;
                }
            }

            DialogResult mBoxResult = MessageBox.Show("Atualização gerada na pasta selecionada", "Gerador de Atualização");


        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void Result_TextChanged(object sender, EventArgs e)
        {

        }

        private void lForm_Load(object sender, EventArgs e)
        {

        }

        private void filePath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
