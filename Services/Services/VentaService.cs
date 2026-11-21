using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Services.Dtos;
using Services.Extensions;
using Services.Mappers;

namespace Services
{
    public class VentaService : IVentaService
    {
        private readonly IGranjaRepository _granjas;
        private readonly ILoteRepository _lotes;
        private readonly IVentaRepository _ventas;
        private readonly IUnitOfWork _uow;
        private readonly IClock _clock;

        public VentaService(
            IGranjaRepository granjas, ILoteRepository lotes, IVentaRepository ventas,
            IUnitOfWork uow, IClock clock)
        {
            _granjas = granjas;
            _lotes = lotes;
            _ventas = ventas;
            _uow = uow;
            _clock = clock;
        }

        /// <summary>
        /// RN-05 a RN-09. Despacha siempre el lote más antiguo no vencido (FIFO). Es atómica:
        /// si no alcanzan las existencias se rechaza completa y no se modifica nada.
        /// </summary>
        public async Task<VentaResponse> RegistrarVentaAsync(VentaRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (!Enum.IsDefined(request.TipoProducto))
                throw new ValidacionDominioException("Tipo de producto no válido.");
            if (request.Cantidad <= 0)
                throw new ValidacionDominioException("La cantidad a vender debe ser mayor que cero.");

            var fecha = request.Fecha ?? _clock.Today;
            var granja = await _granjas.ObtenerGranjaAsync(ct);
            var precio = granja.ObtenerParametro(request.TipoProducto).Precio;

            // Lotes con existencias y no vencidos a la fecha de venta, del más antiguo al más nuevo.
            var lotes = await _lotes.ObtenerVendiblesAsync(request.TipoProducto, fecha, ct);

            var disponible = lotes.Sum(l => l.CantidadDisponible);
            if (disponible < request.Cantidad)
                throw new InventarioInsuficienteException(request.TipoProducto, request.Cantidad, disponible);

            var venta = new Venta(fecha, request.TipoProducto, precio);
            var restante = request.Cantidad;
            foreach (var lote in lotes)
            {
                if (restante == 0) break;

                var tomar = Math.Min(lote.CantidadDisponible, restante);
                lote.Descontar(tomar);
                venta.AgregarDetalle(lote, tomar);
                restante -= tomar;
            }

            await _ventas.AgregarAsync(venta, ct);
            await _uow.SaveChangesAsync(ct); // una sola transacción: venta + descuento de lotes

            return VentaMapper.ToResponse(venta);
        }

        public async Task<VentaResponse> ObtenerVentaAsync(int id, CancellationToken ct = default)
        {
            var venta = await _ventas.ObtenerConDetalleAsync(id, ct)
                        ?? throw new EntidadNoEncontradaException("la venta", id);
            return VentaMapper.ToResponse(venta);
        }
    }
}
