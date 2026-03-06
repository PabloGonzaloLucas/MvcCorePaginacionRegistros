using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Models;

namespace MvcCorePaginacionRegistros.Data
{
    public class HospitalContext : DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> opt) : base(opt)
        {
        }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<VistaDepartamento> VistaDepartamentos { get; set; }
    }
}
