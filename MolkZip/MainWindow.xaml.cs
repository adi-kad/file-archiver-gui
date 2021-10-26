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
using Microsoft.Win32;

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
            
            labelTip.Visibility = Visibility.Hidden;
            remove.Visibility = Visibility.Visible;
           
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
                        FilesList.Items.Add(file);

                        
                    }                    
                    

                    }


                }
            }
        }


        private void browse(object sender, RoutedEventArgs e)
        {
            addFiles();
        }

        private void removeFile(object sender, RoutedEventArgs e)
        {
                FilesList.Items.Remove(FilesList.SelectedItem);
                hideRemoveButton();

        }

        private void FilesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
               
                FilesList.Items.Remove(FilesList.SelectedItem);
                hideRemoveButton();
            }
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("Do you want to close this window?",
                    "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    mainWindow.Close(); 
                }
                else
                {
                    // Do not close the window  
                }
            }
            else if (e.Key == Key.Insert)
            {
                addFiles();
            }
        }

        private void hideRemoveButton()
        {
            if (FilesList.Items.Count == 0)
            {
                remove.Visibility = Visibility.Hidden;
                labelTip.Visibility = Visibility.Visible;
            }
        }
        private void addFiles()
        {

            OpenFileDialog newDiolog = new OpenFileDialog();
            newDiolog.ShowDialog();
            string fileName = newDiolog.FileName;
            if (fileName != "")
            {
                FilesList.Items.Add(fileName);
                labelTip.Visibility = Visibility.Hidden;
            }
           
            if (FilesList.Items.Count > 0)
            {
                remove.Visibility = Visibility.Visible;
            }
        }

        private void addFile_MouseMove(object sender, MouseEventArgs e)
        {
            addFile.Background = Brushes.Magenta;
        }

        private void addFile_MouseEnter(object sender, MouseEventArgs e)
        {
            addFile.FontSize = 18;
            addFile.FontWeight = FontWeights.Bold;
        }

        private void addFile_MouseLeave(object sender, MouseEventArgs e)
        {
            addFile.FontSize = 16;
            addFile.FontWeight = FontWeights.Normal;
        }

        private void remove_MouseEnter(object sender, MouseEventArgs e)
        {
            remove.FontSize = 18;
            remove.FontWeight = FontWeights.Bold;
        }

        private void remove_MouseLeave(object sender, MouseEventArgs e)
        {
            remove.FontSize = 16;
            remove.FontWeight = FontWeights.Bold;

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
            string unmolkPath = projectRootDir + "unmolk.exe";
            string sourceFilePath = projectRootDir + "archive.molk"; //TODO: don't hardcode this.
            List<string> args = new List<string>();
            args.Add(sourceFilePath);
            args.Add("-d testFolder");
            RunCLIprogram(unmolkPath, args);

        }
    
}
