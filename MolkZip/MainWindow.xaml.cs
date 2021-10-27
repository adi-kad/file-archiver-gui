
﻿using Microsoft.WindowsAPICodePack.Dialogs;
﻿using Microsoft.Win32;
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
        //Assumes Visual Studio project folder structure
        static readonly string projectRootDir =
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddFilesToList(string[] filepaths)
        {
            foreach (string filepath in filepaths)
            {
                bool isDuplicate = false;
                foreach (FilesListItem item in FilesList.Items)
                {
                    //Has this filename already been added to the list?
                    string filename = item.Filename;
                    if (filename.Equals(System.IO.Path.GetFileName(filepath)))
                    {
                        isDuplicate = true;
                        MessageBox.Show($"Couldn't add file {filename}." +
                            "\nReason: Filename is already in the list.",
                            "Couldn't add file",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                        break;
                    }
                }
                if (!isDuplicate)
                {
                    //TODO: Do some testing to see if these commented-out lines are actually needed.
                    //ListBoxItem file = new ListBoxItem();
                    //file.Content = filepath;
                    //FilesList.Items.Add(filename);
                    FilesList.Items.Add(new FilesListItem(filepath, FilesList));
                }
            }
        }

        private void Files_Drop(object sender, DragEventArgs e)
        {
            labelTip.Visibility = Visibility.Hidden;
            remove.Visibility = Visibility.Visible;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFilesToList(files);
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

            //MessageBox.Show(commandLineArgs.ToString());
            System.Diagnostics.Process.Start(programPath, commandLineArgs.ToString());
        }

        private void Molk(object sender, RoutedEventArgs e)
        {
            string molkPath = projectRootDir + "molk.exe";
            CommonOpenFileDialog folderPickerDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = projectRootDir,
            };
            if (FilesList.Items.IsEmpty)
            {
                MessageBox.Show("Couldn't molk." +
                                "\nReason: File list is empty.",
                                "Couldn't molk",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else if (folderPickerDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string destFilePath = folderPickerDialog.FileName + "\\archive.molk"; //TODO: let user choose filename
                List<string> args = new List<string>();
                //-j flag makes it so that the archive contains *just* the file,
                //instead of mimicking the entire folder structure of the file's full path.
                args.Add("-j");
                args.Add(destFilePath);
                foreach (FilesListItem item in FilesList.Items)
                {
                    args.Add(item.FullPath);
                }
                RunCLIprogram(molkPath, args);
                MessageBox.Show("Molked files into archive.molk",
                                "Molking done",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void Unmolk(object sender, RoutedEventArgs e)
        {
            string unmolkPath = projectRootDir + "unmolk.exe";
            CommonOpenFileDialog folderPickerDialog = new CommonOpenFileDialog {
                IsFolderPicker = true,
                InitialDirectory = projectRootDir,
            };
            
            if (FilesList.Items.IsEmpty)
            {
                MessageBox.Show("Couldn't unmolk." +
                                "\nReason: File list is empty.",
                                "Couldn't unmolk",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
            else if(folderPickerDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                int successfulUnmolks = 0;
                foreach(FilesListItem item in FilesList.Items)
                {
                    //Primitive check to see if file is a molk archive
                    if(System.IO.Path.GetExtension(item.Filename) != ".molk")
                    {
                        MessageBox.Show($"Couldn't unmolk file {System.IO.Path.GetFileName(item.FullPath)}" +
                                "\nReason: File is not a molk archive.",
                                "Couldn't unmolk file",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                        continue;
                    }
                    List<string> args = new List<string>();
                    args.Add(item.FullPath);
                    args.Add("-d");
                    args.Add(folderPickerDialog.FileName);
                    RunCLIprogram(unmolkPath, args);
                    ++successfulUnmolks;
                }
                if(successfulUnmolks > 0)
                {
                    MessageBox.Show($"Unmolked {successfulUnmolks} molk archives.",
                                "Unmolking done",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                }
            }
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            BrowseForFile();
        }

        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            FilesList.Items.Remove(FilesList.SelectedItem);
            HideRemoveButton();
        }

        private void FilesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {

                FilesList.Items.Remove(FilesList.SelectedItem);
                HideRemoveButton();
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("Do you want to close this window?",
                    "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    // Do not close the window  
                }
            }
            else if (e.Key == Key.Insert)
            {
                BrowseForFile();
            }
        }

        private void HideRemoveButton()
        {
            if (FilesList.Items.Count == 0)
            {
                remove.Visibility = Visibility.Hidden;
                labelTip.Visibility = Visibility.Visible;
            }
        }
        private void BrowseForFile()
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            string fileName = fileDialog.FileName;
            if (fileName != "")
            {
                AddFilesToList(new string[] { fileName });
                labelTip.Visibility = Visibility.Hidden;
            }

            if (FilesList.Items.Count > 0)
            {
                remove.Visibility = Visibility.Visible;
            }
        }

        private void AddFile_MouseMove(object sender, MouseEventArgs e)
        {
            addFile.Background = Brushes.Magenta;
        }

        private void AddFile_MouseEnter(object sender, MouseEventArgs e)
        {
            addFile.FontSize = 18;
            addFile.FontWeight = FontWeights.Bold;
        }

        private void AddFile_MouseLeave(object sender, MouseEventArgs e)
        {
            addFile.FontSize = 16;
            addFile.FontWeight = FontWeights.Normal;
        }

        private void Remove_MouseEnter(object sender, MouseEventArgs e)
        {
            remove.FontSize = 18;
            remove.FontWeight = FontWeights.Bold;
        }

        private void Remove_MouseLeave(object sender, MouseEventArgs e)
        {
            remove.FontSize = 16;
            remove.FontWeight = FontWeights.Bold;

        }
    }
}
