using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Asistencia;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class AsistenciasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsistenciasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("calendarios/{idCalendario:guid}/acta-asistencia")]
        public async Task<ActionResult<ActaAsistenciaSesionDto>> ObtenerActaAsistenciaSesion(Guid idCalendario)
        {
            var sesion = await _context.Calendarios
                .Include(c => c.IdAulaNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdGrupoNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdMallaCurricularNavigation)
                        .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(c => c.IdMateriaPorGrupoNavigation)
                    .ThenInclude(mpg => mpg.IdDocenteNavigation)
                        .ThenInclude(d => d.IdUsuarioNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == idCalendario);

            if (sesion == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Sesión de calendario con ID {idCalendario} no encontrada."
                });
            }

            var mpg = sesion.IdMateriaPorGrupoNavigation;
            var titular = mpg.IdDocenteNavigation.IdUsuarioNavigation;

            var cabecera = await _context.Asistencia
                .Include(a => a.IdDocenteSustitutoNavigation)
                    .ThenInclude(ds => ds!.IdUsuarioNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdCalendario == idCalendario);

            var inscripciones = await _context.InscripcionGrupos
                .Include(ig => ig.IdEstudianteNavigation)
                .Where(ig => ig.IdMateriaPorGrupo == mpg.Id && ig.Activo)
                .OrderBy(ig => ig.IdEstudianteNavigation.PrimerApellido)
                .ThenBy(ig => ig.IdEstudianteNavigation.Nombres)
                .AsNoTracking()
                .ToListAsync();

            var detallesExistentes = cabecera != null
                ? await _context.AsistenciaDetalles
                    .Where(ad => ad.IdAsistencia == cabecera.Id)
                    .AsNoTracking()
                    .ToDictionaryAsync(ad => ad.IdInscripcionGrupo)
                : new Dictionary<Guid, AsistenciaDetalle>();

            var estudiantesDto = inscripciones.Select(ig =>
            {
                detallesExistentes.TryGetValue(ig.Id, out var det);
                var est = ig.IdEstudianteNavigation;
                return new EstudianteActaItemDto
                {
                    IdAsistenciaDetalle = det?.Id,
                    IdInscripcionGrupo = ig.Id,
                    IdEstudiante = est.Id,
                    CodigoUnico = est.CodigoUnico,
                    Carnet = est.Carnet,
                    NombreCompleto = $"{est.Nombres} {est.PrimerApellido} {est.SegundoApellido}".Trim(),
                    Asistio = det?.Asistio ?? true,
                    IdMotivoAusencia = det?.IdMotivoAusencia,
                    FechaRegistroEstudiante = det?.FechaRegistroEstudiante
                };
            }).ToList();

            var response = new ActaAsistenciaSesionDto
            {
                Sesion = new SesionDetalleActaDto
                {
                    IdCalendario = sesion.Id,
                    IdMateriaPorGrupo = mpg.Id,
                    MateriaCodigo = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    GrupoCodigo = mpg.IdGrupoNavigation.Codigo,
                    AulaCodigo = sesion.IdAulaNavigation.Codigo,
                    DocenteTitularNombre = $"{titular.Nombre} {titular.Apellido}",
                    FechaInicio = sesion.FechaInicio,
                    FechaFin = sesion.FechaFin
                },
                CabeceraAsistencia = new CabeceraAsistenciaActaDto
                {
                    Id = cabecera?.Id,
                    DocenteAsignadoAsistio = cabecera?.DocenteAsignadoAsistio,
                    IdDocenteSustituto = cabecera?.IdDocenteSustituto,
                    DocenteSustitutoNombre = cabecera?.IdDocenteSustitutoNavigation != null
                        ? $"{cabecera.IdDocenteSustitutoNavigation.IdUsuarioNavigation.Nombre} {cabecera.IdDocenteSustitutoNavigation.IdUsuarioNavigation.Apellido}"
                        : null,
                    IdMotivoAusencia = cabecera?.IdMotivoAusencia,
                    FechaRegistroDocente = cabecera?.FechaRegistroDocente
                },
                Estudiantes = estudiantesDto
            };

            return Ok(response);
        }

        [HttpPut("calendarios/{idCalendario:guid}/acta-asistencia")]
        public async Task<ActionResult<ResultadoGuardarActaDto>> GuardarActaAsistenciaSesion(Guid idCalendario, [FromBody] GuardarActaAsistenciaDto dto)
        {
            var sesion = await _context.Calendarios.FindAsync(idCalendario);
            if (sesion == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Sesión con ID {idCalendario} no encontrada."
                });
            }

            if (!dto.DocenteAsignadoAsistio && !dto.IdMotivoAusencia.HasValue)
            {
                return BadRequest(new
                {
                    status = 400,
                    error = "VALIDATION_ERROR",
                    message = "Debe especificar el motivo de ausencia del docente titular."
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cabecera = await _context.Asistencia.FirstOrDefaultAsync(a => a.IdCalendario == idCalendario);
                var ahora = DateTime.UtcNow;

                if (cabecera == null)
                {
                    cabecera = new Asistencium
                    {
                        Id = Guid.NewGuid(),
                        IdCalendario = idCalendario,
                        DocenteAsignadoAsistio = dto.DocenteAsignadoAsistio,
                        IdDocenteSustituto = dto.IdDocenteSustituto,
                        IdMotivoAusencia = dto.IdMotivoAusencia,
                        FechaRegistroDocente = ahora
                    };
                    _context.Asistencia.Add(cabecera);
                }
                else
                {
                    cabecera.DocenteAsignadoAsistio = dto.DocenteAsignadoAsistio;
                    cabecera.IdDocenteSustituto = dto.IdDocenteSustituto;
                    cabecera.IdMotivoAusencia = dto.IdMotivoAusencia;
                    cabecera.FechaRegistroDocente = ahora;
                }

                await _context.SaveChangesAsync();

                var detallesExistentes = await _context.AsistenciaDetalles
                    .Where(ad => ad.IdAsistencia == cabecera.Id)
                    .ToListAsync();

                var detallesDict = detallesExistentes.ToDictionary(ad => ad.IdInscripcionGrupo);

                int presentes = 0;
                int ausentes = 0;
                int justificados = 0;

                foreach (var item in dto.Detalles)
                {
                    if (item.Asistio)
                    {
                        presentes++;
                    }
                    else
                    {
                        if (item.IdMotivoAusencia.HasValue)
                        {
                            justificados++;
                        }
                        else
                        {
                            ausentes++;
                        }
                    }

                    if (detallesDict.TryGetValue(item.IdInscripcionGrupo, out var detalleExistente))
                    {
                        detalleExistente.Asistio = item.Asistio;
                        detalleExistente.IdMotivoAusencia = item.IdMotivoAusencia;
                        detalleExistente.FechaRegistroEstudiante = ahora;
                    }
                    else
                    {
                        var nuevoDetalle = new AsistenciaDetalle
                        {
                            Id = Guid.NewGuid(),
                            IdAsistencia = cabecera.Id,
                            IdInscripcionGrupo = item.IdInscripcionGrupo,
                            Asistio = item.Asistio,
                            IdMotivoAusencia = item.IdMotivoAusencia,
                            FechaRegistroEstudiante = ahora
                        };
                        _context.AsistenciaDetalles.Add(nuevoDetalle);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new ResultadoGuardarActaDto
                {
                    IdAsistencia = cabecera.Id,
                    IdCalendario = idCalendario,
                    DocenteAsignadoAsistio = cabecera.DocenteAsignadoAsistio ?? true,
                    IdDocenteSustituto = cabecera.IdDocenteSustituto,
                    IdMotivoAusencia = cabecera.IdMotivoAusencia,
                    FechaRegistroDocente = cabecera.FechaRegistroDocente ?? ahora,
                    Resumen = new ResumenGuardarActaDto
                    {
                        Presentes = presentes,
                        Ausentes = ausentes,
                        Justificados = justificados
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    status = 500,
                    error = "INTERNAL_SERVER_ERROR",
                    message = "Ocurrió un error al guardar el acta de asistencia.",
                    details = ex.Message
                });
            }
        }

        [HttpPatch("asistencias-detalle/{id:guid}")]
        public async Task<IActionResult> ActualizarAsistenciaEstudianteIndividual(Guid id, [FromBody] UpdateAsistenciaDetalleDto dto)
        {
            var detalle = await _context.AsistenciaDetalles.FindAsync(id);
            if (detalle == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Detalle de asistencia con ID {id} no encontrado."
                });
            }

            detalle.Asistio = dto.Asistio;
            detalle.IdMotivoAusencia = dto.IdMotivoAusencia;
            detalle.FechaRegistroEstudiante = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = detalle.Id,
                id_asistencia = detalle.IdAsistencia,
                id_inscripcion_grupo = detalle.IdInscripcionGrupo,
                asistio = detalle.Asistio,
                id_motivo_ausencia = detalle.IdMotivoAusencia,
                fecha_registro_estudiante = detalle.FechaRegistroEstudiante
            });
        }

        [HttpDelete("asistencias/{id:guid}")]
        public async Task<IActionResult> EliminarActaAsistencia(Guid id)
        {
            var acta = await _context.Asistencia.FindAsync(id);
            if (acta == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Acta de asistencia con ID {id} no encontrada."
                });
            }

            _context.Asistencia.Remove(acta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("materias-por-grupo/{id:guid}/sabana-asistencia")]
        public async Task<ActionResult<SabanaAsistenciaDto>> ObtenerSabanaAsistenciaGrupo(Guid id)
        {
            var mpg = await _context.MateriaPorGrupos
                .Include(m => m.IdGrupoNavigation)
                    .ThenInclude(g => g.IdInstanciaAcademicaNavigation)
                .Include(m => m.IdMallaCurricularNavigation)
                    .ThenInclude(mc => mc.IdMateriaNavigation)
                .Include(m => m.IdDocenteNavigation)
                    .ThenInclude(d => d.IdUsuarioNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mpg == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "NOT_FOUND",
                    message = $"Materia por grupo con ID {id} no encontrada."
                });
            }

            var sesiones = await _context.Calendarios
                .Include(c => c.Asistencium)
                .Where(c => c.IdMateriaPorGrupo == id && c.Activo)
                .OrderBy(c => c.FechaInicio)
                .AsNoTracking()
                .ToListAsync();

            var inscripciones = await _context.InscripcionGrupos
                .Include(ig => ig.IdEstudianteNavigation)
                .Where(ig => ig.IdMateriaPorGrupo == id && ig.Activo)
                .OrderBy(ig => ig.IdEstudianteNavigation.PrimerApellido)
                .ThenBy(ig => ig.IdEstudianteNavigation.Nombres)
                .AsNoTracking()
                .ToListAsync();

            var idsActas = sesiones.Where(s => s.Asistencium != null).Select(s => s.Asistencium!.Id).ToList();

            var detalles = await _context.AsistenciaDetalles
                .Include(ad => ad.IdMotivoAusenciaNavigation)
                .Where(ad => idsActas.Contains(ad.IdAsistencia))
                .AsNoTracking()
                .ToListAsync();

            var actasPorCalendario = sesiones
                .Where(s => s.Asistencium != null)
                .ToDictionary(s => s.Asistencium!.Id, s => s.Id);

            var detallesLookup = new Dictionary<(Guid IdInscripcion, Guid IdCalendario), AsistenciaDetalle>();
            foreach (var det in detalles)
            {
                if (actasPorCalendario.TryGetValue(det.IdAsistencia, out var calId))
                {
                    detallesLookup[(det.IdInscripcionGrupo, calId)] = det;
                }
            }

            var columnas = sesiones.Select((s, index) => new SesionColumnaDto
            {
                IdCalendario = s.Id,
                NumeroEncuentro = index + 1,
                FechaInicio = s.FechaInicio,
                ActaRegistrada = s.Asistencium != null
            }).ToList();

            var filas = inscripciones.Select(ig =>
            {
                var est = ig.IdEstudianteNavigation;
                int presentes = 0;
                int ausentesInjustificados = 0;
                int justificados = 0;

                var marcajes = sesiones.Select(s =>
                {
                    if (s.Asistencium == null)
                    {
                        return new MarcajeDto
                        {
                            IdCalendario = s.Id,
                            Estado = "PENDIENTE",
                            MotivoAusenciaNombre = null,
                            FechaRegistroEstudiante = null
                        };
                    }

                    detallesLookup.TryGetValue((ig.Id, s.Id), out var d);
                    if (d == null)
                    {
                        return new MarcajeDto
                        {
                            IdCalendario = s.Id,
                            Estado = "PENDIENTE",
                            MotivoAusenciaNombre = null,
                            FechaRegistroEstudiante = null
                        };
                    }

                    if (d.Asistio)
                    {
                        presentes++;
                        return new MarcajeDto
                        {
                            IdCalendario = s.Id,
                            Estado = "P",
                            MotivoAusenciaNombre = null,
                            FechaRegistroEstudiante = d.FechaRegistroEstudiante
                        };
                    }

                    if (d.IdMotivoAusencia.HasValue)
                    {
                        justificados++;
                        return new MarcajeDto
                        {
                            IdCalendario = s.Id,
                            Estado = "J",
                            MotivoAusenciaNombre = d.IdMotivoAusenciaNavigation?.Nombre,
                            FechaRegistroEstudiante = d.FechaRegistroEstudiante
                        };
                    }

                    ausentesInjustificados++;
                    return new MarcajeDto
                    {
                        IdCalendario = s.Id,
                        Estado = "A",
                        MotivoAusenciaNombre = null,
                        FechaRegistroEstudiante = d.FechaRegistroEstudiante
                    };
                }).ToList();

                var totalEvaluados = presentes + ausentesInjustificados + justificados;
                decimal porcentaje = totalEvaluados > 0
                    ? Math.Round(((decimal)presentes / totalEvaluados) * 100, 2)
                    : 0m;

                string alerta = porcentaje >= 90m ? "VERDE" : (porcentaje >= 75m ? "AMARILLO" : "ROJO");

                return new AlumnoFilaDto
                {
                    IdInscripcionGrupo = ig.Id,
                    IdEstudiante = est.Id,
                    CodigoUnico = est.CodigoUnico,
                    Carnet = est.Carnet,
                    NombreCompleto = $"{est.Nombres} {est.PrimerApellido} {est.SegundoApellido}".Trim(),
                    Marcajes = marcajes,
                    Estadisticas = new EstadisticasAlumnoDto
                    {
                        TotalPresentes = presentes,
                        TotalAusentesInjustificados = ausentesInjustificados,
                        TotalJustificados = justificados,
                        PorcentajeAsistencia = porcentaje,
                        AlertaRiesgo = alerta
                    }
                };
            }).ToList();

            var response = new SabanaAsistenciaDto
            {
                Curso = new CursoSabanaDto
                {
                    IdMateriaPorGrupo = mpg.Id,
                    MateriaCodigo = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Codigo,
                    MateriaNombre = mpg.IdMallaCurricularNavigation.IdMateriaNavigation.Nombre,
                    GrupoCodigo = mpg.IdGrupoNavigation.Codigo,
                    DocenteNombre = $"{mpg.IdDocenteNavigation.IdUsuarioNavigation.Nombre} {mpg.IdDocenteNavigation.IdUsuarioNavigation.Apellido}",
                    InstanciaNombre = mpg.IdGrupoNavigation.IdInstanciaAcademicaNavigation.Nombre
                },
                SesionesColumnas = columnas,
                AlumnosFilas = filas
            };

            return Ok(response);
        }
    }
}
