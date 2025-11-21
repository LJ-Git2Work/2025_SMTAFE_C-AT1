using Microsoft.Win32;
using Sensing4U_MVP.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Sensing4U_MVP.Services
{
    public class DataProcessor
    {
        /// <summary>
        /// Prevents the instantiation of the <see cref="DataProcessor"/> class from outside the class.
        /// </summary>
        /// <remarks>This private constructor is used to enforce the singleton pattern or to restrict
        /// instantiation of the class. Instances of <see cref="DataProcessor"/> can only be created internally within
        /// the class.</remarks>
        private DataProcessor() { }
        
        /// <summary>
        /// Provides a lazily initialized, thread-safe singleton instance of the <see cref="DataProcessor"/> class.
        /// </summary>
        /// <remarks>The singleton instance is created only when it is first accessed, ensuring efficient
        /// resource usage. This implementation uses <see cref="Lazy{T}"/> to guarantee thread safety during
        /// initialization.</remarks>
        private static readonly Lazy<DataProcessor> _instance = new Lazy<DataProcessor>(() => new DataProcessor());
        
        /// <summary>
        /// Gets the singleton instance of the <see cref="DataProcessor"/> class.
        /// </summary>
        public static DataProcessor Instance => _instance.Value;

        /// <summary>
        /// Gets or sets the collection of files that have been processed.
        /// </summary>
        public ObservableCollection<DataSetFile> ProcessedFiles { get; set; } = new ObservableCollection<DataSetFile>();
        
        /// <summary>
        /// The directory path where the application is currently running.
        /// </summary>
        /// <remarks>This field is initialized to the base directory of the current application domain. It
        /// can be used to locate files or resources relative to the application's execution path.</remarks>
        public string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        /// <summary>
        /// Represents the directory path where data files are stored.
        /// </summary>
        /// <remarks>This field is intended for internal use to manage the location of data files. Ensure
        /// that the value assigned to this field is a valid directory path.</remarks>
        private string _dataDirectory;

        /// <summary>
        /// Gets the path to the application's data directory.
        /// </summary>
        public string DataDirectory
        {
            get
            {
                if (_dataDirectory == null)
                {
                    _dataDirectory = Path.Combine(appDirectory, "Data");
                    Directory.CreateDirectory(_dataDirectory);
                }
                return _dataDirectory;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="icon"></param>
        private void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private string GetSafeFileName(string label)
        {
            string fileName = string.IsNullOrEmpty(label) ? "Untitled" : label;
            fileName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return fileName + ".dat";
        }
        /// <summary>
        /// 
        /// </summary>
        public void SaveAllToBinary()
        {
            if (ProcessedFiles == null || ProcessedFiles.Count == 0)
            {
                ShowMessage("No files to save.", "Information", MessageBoxImage.Information);
                return;
            }

            int savedCount = 0;
            foreach (var dataFile in ProcessedFiles)
            {
                try
                {
                    SaveToBinary(dataFile);
                    savedCount++;
                }
                catch (Exception ex)
                {
                    ShowMessage($"Error saving file '{dataFile.Label}': {ex.Message}", "Error", MessageBoxImage.Error);
                }
            }

            ShowMessage($"Successfully saved {savedCount} file(s) to binary format.", "Success", MessageBoxImage.Information);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFile"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SaveToBinary(DataSetFile dataFile)
        {
            if (dataFile == null)
                throw new ArgumentNullException(nameof(dataFile));

            string filePath = null;

            
            if (!string.IsNullOrEmpty(dataFile.Path))
            {
                try
                {
                    string directory = Path.GetDirectoryName(dataFile.Path);
                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        filePath = dataFile.Path;
                    }
                }
                catch
                {
                    // Path is malformed, will try dialog next
                }
            }

            
            if (filePath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Binary Data Files (*.dat)|*.dat|All Files (*.*)|*.*",
                    Title = "Save Data File",
                    InitialDirectory = DataDirectory,
                    FileName = GetSafeFileName(dataFile.Label)
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    filePath = saveFileDialog.FileName;
                }
            }

            
            if (filePath == null)
            {
                string fileName = GetSafeFileName(dataFile.Label);
                filePath = GetUniqueFilePath(Path.Combine(DataDirectory, fileName));
            }

            
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                if (dataFile.Grid == null)
                {
                    writer.Write(0); 
                    writer.Write(0); 
                }
                else
                {
                    int rows = dataFile.Grid.GetLength(0);
                    int cols = dataFile.Grid.GetLength(1);

                    writer.Write(rows);
                    writer.Write(cols);

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            var record = dataFile.Grid[i, j];
                            if (record != null)
                            {
                                writer.Write(true);
                                writer.Write(record.Value);
                                writer.Write(record.Label ?? string.Empty);
                                writer.Write(record.Timestamp.ToBinary());
                            }
                            else
                            {
                                writer.Write(false);
                            }
                        }
                    }
                }
            }

            dataFile.Path = filePath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFilePath;

            do
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExt}_{counter}{extension}");
                counter++;
            }
            while (File.Exists(newFilePath));

            return newFilePath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public DataSetFile LoadFromBinary(string dataFilePath)
        {
            if (string.IsNullOrEmpty(dataFilePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(dataFilePath));

            if (!File.Exists(dataFilePath))
                throw new FileNotFoundException("File does not exist.", dataFilePath);

            var dataFile = new DataSetFile
            {
                Label = Path.GetFileNameWithoutExtension(dataFilePath),
                Path = dataFilePath
            };

            using (var reader = new BinaryReader(File.Open(dataFilePath, FileMode.Open)))
            {
                int rows = reader.ReadInt32();
                int cols = reader.ReadInt32();

                if (rows == 0 || cols == 0)
                {
                    dataFile.Grid = new SensorData[0, 0];
                    return dataFile;
                }

                var grid = new SensorData[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (!reader.ReadBoolean())
                        {
                            grid[i, j] = new SensorData();
                            continue;
                        }

                        grid[i, j] = new SensorData
                        {
                            Value = reader.ReadSingle(),
                            Label = reader.ReadString(),
                            Timestamp = DateTime.FromBinary(reader.ReadInt64())
                        };
                    }
                }

                dataFile.Grid = grid;
            }

            return dataFile;
        }
        /// <summary>
        /// 
        /// </summary>
        public void LoadFromBinary()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Binary Data Files (*.dat)|*.dat|All Files (*.*)|*.*",
                Title = "Load Binary Data File(s)",
                InitialDirectory = DataDirectory,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                int loadedCount = 0;
                foreach (string filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        DataSetFile dataFile = LoadFromBinary(filePath);

                        if (dataFile != null)
                        {
                            ProcessedFiles.Add(dataFile);
                            loadedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error loading file '{Path.GetFileName(filePath)}': {ex.Message}", "Error", MessageBoxImage.Error);
                    }
                }

                if (loadedCount > 0)
                {
                    ShowMessage($"Successfully loaded {loadedCount} file(s).", "Success", MessageBoxImage.Information);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SensorData"></param>
        /// <returns></returns>
        public float CalculateAverage(SensorData[,] SensorData)
        {
            if (SensorData == null || SensorData.Length == 0)
                return 0f;

            float sum = 0f;
            int count = 0;

            for (int i = 0; i < SensorData.GetLength(0); i++)
            {
                for (int j = 0; j < SensorData.GetLength(1); j++)
                {
                    if (SensorData[i, j] != null)
                    {
                        sum += SensorData[i, j].Value;
                        count++;
                    }
                }
            }
            if (count == 0)
                return 0;
            else
                return sum / count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flat"></param>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public SensorData[,] SquareGrid(List<SensorData> flat, int rows, int cols)
        {
            if (flat.Count != rows * cols)
                throw new ArgumentException("List count does not match grid dimensions");

            SensorData[,] grid = new SensorData[rows, cols];

            int index = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    grid[i, j] = flat[index++];

            return grid;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public int BinarySearchNearest(List<SensorData> list, SensorData target, Comparison<SensorData> cmp)
        {
            int left = 0;
            int right = list.Count - 1;

            if (list.Count == 0)
                return -1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int result = cmp(list[mid], target);

                if (result == 0)
                    return mid;

                if (result < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            if (right < 0)
                return left;
            if (left >= list.Count)
                return right;

            int diffRight = Math.Abs(cmp(list[right], target));
            int diffLeft = Math.Abs(cmp(list[left], target));

            return (diffLeft < diffRight) ? left : right;
        }

    }
}
