using Microsoft.AspNetCore.Mvc;

namespace SistemaBase.Filters
{
    public class AccesoPantallaAttribute : TypeFilterAttribute
    {
        public AccesoPantallaAttribute(string nomForma) : base(typeof(AccesoPantallaFilter))
        {
            Arguments = new object[] { nomForma };
        }
    }
}
