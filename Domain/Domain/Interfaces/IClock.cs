namespace Domain.Interfaces
{
    /// <summary>Fecha actual inyectable (RN-10), para poder simular varios días en las pruebas.</summary>
    public interface IClock
    {
        DateOnly Today { get; }
    }
}
