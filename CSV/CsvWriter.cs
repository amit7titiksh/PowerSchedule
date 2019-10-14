using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerServiceScheduler
{
    public class CsvWriter : StreamWriter
    {
        /// <summary>
        /// Gets or sets the Tab
        /// </summary>
        public char Tab { get; set; }
        public CsvWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvWriter(string fileName)
            : base(fileName)
        {

        }

        public CsvWriter(string fileName, bool append)
            : base(fileName, append)
        {

        }

        public CsvWriter(string fileName, bool append, char pTab)
            : base(fileName, append)
        {
            this.Tab = pTab;
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (object value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(Tab);
                if (value != null)
                {

                    // Implement special handling for values that contain comma or quote
                    // Enclose in quotes and double up any double quotes
                    if (value.ToString().IndexOfAny(new char[] { '"', ',', Tab }) != -1)
                        builder.AppendFormat("\"{0}\"", value.ToString().Replace("\"", "\"\""));
                    else
                        builder.Append(value);
                }
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }

    }
}
