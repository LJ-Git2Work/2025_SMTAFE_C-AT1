using Sensing4U_MVP.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sensing4U_MVP.Models
{
    public class DataSetFile
    {
        /// <summary>
        /// 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private SensorData[,] _grid;
        public SensorData[,] Grid
        {
            get => _grid;
            set => _grid = value;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataSetFile()
        {
            _grid = new SensorData[0, 0];
        }

        /// <summary>
        /// Constructor with sensor records list
        /// </summary>
        public DataSetFile(int rows, int cols)
        {
            _grid = new SensorData[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    _grid[i, j] = new SensorData();
                }
            }
        }
    }
}
