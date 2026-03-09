using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

#region VISTAS
//ALTER VIEW V_DEPARTAMENTOS_INDIVIDUAL
//as
//	select cast(ROW_NUMBER() over (order by DEPT_NO) as int) as POSICION, DEPT_NO, DNOMBRE, LOC from DEPT
//go

//CREATE PROCEDURE SP_GRUPO_DEPARTAMENTOS
//(@posicion int)
//as
//	select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL
//	where POSICION >= @posicion and POSICION < (@posicion + 2)
//go
//exec SP_GRUPO_DEPARTAMENTOS 1

//create view V_GRUPO_EMPLEADOS
//as
//   select cast(row_number() over (order by APELLIDO) as int)
//   as POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
//   from EMP
//go

//create procedure SP_GRUPO_EMPLEADOS
//(@posicion int)
//as
//   select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
//   from V_GRUPO_EMPLEADOS
//   where POSICION >= @posicion and @posicion < (@posicion + 3)
//go

//ALTER procedure SP_GRUPO_EMPLEADOS_OFICIO
//(@posicion int, @oficio nvarchar(50),
//  @registros int out) as
//   select  @registros = count(EMP_NO) from EMP
//   where OFICIO = @oficio
//   select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from
//   (select cast(row_number() over(order by APELLIDO) as int) as POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
//   from V_GRUPO_EMPLEADOS
//   where OFICIO = @oficio) QUERY
//   where (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3))

#endregion
namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;
        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento departamento =
                await this.context.VistaDepartamentos.Where(v => v.Posicion == posicion).FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentoAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion
                           && datos.Posicion < (posicion + 2)
                           select datos;
            return await consulta.ToListAsync();
        }

        public async Task<List<Departamento>> GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int>
            GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(x => x.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>>
            GetGrupoEmpleadosOficioAsync(string oficio, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPos = new SqlParameter("posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("oficio", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamOficio);
            return await consulta.ToListAsync();
        }
        public async Task<ModelEmpleadosOficio>
            GetGrupoEmpleadosOficioOutAsync(string oficio, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio, @registros out";
            SqlParameter pamPos = new SqlParameter("posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("oficio", oficio);
            SqlParameter pamReg = new SqlParameter("registros", 0);
            pamReg.DbType = System.Data.DbType.Int32;
            pamReg.Direction = System.Data.ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamOficio, pamReg);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamReg.Value;

            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<Departamento> GetDepartamentoByIdAsync(int iddept)
        {
            return await this.context.Departamentos.FindAsync(iddept);
        }

        public async Task<Empleado> GetEmpleadosByDepartamentoAsync(int iddept, int posicion)
        {
            string sql = "SP_EMPLEADOS_DEPARTAMENTO @posicion, @iddept, @registros out";
            SqlParameter pamPos = new SqlParameter("posicion", posicion);
            SqlParameter pamDept = new SqlParameter("iddept", iddept);
            SqlParameter pamReg = new SqlParameter("registros", 0);
            pamReg.DbType = System.Data.DbType.Int32;
            pamReg.Direction = System.Data.ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPos, pamDept, pamReg);
            List<Empleado> empleados = await consulta.ToListAsync();
            Empleado empleado = empleados.FirstOrDefault();
            int registros = (int)pamReg.Value;
            empleado.NumRegistros = registros;
            return empleado;
        }
    }
}
