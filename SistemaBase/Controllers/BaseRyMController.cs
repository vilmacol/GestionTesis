using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Scryber.Components;
using SistemaBase.Interface.Pdf;
using System;
using System.IO;
using System.Text;

namespace SistemaBase.Controllers
{
    [Authorize]
    public class BaseRyMController : Controller
    {
        private readonly IPdfService _pdfservice;

        public BaseRyMController(IServiceProvider serviceProvider)
        {
            _pdfservice = serviceProvider.GetRequiredService<IPdfService>();
        }

        protected bool IsPdfResponseView()
        {
            var viewJson = Request?.Headers["ExportMode"];
            if(viewJson?.Equals("PDF") ?? false){
                return true;
            }
            return false;
        }

      public new IActionResult View(object model)
        {
            if (IsPdfResponseView())
            {
                try{
                    var file = _pdfservice.PrintToPdf(model).ToArray();
                    string base64 = Convert.ToBase64String(file);
                    return Json(base64);


                    //  return File(_pdfservice.PrintToPdf(model).ToArray(), "application/pdf", "convert.pdf");

                }
                catch (Exception ex)
                {
                    throw new Exception("Error", ex);

                }
                
            }

            return base.View(model);
        }

        public new IActionResult View(string ViewName, object model)
        {
            if (IsPdfResponseView())
            {
                try
                {
                    var file = _pdfservice.PrintToPdf(model).ToArray();
                        string base64 = Convert.ToBase64String(file);
                        return Json(base64);



                  //  return File(_pdfservice.PrintToPdf(model).ToArray(), "application/pdf", "convert.pdf");

                }
                catch
                {

                }

            }

            return base.View(ViewName, model);
        }
    }
}
