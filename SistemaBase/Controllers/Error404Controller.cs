using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaBase.Models;
//using SistemaBase.ModelsCustom;

namespace SistemaBase.Controllers
{
    [Authorize]
    public class Error404Controller : BaseRyMController
    {
        private readonly Models.UAADbContext _context;

        public Error404Controller(Models.UAADbContext context, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _context = context;
        }

        // GET: Modulo
        public Task<IActionResult> Index()
        {
         
            return Task.FromResult<IActionResult>(View());
        }
    }
}
