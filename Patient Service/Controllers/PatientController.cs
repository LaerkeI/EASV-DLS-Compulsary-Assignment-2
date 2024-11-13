using Microsoft.AspNetCore.Mvc;
using Patient_Service.Entities;
using Patient_Service.Repository;

namespace Patient_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _repository;

        public PatientController(IPatientRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{ssn}")]
        public async Task<IActionResult> GetPatient(string ssn)
        {
            var patient = await _repository.GetPatientBySSNAsync(ssn);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
        {
            await _repository.CreatePatientAsync(patient);
            return CreatedAtAction(nameof(GetPatient), new { ssn = patient.SSN }, patient);
        }
    }

}
