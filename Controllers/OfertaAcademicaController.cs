using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Calendario;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class OfertaAcademicaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OfertaAcademicaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("grupos")]
        public async Task<ActionResult<List<GrupoDto>>> ListarGrupos([FromQuery] Guid? idInstanciaAcademica, [FromQuery] bool? activo)
        {
            var query = _context.Grupos
                .Include(g => g.IdInstanciaAcademicaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (idInstanciaAcademica.HasValue)
            {
                query = query.Where(g => g.IdInstanciaAcademica == idInstanciaAcademica.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(g => g.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(g => g.Codigo)
                .Select(g => new GrupoDto
                {
                    Id = g.Id,
                    IdInstanciaAcademica = g.IdInstanciaAcademica,
                    InstanciaNombre = g.IdInstanciaAcademicaNavigation.Nombre,
                    Codigo = g.Codigo,
                    Nombre = g.Nombre,
                    Cupo = g.Cupo,
                    Activo = g.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("grupos")]
        public async Task<ActionResult<GrupoDto>> CrearGrupo([FromBody] CreateGrupoDto dto)
        {
            var instancia = await _context.InstanciaAcademicas.FindAsync(dto.IdInstanciaAcademica);
            if (instancia == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La instancia académica especificada no existe." });
            }

            var exists = await _context.Grupos.AnyAsync(g => g.IdInstanciaAcademica == dto.IdInstanciaAcademica && g.Codigo == dto.Codigo.Trim());
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Ya existe un grupo con este código en la instancia académica seleccionada." });
            }

            var grupo = new Grupo
            {
                Id = Guid.NewGuid(),
                IdInstanciaAcademica = dto.IdInstanciaAcademica,
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Cupo = dto.Cupo,
                Activo = dto.Activo
            };

            _context.Grupos.Add(grupo);
            await _context.SaveChangesAsync();

            return StatusCode(201, new GrupoDto
            {
                Id = grupo.Id,
                IdInstanciaAcademica = grupo.IdInstanciaAcademica,
                InstanciaNombre = instancia.Nombre,
                Codigo = grupo.Codigo,
                Nombre = grupo.Nombre,
                Cupo = grupo.Cupo,
                Activo = grupo.Activo
            });
        }

        [HttpGet("materias-por-grupo")]
        public async Task<ActionResult<List<MateriaPorGrupoDto>>> ListarMateriasPorGrupo([FromQuery] Guid? idGrupo, [FromQuery] Guid? idDocente, [FromQuery] bool? activo)
        {
            var query = _context.MateriaPorGrupos
                .Include(m => m.IdGrupoNavigation)
                .Include(m => m.IdMallaCurricularNavigation)
                    .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(m => m.IdDocenteNavigation)
                    .ThenInclude(d => d.IdUsuarioNavigation)
                .Include(m => m.InscripcionGrupos)
                .AsNoTracking()
                .AsQueryable();

            if (idGrupo.HasValue)
            {
                query = query.Where(m => m.IdGrupo == idGrupo.Value);
            }

            if (idDocente.HasValue)
            {
                query = query.Where(m => m.IdDocente == idDocente.Value);
            }

            if (activo.HasValue)
            {
                query = query.Where(m => m.Activo == activo.Value);
            }

            var items = await query
                .OrderBy(m => m.IdGrupoNavigation.Codigo)
                .Select(m => new MateriaPorGrupoDto
                {
                    Id = m.Id,
                    IdGrupo = m.IdGrupo,
                    GrupoCodigo = m.IdGrupoNavigation.Codigo,
                    IdMallaCurricular = m.IdMallaCurricular,
                    IdMateria = m.IdMallaCurricularNavigation.IdMateria,
                    MateriaCodigo = m.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = m.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    IdDocente = m.IdDocente,
                    DocenteNombreCompleto = $"{m.IdDocenteNavigation.IdUsuarioNavigation.Nombre} {m.IdDocenteNavigation.IdUsuarioNavigation.Apellido}",
                    CupoLimite = m.CupoLimite,
                    TotalInscritos = m.InscripcionGrupos.Count(ig => ig.Activo),
                    Activo = m.Activo
                })
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost("materias-por-grupo")]
        public async Task<ActionResult<MateriaPorGrupoDto>> CrearMateriaPorGrupo([FromBody] CreateMateriaPorGrupoDto dto)
        {
            var grupo = await _context.Grupos.FindAsync(dto.IdGrupo);
            if (grupo == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El grupo especificado no existe." });
            }

            var malla = await _context.MallaCurriculars
                .Include(m => m.IdMateriaNavigation)
                .FirstOrDefaultAsync(m => m.Id == dto.IdMallaCurricular);

            if (malla == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "La materia en la malla curricular especificada no existe." });
            }

            var docente = await _context.Docentes
                .Include(d => d.IdUsuarioNavigation)
                .FirstOrDefaultAsync(d => d.Id == dto.IdDocente);

            if (docente == null)
            {
                return BadRequest(new { status = 400, error = "BAD_REQUEST", message = "El docente especificado no existe." });
            }

            var exists = await _context.MateriaPorGrupos.AnyAsync(m => m.IdGrupo == dto.IdGrupo && m.IdMallaCurricular == dto.IdMallaCurricular);
            if (exists)
            {
                return Conflict(new { status = 409, error = "CONFLICT", message = "Esta materia ya está asignada a este grupo." });
            }

            var mpg = new MateriaPorGrupo
            {
                Id = Guid.NewGuid(),
                IdGrupo = dto.IdGrupo,
                IdMallaCurricular = dto.IdMallaCurricular,
                IdDocente = dto.IdDocente,
                CupoLimite = dto.CupoLimite,
                Activo = dto.Activo
            };

            _context.MateriaPorGrupos.Add(mpg);
            await _context.SaveChangesAsync();

            return StatusCode(201, new MateriaPorGrupoDto
            {
                Id = mpg.Id,
                IdGrupo = mpg.IdGrupo,
                GrupoCodigo = grupo.Codigo,
                IdMallaCurricular = mpg.IdMallaCurricular,
                IdMateria = malla.IdMateria,
                MateriaCodigo = malla.IdMateriaNavigation.Codigo,
                MateriaNombre = malla.IdMateriaNavigation.Nombre,
                IdDocente = mpg.IdDocente,
                DocenteNombreCompleto = $"{docente.IdUsuarioNavigation.Nombre} {docente.IdUsuarioNavigation.Apellido}",
                CupoLimite = mpg.CupoLimite,
                TotalInscritos = 0,
                Activo = mpg.Activo
            });
        }
    }
}
