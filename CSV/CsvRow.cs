using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerServiceScheduler
{
    public class CsvRow : List<object>
    {
        /// <summary>
        /// Gets or sets the line text
        /// </summary>
        public string LineText { get; set; }
    }
}
