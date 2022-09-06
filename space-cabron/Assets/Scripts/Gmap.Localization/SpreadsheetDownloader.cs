using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gmap.Localization
{
    public class SpreadsheetDownloadResult
    {
        public string RawResult;
        public DataTable Result = new DataTable();
        public bool Success => Exception == null;
        public System.Exception Exception = null;
    }

    public class SpreadsheetDownloader
    {
        string url;
        public const char Separator = ',';
        public const char NewLine = '\n';
        public SpreadsheetDownloader(string url)
        {
            this.url = url;
        }

        public SpreadsheetDownloadResult Download()
        {
            var task = Task<SpreadsheetDownloadResult>.Run(DownloadSpreadsheet);
            task.Wait();
            return task.Result;
        }

        private async Task<SpreadsheetDownloadResult> DownloadSpreadsheet()
        {
            HttpClient client = new HttpClient();
            SpreadsheetDownloadResult result = new SpreadsheetDownloadResult();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                result.RawResult = responseBody;
                result.Result = CsvStringToDataTable(responseBody);
            }
            catch (System.Exception ex) {
                result.Exception = ex;
            }

            client.Dispose();
            return result;
        }

        private DataTable CsvStringToDataTable(string str)
        {
            str = str.Replace("\r", "");
            string[] lines = str.Split(NewLine);
            string[] columns = lines[0].Split(Separator);

            DataTable dataTable = new DataTable();
            System.Array.ForEach(columns, c => dataTable.Columns.Add(c));
            System.Array.ForEach(lines, line => dataTable.Rows.Add(line.Split(Separator)));
            return dataTable;
        }
    }
}