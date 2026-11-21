using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace ApiSampleFinal.Controllers
{
    [ApiController]
    [Route("api/inventario")]
    [Produces("application/json")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _service;

        public InventarioController(IInventarioService service)
        {
            _service = service;
        }

        /// <summary>Existencias vendibles (con cantidad y sin vencer) y sus lotes a una fecha.</summary>
        /// <param name="fecha">Fecha de consulta (yyyy-MM-dd). Por defecto, hoy.</param>
        /// <param name="tipo">Queso o Yogurt. Por defecto, ambos.</param>
        [HttpGet]
        [ProducesResponseType(typeof(InventarioResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<InventarioResponse>> Consultar(
            [FromQuery] DateOnly? fecha, [FromQuery] TipoProducto? tipo, CancellationToken ct)
            => Ok(await _service.ObtenerInventarioAsync(fecha, tipo, ct));

        /// <summary>Consulta un lote por identificador.</summary>
        [HttpGet("lotes/{id:int}")]
        [ProducesResponseType(typeof(LoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoteResponse>> ObtenerLote(int id, CancellationToken ct)
            => Ok(await _service.ObtenerLoteAsync(id, ct));

        /// <summary>Procesa vencimientos: lo que quedó disponible en lotes vencidos pasa a producto dañado.</summary>
        [HttpPost("vencimientos")]
        [ProducesResponseType(typeof(VencimientosResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<VencimientosResponse>> ProcesarVencimientos([FromQuery] DateOnly? fecha, CancellationToken ct)
            => Ok(await _service.ProcesarVencimientosAsync(fecha, ct));
    }
}
