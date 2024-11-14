using Dapper;
using Measurement_Service.Entities;
using MySql.Data.MySqlClient;

namespace Measurement_Service.Repository
{
    public class MeasurementRepository : IMeasurementRepository
    {
        private readonly MySqlConnection _connection;

        public MeasurementRepository(MySqlConnection connection)
        {
            _connection = connection;
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

        public async Task CreateMeasurementAsync(Measurement measurement)
        {
            var query = "INSERT INTO Measurements (date, systolic, diastolic, patientSSN, seen) " +
                        "VALUES (@Date, @Systolic, @Diastolic, @PatientSSN, @Seen)";
            await _connection.ExecuteAsync(query, measurement);
        }
    }
}
