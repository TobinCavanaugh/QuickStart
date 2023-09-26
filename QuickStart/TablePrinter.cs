using System;
using System.Collections.Generic;
using System.Text;

namespace QSn
{
    public class TablePrinter
    {
        public List<string> header = new List<string>();
        public List<List<string>> grid = new List<List<string>>();

        public int maxFieldWidth = 50;

        public TablePrinter()
        {
            grid.Add(new List<string>() {""});
        }

        private int[] CalculateCellWidths(int addition)
        {
            int columnCount = 0;
            grid.ForEach(x => columnCount = Math.Max(columnCount, x.Count));

            int[] widths = new int[Math.Max(header.Count, columnCount)];

            foreach (var row in grid)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    widths[i] = Math.Max(row[i].Length, widths[i]);
                }
            }

            for (int i = 0; i < header.Count; i++)
            {
                if (widths == null)
                {
                    continue;
                }

                widths[i] = Math.Max(header[i].Length, widths[i]) + addition;

                if (widths[i] % 2 != 0)
                {
                    widths[i] += 1;
                }
            }

            return widths;
        }

        string PadLeftRight(string value, int maxLength, string padding = " ", string ending = "...")
        {
            value = value.Trim();

            if (value.Length >= maxFieldWidth)
            {
                value = value.Substring(Math.Max(value.Length - maxFieldWidth, 0));
                // value = value.Substring(value.Length- ending.Length);

                value = value.Substring(ending.Length);
                value = ending + value;
            }

            string leftPad = "";
            for (int i = 0; i < (Math.Min(maxLength, maxFieldWidth) - value.Length) / 2; i++)
            {
                leftPad += padding;
            }

            string rightPad = "";
            for (int i = 0; i < (Math.Min(maxLength, maxFieldWidth) - value.Length) / 2; i++)
            {
                rightPad += padding;
            }

            string result = leftPad + value + rightPad;

            if (result.Length % 2 != 0)
            {
                result += padding;
            }


            return result;
        }

        public override string ToString()
        {

            StringBuilder result = new StringBuilder();
            int[] widest = CalculateCellWidths(2);
            int headerWidthAdd = 0;
            string colDelim = "|";
            string jointDelim = "+";
            string line = "-";

            //Print the header
            for (int i = 0; i < header.Count; i++)
            {
                result.Append(jointDelim);
                result.Append(PadLeftRight(header[i], widest[i] + headerWidthAdd, line, "---"));
            }

            result.Append(jointDelim + "\n");

            foreach (var row in grid)
            {
                int col = 0;
                foreach (var cell in row)
                {
                    string cellElement = PadLeftRight(cell, widest[col]);

                    result.Append(colDelim);
                    result.Append(cellElement);
                    col++;
                }

                result.Append(colDelim);
                result.AppendLine();
            }


            //Print the header
            for (int i = 0; i < header.Count; i++)
            {
                result.Append(jointDelim);
                result.Append(PadLeftRight("", widest[i] + headerWidthAdd, line, "---"));
            }

            result.Append(jointDelim);

            return result.ToString();
        }


        public void SetCell(int row, int col, string cellContents)
        {
            ExpandRows(row);
            ExpandColumns(col);

            grid[row][col] = cellContents;
        }

        private void ExpandRows(int targetRow)
        {
            // 12, 7 = 5
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