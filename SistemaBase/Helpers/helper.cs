using System.Globalization;

namespace SistemaBase.Helpers
{
    public class Helper
    {
        public static string FormatearConSeparadorMiles(decimal? valor)
        {
            if (valor.HasValue)
            {
                return valor.Value.ToString("N0", CultureInfo.GetCultureInfo("es-ES"));
            }
            else
            {
                return string.Empty;
            }
        }
    }
}