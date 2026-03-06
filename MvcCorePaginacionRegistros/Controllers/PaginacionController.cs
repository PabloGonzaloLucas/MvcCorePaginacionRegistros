using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;
        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewBag.ULTIMO = numRegistros;
            ViewBag.SIGUIENTE = siguiente;
            ViewBag.ANTERIOR = anterior;
            VistaDepartamento dept = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(dept);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            string html = "<div>";
            for (int i = 1; i <= numRegistros; i += 2)
            {
                html += "<a href='GrupoVistaDepartamentos?posicion=" + i + "'>Página " + numPagina + "</a>";
                numPagina += 1;
            }
            html += "</div>";
            ViewBag.LINKS = html;
            ViewBag.NumRegistros = numRegistros;
            ViewBag.Posicion = posicion;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);
        }
    }
}
