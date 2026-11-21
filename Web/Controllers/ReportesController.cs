using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dtos;

namespace ApiSampleFinal.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    [Produces("application/json")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _service;

        public ReportesController(IReporteService service)
        {
            _service = service;
        }

        /// <summary>Ventas del período: fecha, producto, cantidad, precio, total y total general.</summary>
        [HttpGet("ventas")]
        [ProducesResponseType(typeof(ReporteVentasResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReporteVentasResponse>> Ventas(
            [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, [FromQuery] TipoProducto? tipo, CancellationToken ct)
            => Ok(await _service.VentasAsync(desde, hasta, tipo, ct));

        /// <summary>Leche total producida por día, litros transformados y litros no aprovechados.</summary>
        [HttpGet("leche-diaria")]
        [ProducesResponseType(typeof(ReporteLecheDiariaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReporteLecheDiariaResponse>> LecheDiaria(
            [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken ct)
            => Ok(await _service.LecheDiariaAsync(desde, hasta, ct));

        /// <summary>Producto elaborado por día y por tipo (queso y yogurt).</summary>
        [HttpGet("productodiario")]
        [ProducesResponseType(typeof(ReporteProductoDiarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReporteProductoDiarioResponse>> ProductoDiario(
            [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken ct)
            => Ok(await _service.ProductoDiarioAsync(desde, hasta, ct));

        /// <summary>Por producto: total producido, vendido, disponible y dañado.</summary>
        [HttpGet("producto-total")]
        [ProducesResponseType(typeof(ReporteProductoTotalResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ReporteProductoTotalResponse>> ProductoTotal(CancellationToken ct)
            => Ok(await _service.ProductoTotalAsync(ct));

        /// <summary>Producto dañado por lote y por tipo, con fecha de vencimiento (el período filtra por vencimiento).</summary>
        [HttpGet("productodanado")]
        [ProducesResponseType(typeof(ReporteProductoDanadoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReporteProductoDanadoResponse>> ProductoDanado(
            [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken ct)
            => Ok(await _service.ProductoDanadoAsync(desde, hasta, ct));

        /// <summary>Producción por vaca: promedio, la que más y la que menos produce, y las que están bajo el promedio.</summary>
        [HttpGet("vacas")]
        [ProducesResponseType(typeof(ReporteVacasResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReporteVacasResponse>> Vacas(
            [FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta, CancellationToken ct)
            => Ok(await _service.VacasAsync(desde, hasta, ct));
    }
}
