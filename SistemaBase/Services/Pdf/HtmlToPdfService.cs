
using Microsoft.AspNetCore.Mvc;
using SistemaBase.Common;
using SistemaBase.Interface.Pdf;
using IronPdf;
using Wkhtmltopdf.NetCore;
using System.IO;
using System;

namespace SistemaBase.Service.Pdf
{
    /// <summary>
    /// Generate invoice, shipment as pdf (from html template to pdf)
    /// </summary>
    public class HtmlToPdfService : IPdfService
    {
        private const string Template = "~/Views/PdfTemplates/PdfTemplate.cshtml";
        private const string _paginationHeader = "/assets/pdf/pagination.html";

        private readonly IViewRenderService _viewRenderService;
        private readonly IWebHostEnvironment _environment;
        private readonly IGeneratePdf _generatePdf;

        public HtmlToPdfService(IViewRenderService viewRenderService, IWebHostEnvironment environment, IGeneratePdf generatePdf)
        {
            _viewRenderService = viewRenderService;
            _environment = environment;
            _generatePdf = generatePdf;
        }

        public async Task<byte[]> PrintPdf(/*Stream stream,*/ IList<object> data)
        {
            //if (stream == null)
            //    throw new ArgumentNullException(nameof(stream));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var html =  _viewRenderService.RenderToStringAsync<(IList<object>, string)>(Template,
                new(data, "")).Result;
           // return GetPDF(html);

            _generatePdf.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                HeaderHtml = _environment.WebRootPath + _paginationHeader,
               // PageOrientation= Wkhtmltopdf.NetCore.Options.Orientation.Landscape,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins() { Bottom = 10, Left = 10, Right = 10, Top = 100 },
            });
            //TextReader sr = new StringReader(html);
            //var doc = Scryber.Components.Document.ParseDocument(sr, Scryber.ParseSourceType.DynamicContent);
            //doc.SaveAsPDF(stream);
           var pdfBytes = _generatePdf.GetPDF(html);
           // stream.Write(pdfBytes);
            return pdfBytes;
        }




        public byte[] PrintToPdf(object data)
        {
            try
            {

            
            if (data == null)
                throw new ArgumentNullException(nameof(data));

                //var fileName = $"Reportprueba.pdf";

                //var dir = _environment.WebRootPath + "/assets/files/exportimport";
                //if (dir == null)
                //    throw new ArgumentNullException(nameof(dir));

                //if (!System.IO.Directory.Exists(dir))
                //{
                //    System.IO.Directory.CreateDirectory(dir);
                //}

                //var filePath = Path.Combine(dir, fileName.Trim());
                //using var fileStream = new FileStream(filePath, FileMode.Create);

                List<Object> collection = new List<Object>((IEnumerable<Object>)data);

                return PrintPdf(/*fileStream,*/ collection).Result;

            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }

    }
}