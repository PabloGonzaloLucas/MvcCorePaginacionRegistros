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


            ViewBag.NumRegistros = numRegistros;
            ViewBag.Posicion = posicion;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();


            ViewBag.NumRegistros = numRegistros;
            ViewBag.Posicion = posicion;
            List<Departamento> departamentos = await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult>
            PaginarGrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetEmpleadosCountAsync();
            ViewBag.REGISTROS = numRegistros;
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosOficio(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, posicion.Value);
                int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
                ViewBag.REGISTROS = registros;
                ViewBag.OFICIO = oficio;
                return View(empleados);
            }

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(oficio, 1);
            int registros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewBag.REGISTROS = registros;
            ViewBag.OFICIO = oficio;
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, posicion.Value);
                ViewBag.REGISTROS = model.NumeroRegistros;
                ViewBag.OFICIO = oficio;
                return View(model.Empleados);
            }

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {
            ModelEmpleadosOficio model = await this.repo.GetGrupoEmpleadosOficioOutAsync(oficio, 1);
            ViewBag.REGISTROS = model.NumeroRegistros;
            ViewBag.OFICIO = oficio;
            return View(model.Empleados);
        }

        public async Task<IActionResult> DepartamentoDetails(int iddept, int? posicion)
        {
            Departamento dept = await this.repo.GetDepartamentoByIdAsync(iddept);

            if (posicion == null)
            {
                posicion = 1;
            }
            ViewBag.DEPT = dept;
            Empleado emp = await this.repo.GetEmpleadosByDepartamentoAsync(iddept, posicion.Value);
            ViewBag.SIGUIENTE = posicion + 1;
            ViewBag.ANTERIOR = posicion - 1;
            ViewBag.ULTIMO = emp.NumRegistros;

            return View(emp);
        }
        public async Task<IActionResult> _EmpleadosDepartamento(int iddept, int posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            Empleado emp = await this.repo.GetEmpleadosByDepartamentoAsync(iddept, posicion);
            return PartialView("_EmpleadosDepartamento", emp);

        }
    }
}
