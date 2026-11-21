using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace ApiSampleFinal.Controllers
{
    [ApiController]
    [Route("api/granja")]
    [Produces("application/json")]
    public class GranjaController : ControllerBase
    {
        private readonly IGranjaService _service;

        public GranjaController(IGranjaService service)
        {
            _service = service;
        }

        /// <summary>Consulta la configuración (n, l) y los parámetros por producto.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(GranjaResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GranjaResponse>> Obtener(CancellationToken ct)
            => Ok(await _service.ObtenerAsync(ct));

        /// <summary>Actualiza n, l y los parámetros de rendimiento, vida útil y precio.</summary>
        [HttpPut]
        [ProducesResponseType(typeof(GranjaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GranjaResponse>> Actualizar([FromBody] GranjaUpdateRequest request, CancellationToken ct)
            => Ok(await _service.ActualizarAsync(request, ct));

        /// <summary>Lista las vacas de la granja.</summary>
        [HttpGet("vacas")]
        [ProducesResponseType(typeof(IReadOnlyList<VacaResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<VacaResponse>>> ListarVacas(CancellationToken ct)
            => Ok(await _service.ListarVacasAsync(ct));
    }
}
