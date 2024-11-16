using Measurement_Service.Entities;

namespace Measurement_Service.Repository
{
    public interface IMeasurementRepository
    {
        Task<IEnumerable<Measurement>> GetMeasurementsByPatientSSNAsync(string ssn);
        Task<Measurement> GetWeeklyAverageAsync(string patientSSN);
        Task CreateMeasurementAsync(Measurement measurement);
    }
}
