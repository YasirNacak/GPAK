using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;

// TODO (yasir): fix conflicts between winforms and win32 dialogs
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using SelectionMode = System.Windows.Controls.SelectionMode;

namespace GPAK.PackageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _title = "Gonzo Package Editor";

        public static RoutedCommand OpenFileCommand = new RoutedCommand();

        public static RoutedCommand NewFileCommand = new RoutedCommand();

        private bool _isPackageOpen;

        public MainWindow()
        {
            InitializeComponent();

            PackageContentsListBox.SelectionMode = SelectionMode.Extended;
            
            OpenFileCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));

            NewFileCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));

            PackageNameLabel.Content = "No Package Loaded";

            _isPackageOpen = false;

            Title = _title;
        }
        
        private void OnNewMenuItemClicked(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { DefaultExt = "GPAK", Filter = "Gonzo Package Files (*.GPAK)|*.GPAK", Title = "Save New Package As"};

            if (saveFileDialog.ShowDialog() == false) return;
            
            LoadPackage(saveFileDialog.FileName);
        }

        private void OnOpenMenuItemClicked(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = "Gonzo Package Files (*.GPAK)|*.GPAK", Title = "Select a Package To Open" };
            
            if (openFileDialog.ShowDialog() == false) return;
            
            LoadPackage(openFileDialog.FileName);
        }

        private void OnExitMenuItemClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnAddEntryCompressedMenuItemClicked(object sender, RoutedEventArgs e)
        {
            AddEntry(true);
        }

        private void OnAddEntryNoCompressionMenuItemClicked(object sender, RoutedEventArgs e)
        {
            AddEntry(false);
        }

        private void OnOpenFileCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OnOpenMenuItemClicked(sender, e);
        }

        private void OnNewFileCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OnNewMenuItemClicked(sender, e);
        }

        private void AddEntry(bool shouldCompress)
        {
            if (!_isPackageOpen)
            {
                MessageBox.Show("No Package Has Loaded!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var openFileDialog = new OpenFileDialog {Multiselect = true, Title = "Select Files To Add"};

            if (openFileDialog.ShowDialog() == false) return;
            
            foreach (var fileName in openFileDialog.FileNames)
            {
                PackageData.Writer.AddEntry(fileName, shouldCompress);
            }

            LoadPackage(PackageData.Reader.PackagePath);
        }

        private void LoadPackage(string packagePath)
        {
            PackageContentsListBox.Items.Clear();

            PackageData.LoadPackage(packagePath);

            foreach (var entryName in PackageData.Reader.GetAllEntryNames())
            {
                var listBoxItem = new ListBoxItem {Content = entryName, ContextMenu = GetListboxContextMenu()};
                PackageContentsListBox.Items.Add(listBoxItem);
            }

            PackageNameLabel.Content = "Package Name: " + PackageData.Reader.PackageName;
            PackageEntryCountLabel.Content = "Number of Entries: " + PackageData.Reader.EntryCount;
            PackageSizeLabel.Content = "Size: " + Util.SizeSuffix(PackageData.Reader.PackageFileSize);

            _isPackageOpen = true;
        }

        private ContextMenu GetListboxContextMenu()
        {
            var cMenu = new ContextMenu();

            var extractMenuItem = new MenuItem {Header = "Extract"};
            extractMenuItem.Click += (sender, args) =>
            {
                var folderBrowserDialog = new FolderBrowserDialog();
                
                if(folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

                foreach (ListBoxItem entry in PackageContentsListBox.SelectedItems)
                {
                    PackageData.Reader.ExtractOne(entry.Content.ToString(), folderBrowserDialog.SelectedPath);
                }
            };
            
            cMenu.Items.Add(extractMenuItem);

            // TODO (yasir): enable this when the GPAK library gets delete functionality
            /*
            var deleteMenuItem = new MenuItem {Header = "Delete"};
            deleteMenuItem.Click += (sender, args) =>
            {
                foreach (ListBoxItem entry in PackageContentsListBox.SelectedItems)
                {
                    PackageData.Writer.RemoveEntry(entry.Content.ToString());
                }
            };
            
            cMenu.Items.Add(deleteMenuItem);
            */

            return cMenu;
        }
    }
}
