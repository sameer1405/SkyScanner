using Ark.Tools.ResourceWatcher.WorkerHost;
using Arkive.SkyScanner.Constants;
using SkyScanner.SDK.Dto;
using NLog;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System;
using static SkyScanner.SDK.Configuration.Constants.SkyJson;

namespace SkyScanner.SDK.DataWriter
{
    public class SkyScanner_Writer : IResourceProcessor<SkyScanner_File, SkyScanner_FileMetadataDto>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static int _batchSize = 10000;

        public async Task Process(SkyScanner_File file, CancellationToken ctk = default)
        {
            int rowCnt = 0;
            var _connectionString = ConfigurationManager.ConnectionStrings[SkyScannerConstants.ConfigKeys.SkyScannerDatabase].ConnectionString;

            var data = (SkyScanner_File<FinalDataToGet>)file;

            var storedProc = "sp_Data_SkyScanner";

            var FromTo = $"{data.Metadata.FromPlace}-{data.Metadata.ToPlace}";
            using (SqlConnection conMaintenance = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = conMaintenance.CreateCommand())
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Amount", typeof(double));
                    dt.Columns.Add("Departure", typeof(DateTime));
                    dt.Columns.Add("DurationGoing", typeof(double));
                    dt.Columns.Add("From", typeof(string));
                    dt.Columns.Add("NoDays", typeof(int));
                    dt.Columns.Add("To", typeof(string));
                    dt.Columns.Add("Arrival", typeof(DateTime));
                    dt.Columns.Add("DurationComingBack", typeof(double));
                    dt.Columns.Add("Rating", typeof(double));
                    dt.Columns.Add("UrlFinal", typeof(string));
                    dt.Columns.Add("UrlChecksum", typeof(string));
                    conMaintenance.Open();

                    foreach (var row in data.ParsedData)
                    {
                        dt.Rows.Add(new object[]
                        {
                            row.AmountMoney,
                            row.DateDeparture,
                            row.DurationGoing,
                            data.Metadata.FromPlace,
                            data.Metadata.NoDays,
                            data.Metadata.ToPlace,
                            row.DateArrival,
                            row.DurationComingBack,
                            row.Rating,
                            row.UrlFinal,
                            row.UrlChecksum
                        });

                        if (++rowCnt % _batchSize == 0)
                        {
                            await _executeSP(storedProc, dt, conMaintenance);
                            dt.Clear();
                        }
                    }
                    await _executeSP(storedProc, dt, conMaintenance);
                    conMaintenance.Close();
                }
            }
        }

        private async Task _executeSP(string storedProc, DataTable dt, SqlConnection connection)
        {
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = storedProc;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@table", dt);

                var result = await cmd.ExecuteScalarAsync();
            }
        }
    }
}
