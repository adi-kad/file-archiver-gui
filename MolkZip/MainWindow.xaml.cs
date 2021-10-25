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

        private void Files_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filename in files)
                {
                    bool isDuplicate = false;
                    foreach (object obj in FilesList.Items)
                    {
                        //Has this filename already been added to the list?
                        if (((string)obj).Equals(filename))
                        {
                            isDuplicate = true;
                            MessageBox.Show("Couldn't add file " + filename +
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
                        //file.Content = filename;
                        FilesList.Items.Add(filename);
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
            //MessageBox.Show(commandLineArgs.ToString());
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
            foreach (Object obj in FilesList.Items)
            {
                args.Add((string)obj);
            }
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
}
