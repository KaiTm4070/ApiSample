using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace ApiSampleFinal.Controllers
{
    [ApiController]
    [Route("api/produccion")]
    [Produces("application/json")]
    public class ProduccionController : ControllerBase
    {
        private readonly IProduccionService _service;

        public ProduccionController(IProduccionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Registra la producción de leche de una fecha. Sin detalle, cada vaca activa produce l litros (n × l).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProduccionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProduccionResponse>> Registrar([FromBody] ProduccionRequest request, CancellationToken ct)
        {
            var resultado = await _service.RegistrarProduccionAsync(request, ct);
            var fecha = resultado.Fecha.ToString("yyyy-MM-dd");
            return Created($"/api/reportes/leche-diaria?desde={fecha}&hasta={fecha}", resultado);
        }

        /// <summary>Transforma leche de una fecha en un lote de queso o de yogurt.</summary>
        [HttpPost("transformaciones")]
        [ProducesResponseType(typeof(TransformacionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TransformacionResponse>> Transformar([FromBody] TransformacionRequest request, CancellationToken ct)
        {
            var resultado = await _service.TransformarAsync(request, ct);
            return Created($"/api/inventario/lotes/{resultado.LoteId}", resultado);
        }
    }
}
