using Cross_platform_development_technologies.Contracts;
using Cross_platform_development_technologies.Exceptions;
using Cross_platform_development_technologies.Models;
using Cross_platform_development_technologies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cross_platform_development_technologies.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionsController(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SessionResponse>> Create(
            [FromBody] CreateSessionRequest request)
        {
            try
            {
                var session = await _sessionService.CreateAsync(
                    request.GaugeId!.Value,
                    request.MeasurementIntervalSeconds!.Value);

                if (session is null)
                {
                    return NotFound(new
                    {
                        message = $"Манометр с идентификатором {request.GaugeId} не найден."
                    });
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = session.Id },
                    ToResponse(session));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SessionResponse>> GetById(int id)
        {
            var session = await _sessionService.GetByIdAsync(id);

            if (session is null)
            {
                return NotFound(new
                {
                    message = $"Сеанс с идентификатором {id} не найден."
                });
            }

            return Ok(ToResponse(session));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}/complete")]
        public async Task<ActionResult<SessionResponse>> Complete(int id)
        {
            try
            {
                var session = await _sessionService.CompleteAsync(id);

                if (session is null)
                {
                    return NotFound(new
                    {
                        message = $"Сеанс с идентификатором {id} не найден."
                    });
                }

                return Ok(ToResponse(session));
            }
            catch (BusinessConflictException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [HttpGet("{id:int}/measurements")]
        public async Task<ActionResult<List<SessionMeasurementResponse>>>
        GetMeasurements(int id)
        {
            var measurements = await _sessionService.GetMeasurementsAsync(id);

            if (measurements is null)
            {
                return NotFound(new
                {
                    message = $"Сеанс с идентификатором {id} не найден."
                });
            }

            return Ok(measurements);
        }

        [HttpGet]
        public async Task<ActionResult<List<SessionResponse>>> GetAll()
        {
            var sessions = await _sessionService.GetAllAsync();

            return Ok(sessions.Select(item => ToResponse(item)).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _sessionService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = $"Сеанс с идентификатором {id} не найден."
                    });
                }

                return NoContent();
            }
            catch (BusinessConflictException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }
        private static SessionResponse ToResponse(ObservationSession session)
        {
            return new SessionResponse
            {
                Id = session.Id,
                GaugeId = session.GaugeId,
                MeasurementIntervalSeconds = session.MeasurementIntervalSeconds,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt
            };
        }

    }
}