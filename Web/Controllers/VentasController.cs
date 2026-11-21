using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace ApiSampleFinal.Controllers
{
    [ApiController]
    [Route("api/ventas")]
    [Produces("application/json")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _service;

        public VentasController(IVentaService service)
        {
            _service = service;
        }

        /// <summary>Registra una venta. Despacha siempre el lote más antiguo no vencido (FIFO) y es atómica.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VentaResponse>> Registrar([FromBody] VentaRequest request, CancellationToken ct)
        {
            var venta = await _service.RegistrarVentaAsync(request, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = venta.Id }, venta);
        }

        /// <summary>Consulta una venta con el detalle de los lotes de los que salió el producto.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VentaResponse>> ObtenerPorId(int id, CancellationToken ct)
            => Ok(await _service.ObtenerVentaAsync(id, ct));
    }
}
