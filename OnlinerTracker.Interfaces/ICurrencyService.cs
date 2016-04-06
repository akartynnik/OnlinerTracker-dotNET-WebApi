using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface ICurrencyService
    {
        Currency GetCurrent(CurrencyType currencyType);
    }
}
