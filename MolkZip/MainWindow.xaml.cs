using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Files_Drop(object sender, DragEventArgs e)
        {
            labelTip.Visibility = Visibility.Hidden;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          
            OpenFileDialog newDiolog = new OpenFileDialog();
            newDiolog.ShowDialog();
            string fileName = newDiolog.FileName;
            FilesList.Items.Add(fileName);
            labelTip.Visibility = Visibility.Hidden;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            for (int i = FilesList.SelectedItems.Count-1 ; i >= 0; i--)
            {
               
                FilesList.Items.Remove(FilesList.SelectedItems[i]);
               
            }
                     
        }
    }
}
