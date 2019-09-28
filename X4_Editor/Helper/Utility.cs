using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X4_Editor.Helper
{
    public static class Utility
    {
        public static double ParseToDouble(string input)
        {
            return double.Parse(input, new NumberFormatInfo() { NumberDecimalSeparator = "." });
        }
    }
}
