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
}
