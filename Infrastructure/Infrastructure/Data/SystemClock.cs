using Domain.Interfaces;

namespace Infrastructure.Data
{
    public class SystemClock : IClock
    {
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
    }
}
