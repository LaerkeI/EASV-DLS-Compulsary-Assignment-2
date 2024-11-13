using Measurement_Service.Entities;
using Measurement_Service.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Measurement_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementController : ControllerBase
    {
        private readonly IMeasurementRepository _repository;

        public MeasurementController(IMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{ssn}")]
        public async Task<IActionResult> GetMeasurements(string ssn)
        {
            var measurements = await _repository.GetMeasurementsByPatientSSNAsync(ssn);
            if (measurements == null || !measurements.Any())
            {
                return NotFound();
            }
            return Ok(measurements);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeasurement([FromBody] Measurement measurement)
        {
            await _repository.CreateMeasurementAsync(measurement);
            return CreatedAtAction(nameof(GetMeasurements), new { ssn = measurement.PatientSSN }, measurement);
        }
    }
}
