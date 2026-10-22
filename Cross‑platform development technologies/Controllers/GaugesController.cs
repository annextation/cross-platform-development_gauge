using Cross_platform_development_technologies.Models;
using Cross_platform_development_technologies.Services;
using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cross_platform_development_technologies.Controllers
{
    [ApiController]
    [Route("api/gauges")]
    public class GaugesController : ControllerBase
    {
        private readonly GaugeService _gaugeService;

        public GaugesController(GaugeService gaugeService)
        {
            _gaugeService = gaugeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<GaugeResponse>>> GetAll()
        {
            var gauges = await _gaugeService.GetAllAsync();

            return Ok(gauges.Select(item => ToResponse(item)).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GaugeResponse>> GetById(int id)
        {
            var gauge = await _gaugeService.GetByIdAsync(id);

            if (gauge is null)
            {
                return NotFound(new
                {
                    message = $"Манометр с идентификатором {id} не найден."
                });
            }

            return Ok(ToResponse(gauge));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GaugeResponse>> Create(
        [FromBody] CreateGaugeRequest request)
        {
            try
            {
                var gauge = await _gaugeService.CreateAsync(
                    request.Name,
                    request.ScaleMin!.Value,
                    request.ScaleMax!.Value,
                    request.AllowedMin!.Value,
                    request.AllowedMax!.Value);

                return CreatedAtAction(
                            nameof(GetById),
                            new { id = gauge.Id },
                            ToResponse(gauge));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/allowed-range")]
        public async Task<ActionResult<GaugeResponse>> UpdateAllowedRange(
        int id,
        [FromBody] UpdateGaugeRangeRequest request)
        {
            try
            {
                var gauge = await _gaugeService.UpdateAllowedRangeAsync(
                    id,
                    request.AllowedMin!.Value,
                    request.AllowedMax!.Value);

                if (gauge is null)
                {
                    return NotFound(new
                    {
                        message = $"Манометр с идентификатором {id} не найден."
                    });
                }

                return Ok(ToResponse(gauge));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _gaugeService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = $"Манометр с идентификатором {id} не найден."
                    });
                }

                return NoContent();
            }
            catch (BusinessConflictException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }
        private static GaugeResponse ToResponse(Gauge gauge)
        {
            return new GaugeResponse
            {
                Id = gauge.Id,
                Name = gauge.Name,
                ScaleMin = gauge.ScaleMin,
                ScaleMax = gauge.ScaleMax,
                AllowedMin = gauge.AllowedMin,
                AllowedMax = gauge.AllowedMax
            };
        }

    }
}