using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4U_MVP.Models
{
    internal class DataFileModel
    {
        // == Properties == 
        public string Label { get; set; }
        public string Path { get; set; }

        public SensorDataModel[,] _sensorDataGrid;

        // == Construtor == 
        public DataFileModel(int rows, int columns)
        {
            _sensorDataGrid = new SensorDataModel[rows, columns];
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    _sensorDataGrid[i, j] = new SensorDataModel();
                }
            }
        }

        // == Serialise to binary ==
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Label);
            writer.Write(_sensorDataGrid.GetLength(0)); // Number of rows
            writer.Write(_sensorDataGrid.GetLength(1)); // Number of columns
            for (int i = 0; i < _sensorDataGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _sensorDataGrid.GetLength(1); j++)
                {
                    if (_sensorDataGrid[i, j] != null)
                    {
                        writer.Write(true); // Indicate that the cell is not null
                        _sensorDataGrid[i, j].WriteTo(writer);
                    }
                    else
                    {
                        writer.Write(false); // Indicate that the cell is null
                    }
                }
            }
        }
        // == Deserialise from binary ==
        public static DataFileModel ReadFrom(BinaryReader reader)
        {
            string label = reader.ReadString();
            int rows = reader.ReadInt32();
            int columns = reader.ReadInt32();
            DataFileModel dataFile = new DataFileModel(rows, columns) { Label = label };
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    dataFile._sensorDataGrid[i, j] = SensorDataModel.ReadFrom(reader);
                }
            }
            return dataFile;
        }
    }
}
