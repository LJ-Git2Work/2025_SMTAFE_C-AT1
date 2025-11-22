using Sensing4U_MVP.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        /// <param name="values">[0] = cell text, [1] = min bound, [2] = max bound, [3] = current row, [4] = current col, [5] = searched row, [6] = searched col</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Validate input parameters
            if (values == null || values.Length < 7)
                return White;

            // Get current row index from current row object
            int currentRow = -1;
            if (values[3] is DataRowView rowView)
            {
                var table = rowView.Row.Table;
                currentRow = table.Rows.IndexOf(rowView.Row);
            }

            // Check if this is the searched cell
            if (values[4] is int currentCol && values[5] is int searchedRow && values[6] is int searchedCol)
            {
                if (currentRow == searchedRow && currentCol == searchedCol)
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
