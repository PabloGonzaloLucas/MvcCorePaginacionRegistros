using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    public class EmpleadosDepartamentoViewComponent : ViewComponent
    {
        private RepositoryHospital repo;
        public EmpleadosDepartamentoViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync(int iddept, int posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            Empleado emp = await this.repo.GetEmpleadosByDepartamentoAsync(iddept, posicion);
            return View(emp);
        }
    }
}
