using Microsoft.Win32;
using Sensing4U_MVP.Models;
using Sensing4U_MVP.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Sensing4U_MVP.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand SaveAllCommand => new RelayCommand(execute => test());
        public RelayCommand OpenFileCommand => new RelayCommand(execute => test2());
        public RelayCommand RefreshCommand => new RelayCommand(execute => RefreshDataGrid());

        public static int SelectedFileIndex { get; set; } = 0;
        private static DataFileModel SelectedFile { get; set; }
        private static SensorDataModel[,] SelectedSensorDataGrid { get; set; }
        private DataTable _SensorDataTable { get; set; }
        public DataTable SensorDataTable
        {
            get => _SensorDataTable;
            set
            {
                _SensorDataTable = value;
                OnPropertyChanged(nameof(SensorDataTable));
            }
        }

        private void RefreshDataGrid()
        {
            SelectedFile = DataProcessor.ProcessedDataFiles[SelectedFileIndex];
            SelectedSensorDataGrid = SelectedFile._sensorDataGrid;
            SensorDataTable = Sensor2DArrayToDataGridHelper.ToDataTable(SelectedSensorDataGrid);
        }
        public void test2()
        {
            var dialog = new OpenFileDialog();
            MessageBox.Show(DataProcessor.masterPath);
            dialog.InitialDirectory = DataProcessor.masterPath;
            dialog.Filter = "Data Files (*.dat)|*.dat";

            bool? success = dialog.ShowDialog();
            if (success == true)
            {
                DataProcessor.ProcessedDataFiles.Add(DataProcessor.LoadFromBinary(dialog.FileName));
                SelectedFile = DataProcessor.ProcessedDataFiles.Last();
                RefreshDataGrid();
            }
            else
            {
                MessageBox.Show("File load cancelled or failed.");
            }
        }


        public void test()
        {
            MessageBox.Show(SelectedFile.Path);
            DataProcessor.SaveToBinary(SelectedFile);
        }
        public MainWindowViewModel()
        {
            var datafile = new DataFileModel
            {
                Label = "Test Data File",
                Path = "D:\\TAFE\\2025_S2\\C# Two_ICTPRG443 - ICTICT430 - ICTICT523\\Assessments\\AT1\\TestBinary.dat",
                _sensorDataGrid = TestData.TestData.GetTestData()
            };
            DataProcessor.ProcessedDataFiles.Add(datafile);
            RefreshDataGrid();
        }

        private string? _fileLabel;
        public string? FileLabel
        {
            get => _fileLabel;
            set
            {
                _fileLabel = value;
                OnPropertyChanged();
            }
        }
    }
}

