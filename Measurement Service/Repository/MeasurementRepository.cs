using Dapper;
using FeatureHubSDK;
using Measurement_Service.Entities;
using MySql.Data.MySqlClient;

namespace Measurement_Service.Repository
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly MySqlConnection _connection;
        private readonly IClientContext _featureHubContext;

        public MeasurementRepository(MySqlConnection connection, IClientContext featureHubContext)
        {
            _connection = connection;
            _featureHubContext = featureHubContext;
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsByPatientSSNAsync(string ssn)
        {
            var query = @"
                SELECT systolic, diastolic, date
                FROM Measurements
                WHERE patientSSN = @SSN
                ORDER BY date DESC
                LIMIT 42"; //A patient with hypertension measures his blood pressure x3 in the morning and x3 in the afternoon for 1 week, and a doctor analyses the results. 
            var result = await _connection.QueryAsync<Measurement>(query, new { SSN = ssn });
            return result;
        }

        public async Task<Measurement> GetWeeklyAverageAsync(string patientSSN)
        {
            // Check if the feature is enabled using FeatureHub
            bool isFeatureEnabled = _featureHubContext["weekly-average"].IsEnabled;
            
            if (!isFeatureEnabled)
            {
                throw new Exception("Feature is disabled.");
            }

            // Query measurements for the last 7 days
            string query = @"
            SELECT diastolic, systolic
            FROM Measurements
            WHERE patientSSN = @PatientSSN AND date >= DATE_SUB(NOW(), INTERVAL 7 DAY)";

            var measurements = await _connection.QueryAsync<Measurement>(query, new { PatientSSN = patientSSN });

            if (!measurements.Any())
            {
                return null;
            }

            // Calculate averages
            return new Measurement
            {
                Date = DateTime.UtcNow,
                Systolic = (int)Math.Round(measurements.Average(m => m.Systolic)),
                Diastolic = (int)Math.Round(measurements.Average(m => m.Diastolic)),
                Seen = false,
                PatientSSN = patientSSN
            };   
        }

        public async Task CreateMeasurementAsync(Measurement measurement)
        {
            var query = "INSERT INTO Measurements (date, systolic, diastolic, patientSSN, seen) " +
                        "VALUES (@Date, @Systolic, @Diastolic, @PatientSSN, @Seen)";
            await _connection.ExecuteAsync(query, measurement);
        }
    }
}
