using System;
using System.Collections.Generic;
using System.Text;

namespace QSn
{
    public class TablePrinter
    {
        /// <summary>
        /// The header string list
        /// </summary>
        public List<string> header = new List<string>();

        /// <summary>
        /// The actual table, we use nested string lists because im lazy.
        /// TODO Memory-wise, an array _could_ be better, but we are doing constant resizes and stuff
        /// </summary>
        public List<List<string>> grid = new List<List<string>>();

        /// <summary>
        /// The maximum field width, so the max width a cell can be character wise
        /// </summary>
        public int maxFieldWidth = 50;

        public TablePrinter()
        {
            grid.Add(new List<string>() {""});
        }

        /// <summary>
        /// Gets the widest widths of all the columns, plus the addition value if the header is the widest
        /// </summary>
        /// <param name="addition"></param>
        /// <returns></returns>
        private int[] CalculateCellWidths(int addition)
        {
            int columnCount = 0;
            grid.ForEach(x => columnCount = Math.Max(columnCount, x.Count));

            int[] widths = new int[Math.Max(header.Count, columnCount)];

            //Go through each row and get the largest value
            foreach (var row in grid)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    widths[i] = Math.Max(row[i].Length, widths[i]);
                }
            }

            //We also take into account the header widths
            for (int i = 0; i < header.Count; i++)
            {
                widths[i] = Math.Max(header[i].Length, widths[i]) + addition;

                if (widths[i] % 2 != 0)
                {
                    widths[i] += 1;
                }
            }

            return widths;
        }

        /// <summary>
        /// Formats the string to the center, uses the padding string either side to get it to reach <see cref="maxLength"/>.
        /// If it goes past, it is too long (greater than <see cref="maxFieldWidth"/>), it is chopped and prefixed with <see cref="prefix"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newLength"></param>
        /// <param name="padding"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        string FormatStringCenter(string value, int newLength, string padding = " ", string prefix = "...")
        {
            value = value.Trim();

            //If the length is greater than the max field width
            if (value.Length >= maxFieldWidth)
            {
                //Substring to remove the excess beginning values
                value = value.Substring(Math.Max(value.Length - maxFieldWidth, 0));

                //Remove the beginning characters then replace them with the prefix
                value = value.Substring(prefix.Length);
                value = prefix + value;
            }

            //Accumulate the left and right padding values, this could be easily improved
            string leftPad = "";
            string rightPad = "";

            for (int i = 0; i < (Math.Min(newLength, maxFieldWidth) - value.Length) / 2; i++)
            {
                leftPad += padding;
                rightPad += padding;
            }

            //Get the result string by combining the left padding value, the value, and the right padding
            string result = leftPad + value + rightPad;

            //If its odd, we padd the right. This does not account for weirdness in padding length
            if (result.Length % 2 != 0)
            {
                result += padding;
            }


            return result;
        }

        /// <summary>
        /// Prints the table in the fancy formatting. See <see cref="ToStringNoFormat"/> for non-formatting needs
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            int[] widest = CalculateCellWidths(2);
            int headerWidthAdd = 0;

            //These values are used to construct the 'box'
            string colDelim = "|";
            string jointDelim = "+";
            string line = "-"; //\u2500
            //TODO ^ Make sick box formatting

            //Print the header
            for (int i = 0; i < header.Count; i++)
            {
                result.Append(jointDelim);
                result.Append(FormatStringCenter(header[i], widest[i] + headerWidthAdd, line, "---"));
            }

            result.Append(jointDelim + "\n");

            //Print all the rows
            foreach (var row in grid)
            {
                int col = 0;
                foreach (var cell in row)
                {
                    string cellElement = FormatStringCenter(cell, widest[col]);

                    result.Append(colDelim);
                    result.Append(cellElement);
                    col++;
                }

                result.Append(colDelim);
                result.AppendLine();
            }

            //Print the ending flat line
            for (int i = 0; i < header.Count; i++)
            {
                result.Append(jointDelim);
                result.Append(FormatStringCenter("", widest[i] + headerWidthAdd, line, "---"));
            }

            result.Append(jointDelim);

            return result.ToString();
        }


        /// <summary>
        /// Set a particular cell, expanding the rows and columns if needed
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cellContents"></param>
        public void SetCell(int row, int col, string cellContents)
        {
            ExpandRows(row);
            ExpandColumns(col);

            grid[row][col] = cellContents;
        }

        /// <summary>
        /// Expands the rows to support setting rows
        /// </summary>
        /// <param name="targetRow"></param>
        private void ExpandRows(int targetRow)
        {
            int rowsToAdd = Math.Max(0, targetRow - grid.Count + 1);

            for (int i = 0; i < rowsToAdd; i++)
            {
                List<string> newRow = new List<string>();
                for (int j = 0; j < grid[0].Count; j++)
                {
                    newRow.Add("");
                }

                grid.Add(newRow);
            }
        }

        /// <summary>
        /// Expand all the columns to fit the target column
        /// </summary>
        /// <param name="targetCol"></param>
        private void ExpandColumns(int targetCol)
        {
            int colsToAdd = Math.Max(0, targetCol - (grid.Count > 0 ? grid[0].Count : 0) + 1);

            for (int i = 0; i < grid.Count; i++)
            {
                for (int j = 0; j < colsToAdd; j++)
                {
                    grid[i].Add("");
                }
            }
        }

        /// <summary>
        /// Prints the table without any fancy formatting
        /// </summary>
        /// <returns></returns>
        public string ToStringNoFormat()
        {
            StringBuilder value = new StringBuilder();

            value.Append(string.Join(" | ", header));

            for (int i = 0; i < grid.Count; i++)
            {
                value.Append("\n");
                value.Append(string.Join(" | ", grid[i]));
            }

            return value.ToString();
        }
    }
}