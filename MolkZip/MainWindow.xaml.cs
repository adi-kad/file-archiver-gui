using Microsoft.WindowsAPICodePack.Dialogs;
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
using static System.Windows.Forms.LinkLabel;

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
                    FilesList.Items.Add(new FilesListItem(filepath, FilesList));
                }
            }
        }

        private void BrowseButtonClick(object sender, RoutedEventArgs e)
        {
            BrowseForFile();
        }

        private void BrowseForFile()
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = projectRootDir,
                Multiselect = true
            };
            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var fileNames = fileDialog.FileNames;
                if (fileNames.Count() != 0)
                {
                    AddFilesToList(fileNames.ToArray());
                    labelTip.Visibility = Visibility.Hidden;
                }

                if (FilesList.Items.Count > 0)
                {
                    Toolbar.Visibility = Visibility.Visible;
                }
            }
        }

        private void Files_Drop(object sender, DragEventArgs e)
        {
            labelTip.Visibility = Visibility.Hidden;
            Toolbar.Visibility = Visibility.Visible;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                AddFilesToList(files);
            }
        }

        private void HideRemoveButtons()
        {
            if (FilesList.Items.Count == 0)
            {
                labelTip.Visibility = Visibility.Visible;
                Toolbar.Visibility = Visibility.Hidden;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (MessageBox.Show("Do you want to close this window?",
                   "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                    else
                    {
                        // Do not close the window  
                    }
                    break;
                case Key.Delete:
                    RemoveFile();
                    break;
                case Key.Insert:
                    BrowseForFile();
                    break;
            }
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
                                MessageBoxButton.OKCancel,
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

        private void RemoveAllFiles()
        {
            if (!FilesList.Items.IsEmpty)
            {
                if (MessageBox.Show("Are you sure you want to clear all files?",
                        "Remove all",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    FilesList.Items.Clear();
                    HideRemoveButtons();
                }
            }
        }

        private void RemoveFile()
        {
            int deletionIndex = FilesList.SelectedIndex;
            FilesList.Items.Remove(FilesList.Items[deletionIndex]);
            if (!FilesList.Items.IsEmpty)
            {
                FilesList.SelectedIndex = Math.Min(deletionIndex, FilesList.Items.Count - 1);
            }
        }

        private void RemoveFileButton(object sender, RoutedEventArgs e)
        {
            RemoveAllFiles();
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

        private void Unmolk(object sender, RoutedEventArgs e)
        {
            string unmolkPath = projectRootDir + "unmolk.exe";
            CommonOpenFileDialog folderPickerDialog = new CommonOpenFileDialog
            {
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
            else if (folderPickerDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                int successfulUnmolks = 0;
                int nonMolkFiles = 0;

                foreach (FilesListItem item in FilesList.Items)
                {
                    //Primitive check to see if file is a molk archive
                    if (System.IO.Path.GetExtension(item.Filename) != ".molk")
                    {
                        ++nonMolkFiles;
                        continue;
                    }
                    List<string> args = new List<string>();
                    args.Add(item.FullPath);
                    args.Add("-d");
                    args.Add(folderPickerDialog.FileName);
                    RunCLIprogram(unmolkPath, args);
                    ++successfulUnmolks;
                }
                if (nonMolkFiles > 0)
                {
                    MessageBox.Show("Some files in the list are not molk archives\n" +
                                "and will not be unmolked.",
                                $"Skipping {nonMolkFiles} files",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                }
                if (successfulUnmolks > 0)
                {
                    MessageBox.Show($"Unmolked {successfulUnmolks} molk archives.",
                                "Unmolking done",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                }
            }
        }
    }
}
