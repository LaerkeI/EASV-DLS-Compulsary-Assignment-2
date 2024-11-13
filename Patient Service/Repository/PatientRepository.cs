using MySql.Data.MySqlClient;
using Patient_Service.Entities;
using Dapper;

namespace Patient_Service.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly MySqlConnection _connection;

        public PatientRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Patient> GetPatientBySSNAsync(string ssn)
        {
            var query = "SELECT * FROM Patients WHERE ssn = @SSN";
            var result = await _connection.QueryFirstOrDefaultAsync<Patient>(query, new { SSN = ssn });
            return result;
        }

        public async Task CreatePatientAsync(Patient patient)
        {
            var query = "INSERT INTO Patients (ssn, mail, name) VALUES (@SSN, @Mail, @Name)";
            await _connection.ExecuteAsync(query, patient);
        }
    }
}
