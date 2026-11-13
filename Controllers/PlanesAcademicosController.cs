using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Planes;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class PlanesAcademicosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlanesAcademicosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("planes-academicos")]
        public async Task<ActionResult<List<PlanItemDto>>> ListarPlanes([FromQuery] PlanFilterDto filtro)
        {
            var query = _context.PlanAcademicos
                .Include(p => p.IdCentroUniversitarioNavigation)
                .Include(p => p.IdCarreraNavigation)
                .Include(p => p.IdTipoPeriodoAcademicoNavigation)
                .Include(p => p.IdModalidadNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (filtro.IdDireccion.HasValue)
            {
                query = query.Where(p => p.IdCarreraNavigation.IdDireccion == filtro.IdDireccion.Value);
            }

            if (filtro.IdCarrera.HasValue)
            {
                query = query.Where(p => p.IdCarrera == filtro.IdCarrera.Value);
            }

            if (filtro.IdModalidad.HasValue)
            {
                query = query.Where(p => p.IdModalidad == filtro.IdModalidad.Value);
            }

            if (filtro.IdCentroUniversitario.HasValue)
            {
                query = query.Where(p => p.IdCentroUniversitario == filtro.IdCentroUniversitario.Value);
            }

            if (filtro.Activo.HasValue)
            {
                query = query.Where(p => p.Activo == filtro.Activo.Value);
            }

            var items = await query
                .OrderBy(p => p.Codigo)
                .Select(p => new PlanItemDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    IdCentroUniversitario = p.IdCentroUniversitario,
                    CentroNombre = p.IdCentroUniversitarioNavigation.Nombre,
                    IdCarrera = p.IdCarrera,
                    CarreraNombre = p.IdCarreraNavigation.Nombre,
                    IdTipoPeriodoAcademico = p.IdTipoPeriodoAcademico,
                    TipoPeriodoNombre = p.IdTipoPeriodoAcademicoNavigation.Nombre,
                    IdModalidad = p.IdModalidad,
                    ModalidadNombre = p.IdModalidadNavigation.Nombre,
                    CantidadCicloAcademico = p.CantidadCicloAcademico,
                    CantidadPeriodoAcademico = p.CantidadPeriodoAcademico,
                    ActivoDesde = p.ActivoDesde,
                    ActivoHasta = p.ActivoHasta,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("planes-academicos/{id:guid}")]
        public async Task<ActionResult<PlanDetalleDto>> ObtenerPlanPorId(Guid id)
        {
            var p = await _context.PlanAcademicos
                .AsNoTracking()
                .FirstOrDefaultAsync(plan => plan.Id == id);

            if (p == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = $"Plan académico con ID {id} no encontrado." });
            }

            return Ok(new PlanDetalleDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                IdCentroUniversitario = p.IdCentroUniversitario,
                IdCarrera = p.IdCarrera,
                IdTipoPeriodoAcademico = p.IdTipoPeriodoAcademico,
                IdModalidad = p.IdModalidad,
                CantidadCicloAcademico = p.CantidadCicloAcademico,
                CantidadPeriodoAcademico = p.CantidadPeriodoAcademico,
                ActivoDesde = p.ActivoDesde,
                ActivoHasta = p.ActivoHasta,
                Activo = p.Activo
            });
        }

        [HttpGet("planes-academicos/{id:guid}/malla-completa")]
        public async Task<ActionResult<MallaCompletaDto>> ObtenerMallaCompletaPlan(Guid id)
        {
            var plan = await _context.PlanAcademicos
                .Include(p => p.IdCarreraNavigation)
                .Include(p => p.IdModalidadNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
            {
                return NotFound(new { status = 404, error = "NOT_FOUND", message = $"Plan académico con ID {id} no encontrado." });
            }

            var detalles = await _context.DetallePlanAcademicos
                .Include(d => d.IdCicloAcademicoNavigation)
                .Include(d => d.IdPeriodoAcademicoNavigation)
                .Include(d => d.MallaCurriculars)
                    .ThenInclude(m => m.IdMateriaNavigation)
                .Include(d => d.MallaCurriculars)
                    .ThenInclude(m => m.MateriaPrerequisitoIdMallaCurricularMateriaNavigations)
                        .ThenInclude(mp => mp.IdMallaCurricularMateriaPrerequisitoNavigation)
                            .ThenInclude(mcp => mcp.IdMateriaNavigation)
                .Where(d => d.IdPlanAcademico == id && d.Activo)
                .OrderBy(d => d.IdCicloAcademicoNavigation.Orden)
                .ThenBy(d => d.IdPeriodoAcademicoNavigation.Orden)
                .AsNoTracking()
                .ToListAsync();

            var ciclosAgrupados = detalles
                .GroupBy(d => d.IdCicloAcademicoNavigation)
                .OrderBy(g => g.Key.Orden)
                .Select(g => new CicloMallaDto
                {
                    IdCicloAcademico = g.Key.Id,
                    CicloNombre = g.Key.Nombre,
                    OrdenCiclo = g.Key.Orden,
                    Bloques = g.Select(bloque => new BloqueMallaDto
                    {
                        IdDetallePlanAcademico = bloque.Id,
                        IdPeriodoAcademico = bloque.IdPeriodoAcademico,
                        PeriodoNombre = bloque.IdPeriodoAcademicoNavigation.Nombre,
                        OrdenPeriodo = bloque.IdPeriodoAcademicoNavigation.Orden,
                        DuracionSemanas = bloque.DuracionSemanas,
                        DuracionMeses = bloque.DuracionMeses,
                        IdDetallePredecesor = bloque.IdDetallePredecesor,
                        Materias = bloque.MallaCurriculars.Where(m => m.Activo).Select(m => new MateriaMallaDto
                        {
                            IdMallaCurricular = m.Id,
                            IdMateria = m.IdMateria,
                            MateriaCodigo = m.IdMateriaNavigation.Codigo,
                            MateriaNombre = m.IdMateriaNavigation.Nombre,
                            Creditos = m.Creditos,
                            CantidadHorasMateria = m.CantidadHorasMateria,
                            EncuentrosTotales = m.EncuentrosTotales,
                            EncuentrosPorSemana = m.EncuentrosPorSemana,
                            Obligatoria = m.Obligatoria,
                            Activo = m.Activo,
                            Prerequisitos = m.MateriaPrerequisitoIdMallaCurricularMateriaNavigations.Select(pr => new PrerequisitoMallaDto
                            {
                                Id = pr.Id,
                                IdMallaCurricularMateriaPrerequisito = pr.IdMallaCurricularMateriaPrerequisito,
                                MateriaCodigo = pr.IdMallaCurricularMateriaPrerequisitoNavigation.IdMateriaNavigation.Codigo,
                                MateriaNombre = pr.IdMallaCurricularMateriaPrerequisitoNavigation.IdMateriaNavigation.Nombre
                            }).ToList()
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            return Ok(new MallaCompletaDto
            {
                Plan = new PlanResumenMallaDto
                {
                    Id = plan.Id,
                    Codigo = plan.Codigo,
                    Nombre = plan.Nombre,
                    CarreraNombre = plan.IdCarreraNavigation.Nombre,
                    ModalidadNombre = plan.IdModalidadNavigation.Nombre
                },
                Ciclos = ciclosAgrupados
            });
        }

        [HttpPost("planes-academicos")]
        public async Task<ActionResult<PlanDetalleDto>> CrearPlanAcademico([FromBody] CreatePlanAcademicoDto dto)
        {
            var exists = await _context.PlanAcademicos.AnyAsync(p => p.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe un plan académico con este código." });
            }

            var plan = new PlanAcademico
            {
                Id = Guid.NewGuid(),
                IdCentroUniversitario = dto.IdCentroUniversitario,
                IdCarrera = dto.IdCarrera,
                IdTipoPeriodoAcademico = dto.IdTipoPeriodoAcademico,
                IdModalidad = dto.IdModalidad,
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                CantidadCicloAcademico = dto.CantidadCicloAcademico,
                CantidadPeriodoAcademico = dto.CantidadPeriodoAcademico,
                ActivoDesde = dto.ActivoDesde,
                ActivoHasta = dto.ActivoHasta,
                Activo = dto.Activo
            };

            _context.PlanAcademicos.Add(plan);
            await _context.SaveChangesAsync();

            return StatusCode(201, new PlanDetalleDto
            {
                Id = plan.Id,
                Codigo = plan.Codigo,
                Nombre = plan.Nombre,
                IdCentroUniversitario = plan.IdCentroUniversitario,
                IdCarrera = plan.IdCarrera,
                IdTipoPeriodoAcademico = plan.IdTipoPeriodoAcademico,
                IdModalidad = plan.IdModalidad,
                CantidadCicloAcademico = plan.CantidadCicloAcademico,
                CantidadPeriodoAcademico = plan.CantidadPeriodoAcademico,
                ActivoDesde = plan.ActivoDesde,
                ActivoHasta = plan.ActivoHasta,
                Activo = plan.Activo
            });
        }
    }
}
