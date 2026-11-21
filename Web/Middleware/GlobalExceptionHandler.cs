using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiSampleFinal.Middleware
{
    /// <summary>
    /// Traduce las excepciones del dominio a respuestas ProblemDetails.
    /// El dominio no conoce HTTP: la correspondencia excepción → código vive solo aquí.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                ValidacionDominioException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
                EntidadNoEncontradaException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ProduccionDuplicadaException => (StatusCodes.Status409Conflict, "Producción ya registrada"),
                LecheInsuficienteException => (StatusCodes.Status409Conflict, "Leche insuficiente"),
                InventarioInsuficienteException => (StatusCodes.Status409Conflict, "Inventario insuficiente"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var esErrorInterno = status == StatusCodes.Status500InternalServerError;
            if (esErrorInterno)
                _logger.LogError(exception, "Error no controlado en {Path}", httpContext.Request.Path);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                // En los errores internos no se expone el mensaje de la excepción.
                Detail = esErrorInterno ? "Ocurrió un error inesperado." : exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(
                problem, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);
            return true;
        }
    }
}
