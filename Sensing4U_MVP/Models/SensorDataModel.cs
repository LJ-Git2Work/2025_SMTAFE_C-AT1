using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4U_MVP.Models
{
    internal class SensorDataModel
    {
        // == Properties == 
        public float Value { get; set; }
        public string Label { get; set; }
        public DateTime TimeStamp { get; set; }

        // == Constructor ==
        public SensorDataModel()
        {
            Value = 0.0f;
            Label = "No Label";
        }

        // == Serialise to binary ==
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Value);
            writer.Write(Label);
            writer.Write(TimeStamp.ToBinary());
        }
        // == Deserialise from binary ==
        public static SensorDataModel ReadFrom(BinaryReader reader)
        {
            return new SensorDataModel
            {
                Value = reader.ReadSingle(),
                Label = reader.ReadString(),
                TimeStamp = DateTime.FromBinary(reader.ReadInt64())
            };
        }
    }
}
