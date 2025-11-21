using Sensing4U_MVP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4U_MVP.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class Sensor2DArrayHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static SensorData[,] ToSensorDataArray(DataTable table)
        {
            if (table == null || table.Rows.Count == 0 || table.Columns.Count == 0)
                return new SensorData[0, 0];

            int rows = table.Rows.Count;
            int cols = table.Columns.Count;
            var array = new SensorData[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    array[r, c] = table.Rows[r][c] as SensorData;
                }
            }

            return array;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(SensorData[,] array)
        {
            var table = new DataTable();

            for (int c = 0; c < array.GetLength(1); c++)
            {
                table.Columns.Add($"Col {c + 1}", typeof(SensorData));
            }

            for (int r = 0; r < array.GetLength(0); r++)
            {
                var row = table.NewRow();
                bool rowHasData = false;

                for (int c = 0; c < array.GetLength(1); c++)
                {
                    if (array[r, c] != null)
                    {
                        row[c] = array[r, c];
                        rowHasData = true;
                    }
                }

                if (rowHasData)
                    table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Convert list of SensorRecords to DataTable for visual display
        /// </summary>
        public static DataTable ListToDataTable(List<SensorData> records, int rows, int cols)
        {
            var table = new DataTable();

            if (records == null || rows <= 0 || cols <= 0)
                return table;

            // Create columns
            for (int c = 0; c < cols; c++)
            {
                table.Columns.Add($"Col {c + 1}", typeof(SensorData));
            }

            // Fill rows
            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                var row = table.NewRow();
                bool rowHasData = false;

                for (int c = 0; c < cols; c++)
                {
                    if (index < records.Count && records[index] != null)
                    {
                        var record = records[index];
                        row[c] = new SensorData
                        {
                            Label = record.Label,
                            Timestamp = record.Timestamp,
                            Value = record.Value
                        };
                        rowHasData = true;
                        index++;
                    }
                    else
                    {
                        row[c] = new SensorData();
                        index++;
                    }
                }

                if (rowHasData || index <= records.Count)
                    table.Rows.Add(row);
            }

            return table;
        }

        /// <summary>
        /// Calculate optimal grid dimensions for displaying records
        /// </summary>
        public static (int rows, int cols) CalculateOptimalDimensions(int recordCount)
        {
            if (recordCount <= 0)
                return (1, 1);

            // Calculate nearest square
            int sqrtCount = (int)Math.Ceiling(Math.Sqrt(recordCount));

            // Adjust for better aspect ratio if needed
            int rows = sqrtCount;
            int cols = (int)Math.Ceiling((double)recordCount / rows);

            return (rows, cols);
        }

        /// <summary>
        /// Arrange sensor records into a 2D grid structure
        /// </summary>
        public static SensorData[,] ArrangeRecordsInGrid(List<SensorData> records, int rows, int cols)
        {
            if (records == null || rows <= 0 || cols <= 0)
                return new SensorData[0, 0];

            var array = new SensorData[rows, cols];
            int index = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (index < records.Count && records[index] != null)
                    {
                        var record = records[index];
                        array[i, j] = new SensorData
                        {
                            Label = record.Label,
                            Timestamp = record.Timestamp,
                            Value = record.Value
                        };
                    }
                    else
                    {
                        array[i, j] = new SensorData();
                    }
                    index++;
                }
            }

            return array;
        }
    }

}
