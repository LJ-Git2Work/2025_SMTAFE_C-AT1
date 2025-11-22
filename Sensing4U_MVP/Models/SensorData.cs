using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4U_MVP.Models
{
    /// <summary>
    /// Core data model representing a single sensor reading.
    /// </summary>
    public class SensorData
    {
        /// <summary>
        /// Label identifying this sensor reading
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Timestamp when this reading was taken
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Numeric value of the sensor reading
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        public SensorData()
        {
            Label = "No Label";
            Timestamp = DateTime.Now;
            Value = 0.0f;
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="label">Sensor label</param>
        /// <param name="timestamp">Reading timestamp</param>
        /// <param name="value">Sensor value</param>
        public SensorData(string label, DateTime timestamp, float value)
        {
            Label = label;
            Timestamp = timestamp;
            Value = value;
        }

        /// <summary>
        /// Returns a string representation of this sensor record
        /// </summary>
        public override string ToString()
        {
            return $"{Value:F3}\n{Label}\n{Timestamp:u}";
        }
    }
}
