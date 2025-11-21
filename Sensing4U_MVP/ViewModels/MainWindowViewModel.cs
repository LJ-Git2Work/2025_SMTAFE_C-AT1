using Sensing4U_MVP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sensing4U_MVP.ViewModels 
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            SaveAllCommand = new RelayCommand(_ => DataProcessor.SaveToCSV());
        }
        public ICommand SaveAllCommand { get; }

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
