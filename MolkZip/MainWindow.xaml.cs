using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Forms;

namespace MolkZip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> filesList = new List<string>();
        //Assumes Visual Studio project folder structure
        static readonly string projectRootDir =
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Files_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filename in files)
                {
                    if (!filesList.Contains(filename))
                    {
                        filesList.Add(filename);

                        ListBoxItem file = new ListBoxItem();
                        file.Content = filename;
                        //FilesList.Items.Add(file);
                    }

                }
            }
        }

        public static void RunCLIprogram(string programPath, List<string> args)
        {
            //Combine list elements into a single string, where each
            //element is surrounded by quotes and separated by spaces.
            //Done mostly to handle args (such as file paths) that contain spaces.
            StringBuilder commandLineArgs = new StringBuilder();
            commandLineArgs.Append("\"");
            commandLineArgs.Append(String.Join("\" \"", args));
            commandLineArgs.Append("\"");
            Debug.WriteLine(commandLineArgs.ToString());
            System.Diagnostics.Process.Start(programPath, commandLineArgs.ToString());
        }

        private void Molk(object sender, RoutedEventArgs e)
        {
            string molkPath = projectRootDir + "molk.exe";
            string destFilePath = projectRootDir + "archive.molk"; //TODO: don't hardcode this.
            List<string> args = new List<string>();
            //-j flag makes it so that the archive contains *just* the file,
            //instead of mimicking the entire folder structure of the file's full path.
            args.Add("-j");
            args.Add(destFilePath);
            args.AddRange(filesList);
            RunCLIprogram(molkPath, args);
        }

        private void Unmolk(object sender, RoutedEventArgs e)
        {
            string destinationFolder = "";

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)

            {
                destinationFolder = dialog.SelectedPath;
            }
            
            string unmolkPath = projectRootDir + "unmolk.exe";
            string sourceFilePath = projectRootDir + "archive.molk"; //TODO: don't hardcode this.
            Debug.WriteLine(projectRootDir);
            List<string> args = new List<string>();
            args.Add(sourceFilePath);
            args.Add($"-d {destinationFolder}");
            RunCLIprogram(unmolkPath, args);
        }

    }
}
