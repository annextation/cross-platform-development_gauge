using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Models;
using Cross_platform_development_technologies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cross_platform_development_technologies.Controllers
{
    [ApiController]
    [Route("api/measurements")]
    public class MeasurementsController : ControllerBase
    {
        private readonly MeasurementService _measurementService;

        public MeasurementsController(
            MeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MeasurementResponse>> Create(
            [FromBody] CreateMeasurementRequest request)
        {
            try
            {
                var measurement = await _measurementService.CreateAsync(
                    request.SessionId!.Value,
                    request.MeasuredAt!.Value,
                    request.Pressure!.Value,
                    request.Confidence!.Value);

                if (measurement is null)
                {
                    return NotFound(new
                    {
                        message = $"Сеанс с идентификатором {request.SessionId} не найден."
                    });
                }

                return CreatedAtAction(
                        nameof(GetById),
                        new { id = measurement.Id },
                        ToResponse(measurement));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MeasurementResponse>> GetById(int id)
        {
            var measurement = await _measurementService.GetByIdAsync(id);

            if (measurement is null)
            {
                return NotFound(new
                {
                    message = $"Измерение с идентификатором {id} не найдено."
                });
            }

            return Ok(ToResponse(measurement));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _measurementService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Измерение с идентификатором {id} не найдено."
                });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/result")]
        public async Task<ActionResult<MeasurementResponse>> UpdateResult(
        int id,
        [FromBody] UpdateMeasurementResultRequest request)
        {
            try
            {
                var measurement = await _measurementService.UpdateResultAsync(
                    id,
                    request.Pressure!.Value,
                    request.Confidence!.Value);

                if (measurement is null)
                {
                    return NotFound(new
                    {
                        message = $"Измерение с идентификатором {id} не найдено."
                    });
                }

                return Ok(ToResponse(measurement));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
        private static MeasurementResponse ToResponse(Measurement measurement)
        {
            return new MeasurementResponse
            {
                Id = measurement.Id,
                SessionId = measurement.SessionId,
                MeasuredAt = measurement.MeasuredAt,
                Pressure = measurement.Pressure,
                Confidence = measurement.Confidence
            };
        }

    }
}