using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class Granja
    {
        private readonly List<Vaca> _vacas = new();
        private readonly List<ParametroProducto> _parametros = new();

        public int Id { get; private set; }
        public string Nombre { get; private set; }

        /// <summary>n: número de vacas de la granja.</summary>
        public int NumeroVacas { get; private set; }

        /// <summary>l: litros que produce cada vaca por día.</summary>
        public decimal LitrosPorVaca { get; private set; }

        public IReadOnlyCollection<Vaca> Vacas => _vacas;
        public IReadOnlyCollection<ParametroProducto> Parametros => _parametros;

        /// <summary>Producción diaria esperada: n × l.</summary>
        public decimal ProduccionDiariaEsperada => NumeroVacas * LitrosPorVaca;

        private Granja() { Nombre = string.Empty; }

        public Granja(string nombre, int numeroVacas, decimal litrosPorVaca)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidacionDominioException("La granja debe tener nombre.");
            Nombre = nombre.Trim();
            Actualizar(numeroVacas, litrosPorVaca);
        }

        /// <summary>
        /// Valores sugeridos por el taller (n = 10, l = 15; queso 10 L/kg y 30 días; yogurt 1 L/L y 15 días).
        /// Son solo el punto de partida: se pueden editar con PUT /api/granja.
        /// Los precios son supuestos: el queso toma el del ejemplo del taller (28 000) y el yogurt es inventado.
        /// </summary>
        public static Granja CrearConValoresSugeridos()
        {
            var granja = new Granja("Granja lechera", numeroVacas: 10, litrosPorVaca: 15);
            granja.ConfigurarProducto(TipoProducto.Queso, litrosPorUnidad: 10, vidaUtilDias: 30, precio: 28000);
            granja.ConfigurarProducto(TipoProducto.Yogurt, litrosPorUnidad: 1, vidaUtilDias: 15, precio: 5000);
            return granja;
        }

        /// <summary>
        /// Cambia n y l y deja la lista de vacas coherente con n: crea las que falten y
        /// desactiva las sobrantes. Requiere que <see cref="Vacas"/> esté cargada
        /// (el repositorio de la granja siempre la incluye).
        /// </summary>
        public void Actualizar(int numeroVacas, decimal litrosPorVaca)
        {
            if (numeroVacas <= 0)
                throw new ValidacionDominioException("El número de vacas debe ser mayor que cero.");
            if (litrosPorVaca <= 0)
                throw new ValidacionDominioException("Los litros por vaca deben ser mayores que cero.");

            NumeroVacas = numeroVacas;
            LitrosPorVaca = litrosPorVaca;
            SincronizarVacas();
        }

        public void ConfigurarProducto(TipoProducto tipo, decimal litrosPorUnidad, int vidaUtilDias, decimal precio)
        {
            var existente = _parametros.FirstOrDefault(p => p.Tipo == tipo);
            if (existente is null)
                _parametros.Add(new ParametroProducto(tipo, litrosPorUnidad, vidaUtilDias, precio));
            else
                existente.Actualizar(litrosPorUnidad, vidaUtilDias, precio);
        }

        public ParametroProducto ObtenerParametro(TipoProducto tipo)
            => _parametros.FirstOrDefault(p => p.Tipo == tipo)
               ?? throw new EntidadNoEncontradaException("el parámetro del producto", tipo);

        private void SincronizarVacas()
        {
            while (_vacas.Count < NumeroVacas)
                _vacas.Add(new Vaca($"Vaca {_vacas.Count + 1}"));

            // Las recién creadas todavía no tienen Id (0): van al final.
            var ordenadas = _vacas.OrderBy(v => v.Id == 0 ? int.MaxValue : v.Id).ToList();
            for (var i = 0; i < ordenadas.Count; i++)
            {
                if (i < NumeroVacas) ordenadas[i].Activar();
                else ordenadas[i].Desactivar();
            }
        }
    }
}
