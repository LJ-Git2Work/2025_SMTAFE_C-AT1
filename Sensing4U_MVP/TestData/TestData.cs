using Sensing4U_MVP.Models;
using Sensing4U_MVP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sensing4U_MVP.TestData
{
    /// <summary>
    /// 
    /// </summary>
    public static class TestData
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Random random = new Random();
        /// <summary>
        /// 
        /// </summary>
        private static int fileCounter = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float GetRandomFloat(float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="valueMin"></param>
        /// <param name="valueMax"></param>
        /// <returns></returns>
        public static SensorData[,] GetTestData(int rows = -1, int cols = -1, float valueMin = 0f, float valueMax = 100f)
        {
            // Use random dimensions if not specified
            if (rows == -1) rows = GetRandomInt(1, 20);
            if (cols == -1) cols = GetRandomInt(1, 20);

            SensorData[,] testdata = new SensorData[rows, cols];

            // Random base timestamp (within last 30 days)
            DateTime baseTime = DateTime.Now.AddDays(-GetRandomInt(0, 30));

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Add some variance to the value range per row
                    float rowVariance = GetRandomFloat(0.8f, 1.2f);
                    float actualMin = valueMin * rowVariance;
                    float actualMax = valueMax * rowVariance;

                    testdata[i, j] = new SensorData
                    {
                        Value = GetRandomFloat(actualMin, actualMax),
                        Label = GenerateRandomLabel(i, j),
                        Timestamp = baseTime.AddMinutes(i * 60 + j * 5) // Incremental timestamps
                    };
                }
            }

            return testdata;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private static string GenerateRandomLabel(int row, int col)
        {
            string[] prefixes = { "Temp", "Press", "Humid", "Flow", "Level", "Speed", "Power", "Voltage" };
            string[] zones = { "North", "South", "East", "West", "Central" };

            string prefix = prefixes[random.Next(prefixes.Length)];
            string zone = zones[random.Next(zones.Length)];

            return $"{prefix}-{zone}-{row:D2}{col:D2}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GenerateRandomPath()
        {
            string[] drives = { "C:", "D:", "E:" };
            string[] folders = { "Data", "Sensors", "Logs", "Measurements", "Archive" };
            string[] subfolders = { "2024", "2025", "Production", "Testing", "Raw" };

            string drive = drives[random.Next(drives.Length)];
            string folder = folders[random.Next(folders.Length)];
            string subfolder = subfolders[random.Next(subfolders.Length)];
            string fileName = $"data_{fileCounter}_{DateTime.Now:yyyyMMdd}_{GetRandomInt(1000, 9999)}.dat";

            return $"{drive}\\{folder}\\{subfolder}\\{fileName}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataSetFile GetTestFile()
        {
            fileCounter++;

            // Vary the sensor grid dimensions and value ranges for each file
            int rows = GetRandomInt(1, 20);
            int cols = GetRandomInt(1, 20);
            float valueMin = GetRandomFloat(0f, 50f);
            float valueMax = GetRandomFloat(100f, 500f);

            string[] fileTypes = { "Hourly", "Daily", "Weekly", "Realtime", "Archived" };
            string fileType = fileTypes[random.Next(fileTypes.Length)];

            return new DataSetFile
            {
                Label = $"{fileType}-File-{fileCounter:D4}",
                Path = GenerateRandomPath(),
                Grid = GetTestData(rows, cols, valueMin, valueMax)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<DataSetFile> GetTestFiles(int count)
        {
            var files = new List<DataSetFile>();
            for (int i = 0; i < count; i++)
            {
                files.Add(GetTestFile());
            }
            return files;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<DataSetFile> GetTestNullFiles(int count)
        {
            var nullFiles = new List<DataSetFile>();

            for (int i = 0; i < count; i++)
            {
                var file = new DataSetFile
                {
                    Label = $"TestFile-{i:D3}",
                    Path = $"C:\\Test\\TestFile-{i:D3}.dat",
                    Grid = GetTestData(4, 6)
                };

                switch (i)
                {
                    case 0: // Null grid
                        file.Grid = null;
                        break;
                    case 1: // Null path
                        file.Path = null;
                        break;
                    case 2: // Null label
                        file.Label = null;
                        break;
                    case 3: // Both label and path null
                        file.Label = null;
                        file.Path = null;
                        break;
                    case 4: // Sparse grid with random nulls
                        for (int r = 0; r < file.Grid.GetLength(0); r++)
                            for (int c = 0; c < file.Grid.GetLength(1); c++)
                                if (random.Next(100) < 30)
                                    file.Grid[r, c] = null;
                        break;
                    case 5: // First row null
                        for (int c = 0; c < file.Grid.GetLength(1); c++)
                            file.Grid[0, c] = null;
                        break;
                    case 6: // Last column null
                        for (int r = 0; r < file.Grid.GetLength(0); r++)
                            file.Grid[r, file.Grid.GetLength(1) - 1] = null;
                        break;
                    case 7: // Only corners populated
                        var corners = new SensorData[5, 5];
                        corners[0, 0] = corners[0, 4] = corners[4, 0] = corners[4, 4] =
                            new SensorData { Value = 100f, Label = "Corner", Timestamp = DateTime.Now };
                        file.Grid = corners;
                        break;
                    case 8: // Everything null
                        file.Label = null;
                        file.Path = null;
                        file.Grid = null;
                        break;
                    case 9: // Grid exists but all elements null
                        for (int r = 0; r < file.Grid.GetLength(0); r++)
                            for (int c = 0; c < file.Grid.GetLength(1); c++)
                                file.Grid[r, c] = null;
                        break;
                }

                nullFiles.Add(file);
            }

            return nullFiles;
        }
    }
}


