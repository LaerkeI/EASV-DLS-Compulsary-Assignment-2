using Measurement_Service.Entities;
using Measurement_Service.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

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
        public async Task<IActionResult> GetMeasurementsByPatientSSNAsync(string ssn)
        {
            var measurements = await _repository.GetMeasurementsByPatientSSNAsync(ssn);
            if (measurements == null || !measurements.Any())
            {
                return NotFound();
            }
            return Ok(measurements);
        }

        [HttpGet("weekly-average")]
        public async Task<IActionResult> GetWeeklyAverage([FromQuery] string patientSSN)
        {
            try
            {
                var result = await _repository.GetWeeklyAverageAsync(patientSSN);
                if (result == null)
                {
                    return NotFound("No measurements found for the given patient.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeasurementAsync([FromBody] Measurement measurement)
        {
            try
            {
                await _repository.CreateMeasurementAsync(measurement);
                return Ok(measurement);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-measurement/{id}")]
        public async Task<IActionResult> UpdateMeasurement(int id, [FromBody] Measurement updatedMeasurement)
        {
            try
            {
                await _repository.UpdateMeasurementAsync(id, updatedMeasurement);
                return Ok(updatedMeasurement);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete-measurement/{id}")]
        public async Task<IActionResult> DeleteMeasurement(int id)
        {
            try
            {
                await _repository.DeleteMeasurementAsync(id);
                return Ok("Measurement deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
