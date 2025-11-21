using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sensing4U_MVP.Models;
using System.Windows;

namespace Sensing4U_MVP.Services
{
    public class DataProcessor
    {
        public static string masterPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "MasterIndex");
        public static string masterFile = masterPath + "master_index.bin";
        public static Collection<DataFileModel> ProcessedDataFiles { get; set; } = new Collection<DataFileModel>();

        // == Write metadata of all files to a master index file ==
        public static void SaveAll()
        {
            if (!File.Exists(masterFile))
                throw new FileNotFoundException("Master index file not found.", masterFile);

            using (var masterWriter = new BinaryWriter(new FileStream(masterFile, FileMode.Create)))
            {
                masterWriter.Write(ProcessedDataFiles.Count);
                foreach (var File in ProcessedDataFiles)
                {
                    masterWriter.Write(File.Label);
                    masterWriter.Write(File.Path);
                    SaveToBinary(File);
                }
            }
        }

        // == Write a single DataFile to binary ==
        public static void SaveToBinary(DataFileModel dataFile)
        {
            using (var dataFileWriter = new BinaryWriter(new FileStream(dataFile.Path, FileMode.Create)))
                dataFile.WriteTo(dataFileWriter);
        }

        // == Read metadata of all files from a master index file ==
        public static void LoadAll()
        {
            if (!File.Exists(masterFile))
                throw new FileNotFoundException("Master index file not found.", masterFile);

            using (var masterReader = new BinaryReader(new FileStream(masterFile, FileMode.Open)))
            {
                int fileCount = masterReader.ReadInt32();
                ProcessedDataFiles.Clear();
                for (int i = 0; i < fileCount; i++)
                {
                    string label = masterReader.ReadString();
                    string path = masterReader.ReadString();
                    DataFileModel dataFile = LoadFromBinary(path);
                    ProcessedDataFiles.Add(dataFile);
                }
            }
        }
        // == Read a single DataFile from binary ==
        public static DataFileModel LoadFromBinary(string dataFilePath)
        {
            DataFileModel dataFile = DataFileModel.ReadFrom(new BinaryReader(new FileStream(dataFilePath, FileMode.Open)));
            dataFile.Path = dataFilePath;
            return dataFile;
        }
        public static void SaveToCSV()
        {
            MessageBox.Show("Filed saved to CSV");
        }
        public static void LoadFromCSV() { }
        public static void Process() { }
        public static void Unload() { }
        public static void SearchByValue() { }
        public static void CalculateAverage() { }
        public static void SortByValue() { }
    }
}
