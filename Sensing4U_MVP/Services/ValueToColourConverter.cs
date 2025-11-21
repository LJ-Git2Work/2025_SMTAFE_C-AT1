using Sensing4U_MVP.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Sensing4U_MVP.Services
{
    /// <summary>
    /// Converts sensor values to colors based on min/max bounds.
    /// Or by Search Result 
    /// </summary>
    public class ValueToColourConverter : IMultiValueConverter
    {
        // Predefined brushes
        private static readonly Brush Tomato = Brushes.Tomato;
        private static readonly Brush SkyBlue = Brushes.SkyBlue;
        private static readonly Brush YellowGreen = Brushes.YellowGreen;
        private static readonly Brush White = Brushes.White;
        private static readonly Brush Gold = Brushes.Gold;

        /// <summary>
        /// Convert cell value to color based on bounds
        /// </summary>
        /// <param name="values">[0] = cell text, [1] = min bound, [2] = max bound, [3] = Search Result</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Validate input parameters
            if (values == null || values.Length < 3)
                return White;

            // Check for Search Result match
            if (values[3] != null)
            {
                if ((string)values[3] == (string)values[0])
                    return Gold;
            }

            // Extract min and max bounds
            if (values[1] is not float min || values[2] is not float max)
                return White;

            // Parse the cell content to extract value
            string cellText = values[0]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(cellText))
                return White;

            string[] lines = cellText.Split('\n');

            // First line should contain the numeric value
            if (lines.Length < 1 || !float.TryParse(lines[0], out float cellValue))
                return White;

            // Determine color based solely on value vs bounds
            if (cellValue > max)
                return Tomato;      // Above max = red

            if (cellValue < min)
                return SkyBlue;     // Below min = blue

            return YellowGreen;     // Within range = green
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
