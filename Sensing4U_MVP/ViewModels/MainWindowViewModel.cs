using Sensing4U_MVP.Models;
using Sensing4U_MVP.Services;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Media;


namespace Sensing4U_MVP.ViewModels
{
    /// <summary>
    /// Sort mode enumeration
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        #region Menu Commands
        public RelayCommand SaveAllCommand => new RelayCommand(execute => DataProcessor.Instance.SaveAllToBinary());
        public RelayCommand SaveFileCommand => new RelayCommand(execute => DataProcessor.Instance.SaveToBinary(SelectedFile));
        public RelayCommand OpenFileCommand => new RelayCommand(execute => DataProcessor.Instance.LoadFromBinary());
        #endregion

        /// <summary>
        /// 
        /// </summary>
        #region File Navigation Commands
        private int _selectedFileIndex;
        public int SelectedFileIndex
        {
            get => _selectedFileIndex;
            set
            {
                _selectedFileIndex = value;
                OnPropertyChanged(nameof(SelectedFileIndex));
            }
        }

        public RelayCommand NextCommand => new RelayCommand(execute =>
        {
            if (SelectedFileIndex < ProcessedDataFiles.Count - 1)
                SelectedFileIndex++;
        });

        public RelayCommand PreviousCommand => new RelayCommand(execute =>
        {
            if (SelectedFileIndex > 0)
                SelectedFileIndex--;
        });
        #endregion

        #region File Navigation Bindings
        public ObservableCollection<DataSetFile> ProcessedDataFiles
        {
            get => DataProcessor.Instance.ProcessedFiles;
            set
            {
                DataProcessor.Instance.ProcessedFiles = value;
                OnPropertyChanged(nameof(ProcessedDataFiles));
            }
        }

