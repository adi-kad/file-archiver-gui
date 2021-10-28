using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MolkZip
{
    class FilesListItem : StackPanel
    {
        private ListBox parentList;
        public string Filename { get; private set; }
        public string FullPath { get; private set; }

        public FilesListItem(string path, ListBox parent)
        {
            FullPath = path;
            Filename = Path.GetFileName(FullPath);
            Orientation = Orientation.Horizontal;

            Button deletionButton = new Button();
            deletionButton.Click += new RoutedEventHandler(DeleteButtonPress);
            deletionButton.Content = "❌";
            deletionButton.BorderThickness = new Thickness(0.0);
            deletionButton.Foreground = new SolidColorBrush(Color.FromRgb(220, 0, 0));
            deletionButton.HorizontalAlignment = HorizontalAlignment.Right;
            Children.Add(deletionButton);

            Children.Add(new TextBlock { 
                Text = Filename,
                Margin = new Thickness(5.0)
            });

            parentList = parent;
        }

        private void DeleteButtonPress(object sender, RoutedEventArgs e)
        {
            parentList.Items.Remove(this);           
        }
    }
}
