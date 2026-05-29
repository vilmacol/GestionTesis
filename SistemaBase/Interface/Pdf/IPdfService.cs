

using System.Threading.Tasks;

namespace SistemaBase.Interface.Pdf
{

    public interface IPdfService
    {
       
        byte[] PrintToPdf(object data);


        Task<byte[]> PrintPdf(/*Stream stream,*/ IList<object> data);



    }
}