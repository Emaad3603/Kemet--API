using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed
{
    public static class JsonCleaner
    {
        public static string CleanJson(string jsonContent)
        {
            // Remove unnecessary newline characters and excessive spaces
            jsonContent = Regex.Replace(jsonContent, @"\s*\n+\s*", ""); // Delete new line
            jsonContent = Regex.Replace(jsonContent, @"\s{2,}", " ");     // Replace multiple spaces with one space
            return jsonContent.Trim(); // Trim leading/trailing spaces
        }
    }
}