        private DataTable _selectedTable;
        public DataTable SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
            }
        }

        private DataSetFile _selectedFile;
        public DataSetFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
                UpdateDataGrid();
            }
        }

        public SensorData[,] SelectedGrid
        {
            get => SelectedFile.Grid;
            set
            {
                SelectedFile.Grid = value;
                OnPropertyChanged(nameof(SelectedGrid));
                UpdateDataGrid();
            }
        }

        #endregion

        #region Control Panel Bindings
        // == Control Panel Properties ==
        public float? LowerBoundValue => _lowerBound == null ? null : ParseFloatOrNull(_lowerBound);
        private string _lowerBound;
        public string LowerBound
        {
            get => _lowerBound;
            set
            {
                _lowerBound = value;
                OnPropertyChanged(nameof(LowerBound));
                OnPropertyChanged(nameof(LowerBoundValue));
            }
        }

        public float? UpperBoundValue => _upperBound == null ? null : ParseFloatOrNull(_upperBound);
        private string? _upperBound = null;
        public string? UpperBound
        {
            get => _upperBound;
            set
            {
                _upperBound = value;
                OnPropertyChanged(nameof(UpperBound));
                OnPropertyChanged(nameof(UpperBoundValue));
            }
        }

        private static float? ParseFloatOrNull(string value)
        {
            return string.IsNullOrEmpty(value) || !float.TryParse(value, out var result)
                ? null
                : result;
        }

        private float? _dataSetAverage;
        public float? DataSetAverage
        {
            get => _dataSetAverage;
            set
            {
                _dataSetAverage = value;
                OnPropertyChanged(nameof(DataSetAverage));
            }
        }

        /// <summary>
        /// Current sort mode based on radio button selection
        /// </summary>
        public string SearchMode => $"Searching for: {CurrentSortMode}";
        private SortMode _currentSortMode = SortMode.None;
        public SortMode CurrentSortMode
        {
            get => _currentSortMode;
            set
            {
                _currentSortMode = value;
                OnPropertyChanged(nameof(SearchMode));
            }
        }
        public bool IsDataSorted => CurrentSortMode == SelectedSortMode;

        private SortMode _selectedSortMode = SortMode.Value;
        public SortMode SelectedSortMode
        {
            get => _selectedSortMode;
            set
            {
                if (_selectedSortMode != value)
                {
                    _selectedSortMode = value;
                    OnPropertyChanged(nameof(SelectedSortMode));
                }
            }
        }


        #endregion

        #region Search Properties 
        // == Search Properties ==

        private string _searchTextBox;
        public string SearchTextBox
        {
            get => _searchTextBox;
            set
            {
                _searchTextBox = value;
                OnPropertyChanged(nameof(SearchTextBox));
            }
        }

        private string _searchResult = "No search performed";
        public string SearchResult
        {
            get => _searchResult;
            set
            {
                _searchResult = value;
                OnPropertyChanged(nameof(SearchResult));
            }
        }
        private string? _searchFound = null;
        public string? SearchFound
        {
            get => _searchFound;
            set
            {
                _searchFound = value;
                OnPropertyChanged(nameof(SearchFound));
            }
        }
        private int _searchedForRowIndex = 1;
        public int SearchedForRowIndex
        {
            get => _searchedForRowIndex;
            set
            {
                _searchedForRowIndex = value;
                OnPropertyChanged(nameof(SearchedForRowIndex));
            }
        }
        public int _searchedForColumnIndex = -1;
        public int SearchedForColumnIndex 
        {
            get => _searchedForColumnIndex;
            set
            {
                _searchedForColumnIndex = value;
                OnPropertyChanged(nameof(SearchedForColumnIndex));
            }
        }
        #endregion

        #region Sort and Search Commands 
        // == Sort Command ==

        public RelayCommand ApplySortCommand => new RelayCommand(execute => ApplySort());

        private void ApplySort()
        {
            if (SelectedFile == null || SelectedFile.Grid == null)
                return;

            try
            {
                int rows = SelectedFile.Grid.GetLength(0);
                int cols = SelectedFile.Grid.GetLength(1);

                var flat = SelectedFile.Grid.Cast<SensorData>();

                IEnumerable<SensorData> sorted = SelectedSortMode switch
                {
                    SortMode.Label => flat.OrderBy(x => x.Label),
                    SortMode.Value => flat.OrderBy(x => x.Value),
                    SortMode.Timestamp => flat.OrderBy(x => x.Timestamp),
                    _ => flat
                };

                SelectedGrid = DataProcessor.Instance.SquareGrid(sorted.ToList(), rows, cols);
                CurrentSortMode = SelectedSortMode;
                SearchResult = $"Sorted by {SelectedSortMode}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sorting data: {ex.Message}", "Sort Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // == Search Command ==

        public RelayCommand SearchCommand => new RelayCommand(
            execute => ExecuteSearch(),
            canExecute => IsDataSorted && !string.IsNullOrWhiteSpace(SearchTextBox)
        );

        private void ExecuteSearch()
        {
            if (!IsDataSorted) { SearchResult = "Please sort data first"; return; }
            if (SelectedFile?.Grid == null || SelectedFile.Grid.Length == 0) { SearchResult = "No data to search"; return; }

            var flat = SelectedFile.Grid.Cast<SensorData>().ToList();
            int resultIndex = -1;

            try
            {
                switch (SelectedSortMode)
                {
                    case SortMode.Label:
                        resultIndex = DataProcessor.Instance.BinarySearchNearest(flat, new SensorData { Label = SearchTextBox },
                            (a, b) => string.Compare(a.Label, b.Label, StringComparison.OrdinalIgnoreCase));
                        break;

                    case SortMode.Value:
                        if (!float.TryParse(SearchTextBox, out float f)) { SearchResult = "Invalid numeric value"; return; }
                        resultIndex = DataProcessor.Instance.BinarySearchNearest(flat, new SensorData { Value = f },
                            (a, b) => a.Value.CompareTo(b.Value));
                        break;

                    case SortMode.Timestamp:
                        if (!DateTime.TryParse(SearchTextBox, out DateTime t)) { SearchResult = "Invalid date/time format"; return; }
                        resultIndex = DataProcessor.Instance.BinarySearchNearest(flat, new SensorData { Timestamp = t },
                            (a, b) => a.Timestamp.CompareTo(b.Timestamp));
                        break;
                }

                if (resultIndex != -1)
                {
                    SearchFound = flat[resultIndex].ToString();
                    SearchResult = $"Found at index {resultIndex}";
                    SearchedForColumnIndex = resultIndex % SelectedFile.Grid.GetLength(1);
                    SearchedForRowIndex = resultIndex / SelectedFile.Grid.GetLength(1);
                }
                else
                {
                    SearchedForColumnIndex = -1;
                    SearchedForRowIndex = -1;
                    SearchResult = "Not found";
                }
            }
            catch (Exception ex)
            {
                SearchFound = ex.Message;
                SearchResult = $"Search error: {ex.Message}";
            }
        }


        #endregion

        #region DataGrid Bindings
        // == DataGrid Properties ==

        private DataTable _SensorDataTable;
        public DataTable SensorDataTable
        {
            get => _SensorDataTable;
            set
            {
                _SensorDataTable = value;
                OnPropertyChanged(nameof(SensorDataTable));
            }
        }
        #endregion

        #region Update Methods 
        // == Update Data Grid based on selected file ==

        private void UpdateDataGrid()
        {
            if (SelectedFile == null)
                return;

            try
            {
                SearchFound = null;
                SearchedForColumnIndex = -1;
                SearchedForRowIndex = -1;

                // Calculate average from records
                if (SelectedFile.Grid != null && SelectedFile.Grid.Length > 0)
                {
                    DataSetAverage = DataProcessor.Instance.CalculateAverage(SelectedFile.Grid);
                    SensorDataTable = Sensor2DArrayHelper.ToDataTable(SelectedFile.Grid);

                    // Data has been reloaded, it's not sorted
                    CurrentSortMode = SortMode.None;
                }
                else
                {
                    DataSetAverage = 0f;
                    SensorDataTable = new DataTable();
                }

                // Clear search results
                SearchResult = "No search performed";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating grid: {ex.Message}", "Update Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Constructor
        // == Test Constructor ==
        public MainWindowViewModel()
        {
            try
            {
                // Generate test files
                var files = TestData.TestData.GetTestFiles(5);

                foreach (var file in files)
                {
                    DataProcessor.Instance.ProcessedFiles.Add(file);
                }

                if (DataProcessor.Instance.ProcessedFiles.Count > 0)
                {
                    SelectedFile = DataProcessor.Instance.ProcessedFiles[0];
                    UpdateDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing view model: {ex.Message}", "Initialization Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
