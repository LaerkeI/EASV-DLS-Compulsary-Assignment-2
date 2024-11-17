using MySql.Data.MySqlClient;
using Patient_Service.Entities;
using Dapper;
using Polly;

namespace Patient_Service.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly MySqlConnection _connection;
        private readonly AsyncPolicy _circuitBreakerPolicy;

        public PatientRepository(MySqlConnection connection)
        {
            _connection = connection;

            // Define the circuit breaker policy: Break after 3 failures and stay broken for 30 seconds
            _circuitBreakerPolicy = Policy.Handle<MySqlException>() // Handles database-related exceptions
                .Or<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        public async Task<Patient> GetPatientBySSNAsync(string ssn)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var query = "SELECT * FROM Patients WHERE ssn = @SSN";
                var result = await _connection.QueryFirstOrDefaultAsync<Patient>(query, new { SSN = ssn });

                if (result == null)
                {
                    throw new Exception("No measurements for that patientSSN.");
                }
                return result;
            });
        }

        public async Task CreatePatientAsync(Patient patient)
        {
            await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var query = "INSERT INTO Patients (ssn, mail, name) VALUES (@SSN, @Mail, @Name)";
                await _connection.ExecuteAsync(query, patient);
            });
        }
    }
}
