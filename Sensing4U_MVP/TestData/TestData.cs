using Sensing4U_MVP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sensing4U_MVP.TestData
{
    public static class TestData
    {
        public static float increment = 0.5f;
        public static SensorDataModel[,] GetTestData()
        {
            SensorDataModel[,] testdata = new SensorDataModel[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    testdata[i, j] = new SensorDataModel
                    {
                        Value = 5.5f + increment,
                        Label = "TestLabel",
                        TimeStamp = DateTime.Now
                    };
                }
            }
            increment += 2.5f;
            return testdata;
        }
    }
}
