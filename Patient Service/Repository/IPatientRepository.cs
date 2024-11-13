using Patient_Service.Entities;

namespace Patient_Service.Repository
{
    public interface IPatientRepository
    {
        Task<Patient> GetPatientBySSNAsync(string ssn);
        Task CreatePatientAsync(Patient patient);
    }
}
