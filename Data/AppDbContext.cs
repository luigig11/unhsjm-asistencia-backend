using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using unhsjm_asistencia_backend.Models;

namespace unhsjm_asistencia_backend.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsistenciaDetalle> AsistenciaDetalles { get; set; }

    public virtual DbSet<Asistencium> Asistencia { get; set; }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<Calendario> Calendarios { get; set; }

    public virtual DbSet<CalendarioSemanal> CalendarioSemanals { get; set; }

    public virtual DbSet<Carrera> Carreras { get; set; }

    public virtual DbSet<CentroUniversitario> CentroUniversitarios { get; set; }

    public virtual DbSet<CicloAcademico> CicloAcademicos { get; set; }

    public virtual DbSet<DetallePlanAcademico> DetallePlanAcademicos { get; set; }

    public virtual DbSet<Direccion> Direccions { get; set; }

    public virtual DbSet<Dium> Dia { get; set; }

    public virtual DbSet<Docente> Docentes { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<Feriado> Feriados { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<InscripcionGrupo> InscripcionGrupos { get; set; }

    public virtual DbSet<InstanciaAcademica> InstanciaAcademicas { get; set; }

    public virtual DbSet<MallaCurricular> MallaCurriculars { get; set; }

    public virtual DbSet<MateriaPorGrupo> MateriaPorGrupos { get; set; }

    public virtual DbSet<MateriaPrerequisito> MateriaPrerequisitos { get; set; }

    public virtual DbSet<Materium> Materia { get; set; }

    public virtual DbSet<Modalidad> Modalidads { get; set; }

    public virtual DbSet<MotivoAusencium> MotivoAusencia { get; set; }

    public virtual DbSet<PeriodoAcademico> PeriodoAcademicos { get; set; }

    public virtual DbSet<PeriodoEscolar> PeriodoEscolars { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<PlanAcademico> PlanAcademicos { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoAula> TipoAulas { get; set; }

    public virtual DbSet<TipoEvento> TipoEventos { get; set; }

    public virtual DbSet<TipoPeriodoAcademico> TipoPeriodoAcademicos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<AuthUser> AuthUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone", "recovery_code" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in", "like", "ilike", "is", "match", "imatch", "isdistinct" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresEnum("tipo_contratacion_docente", new[] { "FIJO", "TEMPORAL" })
            .HasPostgresEnum("tipo_genero", new[] { "F", "M" })
            .HasPostgresEnum("tipo_operacion", new[] { "insert", "select", "update", "delete" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<AsistenciaDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asistencia_detalle_pkey");

            entity.ToTable("asistencia_detalle");

            entity.HasIndex(e => e.IdAsistencia, "idx_asistencia_detalle_asistencia");

            entity.HasIndex(e => e.IdInscripcionGrupo, "idx_asistencia_detalle_inscripcion");

            entity.HasIndex(e => e.IdMotivoAusencia, "idx_asistencia_detalle_motivo");

            entity.HasIndex(e => new { e.IdAsistencia, e.IdInscripcionGrupo }, "uq_asistencia_detalle").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Asistio).HasColumnName("asistio");
            entity.Property(e => e.FechaRegistroEstudiante)
                .HasDefaultValueSql("now()")
                .HasColumnName("fecha_registro_estudiante");
            entity.Property(e => e.IdAsistencia).HasColumnName("id_asistencia");
            entity.Property(e => e.IdInscripcionGrupo).HasColumnName("id_inscripcion_grupo");
            entity.Property(e => e.IdMotivoAusencia).HasColumnName("id_motivo_ausencia");

            entity.HasOne(d => d.IdAsistenciaNavigation).WithMany(p => p.AsistenciaDetalles)
                .HasForeignKey(d => d.IdAsistencia)
                .HasConstraintName("fk_asistencia_detalle_asistencia");

            entity.HasOne(d => d.IdInscripcionGrupoNavigation).WithMany(p => p.AsistenciaDetalles)
                .HasForeignKey(d => d.IdInscripcionGrupo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asistencia_detalle_inscripcion");

            entity.HasOne(d => d.IdMotivoAusenciaNavigation).WithMany(p => p.AsistenciaDetalles)
                .HasForeignKey(d => d.IdMotivoAusencia)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asistencia_detalle_motivo");
        });

        modelBuilder.Entity<Asistencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("asistencia_pkey");

            entity.ToTable("asistencia");

            entity.HasIndex(e => e.IdCalendario, "idx_asistencia_calendario");

            entity.HasIndex(e => e.IdDocenteSustituto, "idx_asistencia_docente_sustituto");

            entity.HasIndex(e => e.IdMotivoAusencia, "idx_asistencia_motivo_ausencia");

            entity.HasIndex(e => e.IdCalendario, "uq_asistencia_calendario").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.DocenteAsignadoAsistio).HasColumnName("docente_asignado_asistio");
            entity.Property(e => e.FechaRegistroDocente).HasColumnName("fecha_registro_docente");
            entity.Property(e => e.IdCalendario).HasColumnName("id_calendario");
            entity.Property(e => e.IdDocenteSustituto).HasColumnName("id_docente_sustituto");
            entity.Property(e => e.IdMotivoAusencia).HasColumnName("id_motivo_ausencia");

            entity.HasOne(d => d.IdCalendarioNavigation).WithOne(p => p.Asistencium)
                .HasForeignKey<Asistencium>(d => d.IdCalendario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asistencia_calendario");

            entity.HasOne(d => d.IdDocenteSustitutoNavigation).WithMany(p => p.Asistencia)
                .HasForeignKey(d => d.IdDocenteSustituto)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asistencia_docente_sustituto");

            entity.HasOne(d => d.IdMotivoAusenciaNavigation).WithMany(p => p.Asistencia)
                .HasForeignKey(d => d.IdMotivoAusencia)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asistencia_motivo_ausencia");
        });

        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aula_pkey");

            entity.ToTable("aula");

            entity.HasIndex(e => e.IdTipoAula, "idx_aula_tipo_aula");

            entity.HasIndex(e => e.Codigo, "uq_aula_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Capacidad)
                .HasDefaultValue((short)40)
                .HasColumnName("capacidad");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.IdTipoAula).HasColumnName("id_tipo_aula");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(150)
                .HasColumnName("ubicacion");

            entity.HasOne(d => d.IdTipoAulaNavigation).WithMany(p => p.Aulas)
                .HasForeignKey(d => d.IdTipoAula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_aula_tipo_aula");
        });

        modelBuilder.Entity<Calendario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("calendario_pkey");

            entity.ToTable("calendario");

            entity.HasIndex(e => e.IdAula, "idx_calendario_aula");

            entity.HasIndex(e => e.IdMateriaPorGrupo, "idx_calendario_materia_grupo");

            entity.HasIndex(e => new { e.IdMateriaPorGrupo, e.FechaInicio }, "uq_calendario_sesion").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdAula).HasColumnName("id_aula");
            entity.Property(e => e.IdMateriaPorGrupo).HasColumnName("id_materia_por_grupo");

            entity.HasOne(d => d.IdAulaNavigation).WithMany(p => p.Calendarios)
                .HasForeignKey(d => d.IdAula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_calendario_aula");

            entity.HasOne(d => d.IdMateriaPorGrupoNavigation).WithMany(p => p.Calendarios)
                .HasForeignKey(d => d.IdMateriaPorGrupo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_calendario_materia_grupo");
        });

        modelBuilder.Entity<CalendarioSemanal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("calendario_semanal_pkey");

            entity.ToTable("calendario_semanal");

            entity.HasIndex(e => e.IdAula, "idx_calendario_semanal_aula");

            entity.HasIndex(e => e.IdDia, "idx_calendario_semanal_dia");

            entity.HasIndex(e => e.IdMateriaPorGrupo, "idx_calendario_semanal_materia_grupo");

            entity.HasIndex(e => new { e.IdMateriaPorGrupo, e.IdDia, e.HoraInicio }, "uq_calendario_semanal").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.HoraFin).HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio).HasColumnName("hora_inicio");
            entity.Property(e => e.IdAula).HasColumnName("id_aula");
            entity.Property(e => e.IdDia).HasColumnName("id_dia");
            entity.Property(e => e.IdMateriaPorGrupo).HasColumnName("id_materia_por_grupo");

            entity.HasOne(d => d.IdAulaNavigation).WithMany(p => p.CalendarioSemanals)
                .HasForeignKey(d => d.IdAula)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_calendario_semanal_aula");

            entity.HasOne(d => d.IdDiaNavigation).WithMany(p => p.CalendarioSemanals)
                .HasForeignKey(d => d.IdDia)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_calendario_semanal_dia");

            entity.HasOne(d => d.IdMateriaPorGrupoNavigation).WithMany(p => p.CalendarioSemanals)
                .HasForeignKey(d => d.IdMateriaPorGrupo)
                .HasConstraintName("fk_calendario_semanal_materia_grupo");
        });

        modelBuilder.Entity<Carrera>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carrera_pkey");

            entity.ToTable("carrera");

            entity.HasIndex(e => e.IdDireccion, "idx_carrera_id_direccion");

            entity.HasIndex(e => e.Codigo, "uq_carrera_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_carrera_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.IdDireccion).HasColumnName("id_direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Carreras)
                .HasForeignKey(d => d.IdDireccion)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_carrera_direccion");
        });

        modelBuilder.Entity<CentroUniversitario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("centro_universitario_pkey");

            entity.ToTable("centro_universitario");

            entity.HasIndex(e => e.Codigo, "uq_centro_universitario_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_centro_universitario_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(200)
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<CicloAcademico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ciclo_academico_pkey");

            entity.ToTable("ciclo_academico");

            entity.HasIndex(e => e.Codigo, "uq_ciclo_academico_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_ciclo_academico_nombre").IsUnique();

            entity.HasIndex(e => e.Orden, "uq_ciclo_academico_orden").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Orden).HasColumnName("orden");
        });

        modelBuilder.Entity<DetallePlanAcademico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_plan_academico_pkey");

            entity.ToTable("detalle_plan_academico");

            entity.HasIndex(e => e.IdCicloAcademico, "idx_detalle_plan_ciclo");

            entity.HasIndex(e => e.IdPeriodoAcademico, "idx_detalle_plan_periodo");

            entity.HasIndex(e => e.IdPlanAcademico, "idx_detalle_plan_plan");

            entity.HasIndex(e => e.IdDetallePredecesor, "idx_detalle_plan_predecesor");

            entity.HasIndex(e => new { e.IdPlanAcademico, e.IdCicloAcademico, e.IdPeriodoAcademico }, "uq_detalle_plan_bloque").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.DuracionMeses).HasColumnName("duracion_meses");
            entity.Property(e => e.DuracionSemanas).HasColumnName("duracion_semanas");
            entity.Property(e => e.IdCicloAcademico).HasColumnName("id_ciclo_academico");
            entity.Property(e => e.IdDetallePredecesor).HasColumnName("id_detalle_predecesor");
            entity.Property(e => e.IdPeriodoAcademico).HasColumnName("id_periodo_academico");
            entity.Property(e => e.IdPlanAcademico).HasColumnName("id_plan_academico");

            entity.HasOne(d => d.IdCicloAcademicoNavigation).WithMany(p => p.DetallePlanAcademicos)
                .HasForeignKey(d => d.IdCicloAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_plan_ciclo");

            entity.HasOne(d => d.IdDetallePredecesorNavigation).WithMany(p => p.InverseIdDetallePredecesorNavigation)
                .HasForeignKey(d => d.IdDetallePredecesor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_detalle_plan_predecesor");

            entity.HasOne(d => d.IdPeriodoAcademicoNavigation).WithMany(p => p.DetallePlanAcademicos)
                .HasForeignKey(d => d.IdPeriodoAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_plan_periodo");

            entity.HasOne(d => d.IdPlanAcademicoNavigation).WithMany(p => p.DetallePlanAcademicos)
                .HasForeignKey(d => d.IdPlanAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalle_plan_plan");
        });

        modelBuilder.Entity<Direccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direccion_pkey");

            entity.ToTable("direccion");

            entity.HasIndex(e => e.Codigo, "uq_direccion_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_direccion_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Dium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dia_pkey");

            entity.ToTable("dia");

            entity.HasIndex(e => e.Nombre, "uq_dia_nombre").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasMaxLength(15)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Docente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("docente_pkey");

            entity.ToTable("docente");

            entity.HasIndex(e => e.IdDireccion, "idx_docente_direccion");

            entity.HasIndex(e => e.IdUsuario, "idx_docente_usuario");

            entity.HasIndex(e => e.CodigoEmpleado, "uq_docente_codigo_empleado").IsUnique();

            entity.HasIndex(e => e.IdUsuario, "uq_docente_usuario").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoEmpleado)
                .HasMaxLength(30)
                .HasColumnName("codigo_empleado");
            entity.Property(e => e.IdDireccion).HasColumnName("id_direccion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Docentes)
                .HasForeignKey(d => d.IdDireccion)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_docente_direccion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.Docente)
                .HasForeignKey<Docente>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_docente_usuario");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estudiante_pkey");

            entity.ToTable("estudiante");

            entity.HasIndex(e => e.IdCarrera, "idx_estudiante_carrera");

            entity.HasIndex(e => e.IdPlanAcademico, "idx_estudiante_plan_academico");

            entity.HasIndex(e => e.Carnet, "uq_estudiante_carnet").IsUnique();

            entity.HasIndex(e => e.Cedula, "uq_estudiante_cedula").IsUnique();

            entity.HasIndex(e => e.CodigoUnico, "uq_estudiante_codigo_unico").IsUnique();

            entity.HasIndex(e => e.Correo, "uq_estudiante_correo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Carnet)
                .HasMaxLength(20)
                .HasColumnName("carnet");
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .HasColumnName("cedula");
            entity.Property(e => e.CodigoUnico)
                .HasMaxLength(20)
                .HasColumnName("codigo_unico");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.FechaIngreso)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_ingreso");
            entity.Property(e => e.IdCarrera).HasColumnName("id_carrera");
            entity.Property(e => e.IdPlanAcademico).HasColumnName("id_plan_academico");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(60)
                .HasColumnName("primer_apellido");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(60)
                .HasColumnName("segundo_apellido");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdCarreraNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.IdCarrera)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_estudiante_carrera");

            entity.HasOne(d => d.IdPlanAcademicoNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.IdPlanAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_estudiante_plan_academico");
        });

        modelBuilder.Entity<Feriado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("feriado_pkey");

            entity.ToTable("feriado");

            entity.HasIndex(e => e.IdPeriodoEscolar, "idx_feriado_periodo_escolar");

            entity.HasIndex(e => e.IdTipoEvento, "idx_feriado_tipo_evento");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdPeriodoEscolar).HasColumnName("id_periodo_escolar");
            entity.Property(e => e.IdTipoEvento).HasColumnName("id_tipo_evento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdPeriodoEscolarNavigation).WithMany(p => p.Feriados)
                .HasForeignKey(d => d.IdPeriodoEscolar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_feriado_periodo_escolar");

            entity.HasOne(d => d.IdTipoEventoNavigation).WithMany(p => p.Feriados)
                .HasForeignKey(d => d.IdTipoEvento)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_feriado_tipo_evento");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grupo_pkey");

            entity.ToTable("grupo");

            entity.HasIndex(e => e.IdInstanciaAcademica, "idx_grupo_instancia");

            entity.HasIndex(e => new { e.IdInstanciaAcademica, e.Codigo }, "uq_grupo_instancia_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Cupo)
                .HasDefaultValue((short)35)
                .HasColumnName("cupo");
            entity.Property(e => e.IdInstanciaAcademica).HasColumnName("id_instancia_academica");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdInstanciaAcademicaNavigation).WithMany(p => p.Grupos)
                .HasForeignKey(d => d.IdInstanciaAcademica)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_grupo_instancia_academica");
        });

        modelBuilder.Entity<InscripcionGrupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inscripcion_grupo_pkey");

            entity.ToTable("inscripcion_grupo");

            entity.HasIndex(e => e.IdEstudiante, "idx_inscripcion_grupo_estudiante");

            entity.HasIndex(e => e.IdMateriaPorGrupo, "idx_inscripcion_grupo_materia");

            entity.HasIndex(e => new { e.IdMateriaPorGrupo, e.IdEstudiante }, "uq_inscripcion_grupo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaInscripcion)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha_inscripcion");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdMateriaPorGrupo).HasColumnName("id_materia_por_grupo");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.InscripcionGrupos)
                .HasForeignKey(d => d.IdEstudiante)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_inscripcion_grupo_estudiante");

            entity.HasOne(d => d.IdMateriaPorGrupoNavigation).WithMany(p => p.InscripcionGrupos)
                .HasForeignKey(d => d.IdMateriaPorGrupo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_inscripcion_grupo_materia_grupo");
        });

        modelBuilder.Entity<InstanciaAcademica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("instancia_academica_pkey");

            entity.ToTable("instancia_academica");

            entity.HasIndex(e => e.IdDetallePlanAcademico, "idx_instancia_academica_detalle_plan");

            entity.HasIndex(e => e.IdPeriodoEscolar, "idx_instancia_academica_periodo");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdDetallePlanAcademico).HasColumnName("id_detalle_plan_academico");
            entity.Property(e => e.IdPeriodoEscolar).HasColumnName("id_periodo_escolar");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdDetallePlanAcademicoNavigation).WithMany(p => p.InstanciaAcademicas)
                .HasForeignKey(d => d.IdDetallePlanAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_instancia_academica_detalle_plan");

            entity.HasOne(d => d.IdPeriodoEscolarNavigation).WithMany(p => p.InstanciaAcademicas)
                .HasForeignKey(d => d.IdPeriodoEscolar)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_instancia_academica_periodo_escolar");
        });

        modelBuilder.Entity<MallaCurricular>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("malla_curricular_pkey");

            entity.ToTable("malla_curricular");

            entity.HasIndex(e => e.IdDetallePlanAcademico, "idx_malla_curricular_detalle");

            entity.HasIndex(e => e.IdMateria, "idx_malla_curricular_materia");

            entity.HasIndex(e => new { e.IdDetallePlanAcademico, e.IdMateria }, "uq_malla_curricular_bloque_materia").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CantidadHorasMateria)
                .HasPrecision(5, 2)
                .HasColumnName("cantidad_horas_materia");
            entity.Property(e => e.Creditos).HasColumnName("creditos");
            entity.Property(e => e.EncuentrosPorSemana).HasColumnName("encuentros_por_semana");
            entity.Property(e => e.EncuentrosTotales).HasColumnName("encuentros_totales");
            entity.Property(e => e.IdDetallePlanAcademico).HasColumnName("id_detalle_plan_academico");
            entity.Property(e => e.IdMateria).HasColumnName("id_materia");
            entity.Property(e => e.Obligatoria)
                .HasDefaultValue(true)
                .HasColumnName("obligatoria");

            entity.HasOne(d => d.IdDetallePlanAcademicoNavigation).WithMany(p => p.MallaCurriculars)
                .HasForeignKey(d => d.IdDetallePlanAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_malla_curricular_detalle");

            entity.HasOne(d => d.IdMateriaNavigation).WithMany(p => p.MallaCurriculars)
                .HasForeignKey(d => d.IdMateria)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_malla_curricular_materia");
        });

        modelBuilder.Entity<MateriaPorGrupo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materia_por_grupo_pkey");

            entity.ToTable("materia_por_grupo");

            entity.HasIndex(e => e.IdDocente, "idx_materia_por_grupo_docente");

            entity.HasIndex(e => e.IdGrupo, "idx_materia_por_grupo_grupo");

            entity.HasIndex(e => e.IdMallaCurricular, "idx_materia_por_grupo_malla");

            entity.HasIndex(e => new { e.IdGrupo, e.IdMallaCurricular }, "uq_materia_por_grupo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CupoLimite)
                .HasDefaultValue((short)35)
                .HasColumnName("cupo_limite");
            entity.Property(e => e.IdDocente).HasColumnName("id_docente");
            entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");
            entity.Property(e => e.IdMallaCurricular).HasColumnName("id_malla_curricular");

            entity.HasOne(d => d.IdDocenteNavigation).WithMany(p => p.MateriaPorGrupos)
                .HasForeignKey(d => d.IdDocente)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_materia_por_grupo_docente");

            entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.MateriaPorGrupos)
                .HasForeignKey(d => d.IdGrupo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_materia_por_grupo_grupo");

            entity.HasOne(d => d.IdMallaCurricularNavigation).WithMany(p => p.MateriaPorGrupos)
                .HasForeignKey(d => d.IdMallaCurricular)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_materia_por_grupo_malla");
        });

        modelBuilder.Entity<MateriaPrerequisito>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materia_prerequisito_pkey");

            entity.ToTable("materia_prerequisito");

            entity.HasIndex(e => e.IdMallaCurricularMateria, "idx_materia_prerequisito_materia");

            entity.HasIndex(e => e.IdMallaCurricularMateriaPrerequisito, "idx_materia_prerequisito_materia_pre");

            entity.HasIndex(e => new { e.IdMallaCurricularMateria, e.IdMallaCurricularMateriaPrerequisito }, "uq_materia_prerequisito").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.IdMallaCurricularMateria).HasColumnName("id_malla_curricular_materia");
            entity.Property(e => e.IdMallaCurricularMateriaPrerequisito).HasColumnName("id_malla_curricular_materia_prerequisito");

            entity.HasOne(d => d.IdMallaCurricularMateriaNavigation).WithMany(p => p.MateriaPrerequisitoIdMallaCurricularMateriaNavigations)
                .HasForeignKey(d => d.IdMallaCurricularMateria)
                .HasConstraintName("fk_prerequisito_materia");

            entity.HasOne(d => d.IdMallaCurricularMateriaPrerequisitoNavigation).WithMany(p => p.MateriaPrerequisitoIdMallaCurricularMateriaPrerequisitoNavigations)
                .HasForeignKey(d => d.IdMallaCurricularMateriaPrerequisito)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_prerequisito_materia_pre");
        });

        modelBuilder.Entity<Materium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materia_pkey");

            entity.ToTable("materia");

            entity.HasIndex(e => e.Codigo, "uq_materia_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_materia_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(25)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Modalidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("modalidad_pkey");

            entity.ToTable("modalidad");

            entity.HasIndex(e => e.Codigo, "uq_modalidad_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_modalidad_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MotivoAusencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("motivo_ausencia_pkey");

            entity.ToTable("motivo_ausencia");

            entity.HasIndex(e => e.Codigo, "uq_motivo_ausencia_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_motivo_ausencia_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.AplicaA)
                .HasMaxLength(15)
                .HasDefaultValueSql("'AMBOS'::character varying")
                .HasColumnName("aplica_a");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.JustificaInasistencia)
                .HasDefaultValue(true)
                .HasColumnName("justifica_inasistencia");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<PeriodoAcademico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("periodo_academico_pkey");

            entity.ToTable("periodo_academico");

            entity.HasIndex(e => e.IdTipoPeriodoAcademico, "idx_periodo_academico_tipo");

            entity.HasIndex(e => new { e.IdTipoPeriodoAcademico, e.Codigo }, "uq_periodo_academico_tipo_codigo").IsUnique();

            entity.HasIndex(e => new { e.IdTipoPeriodoAcademico, e.Orden }, "uq_periodo_academico_tipo_orden").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.IdTipoPeriodoAcademico).HasColumnName("id_tipo_periodo_academico");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Orden).HasColumnName("orden");

            entity.HasOne(d => d.IdTipoPeriodoAcademicoNavigation).WithMany(p => p.PeriodoAcademicos)
                .HasForeignKey(d => d.IdTipoPeriodoAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_periodo_academico_tipo");
        });

        modelBuilder.Entity<PeriodoEscolar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("periodo_escolar_pkey");

            entity.ToTable("periodo_escolar");

            entity.HasIndex(e => e.NombrePeriodoEscolar, "uq_periodo_escolar_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.NombrePeriodoEscolar)
                .HasMaxLength(50)
                .HasColumnName("nombre_periodo_escolar");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permiso_pkey");

            entity.ToTable("permiso");

            entity.HasIndex(e => e.Entidad, "idx_permiso_entidad");

            entity.HasIndex(e => e.IdRol, "idx_permiso_id_rol");

            entity.HasIndex(e => new { e.IdRol, e.Entidad }, "uq_permiso_rol_entidad").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Entidad)
                .HasMaxLength(60)
                .HasColumnName("entidad");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("now()")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("now()")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Permisos)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_permiso_rol");
        });

        modelBuilder.Entity<PlanAcademico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("plan_academico_pkey");

            entity.ToTable("plan_academico");

            entity.HasIndex(e => e.IdCarrera, "idx_plan_academico_carrera");

            entity.HasIndex(e => e.IdCentroUniversitario, "idx_plan_academico_centro");

            entity.HasIndex(e => e.IdModalidad, "idx_plan_academico_modalidad");

            entity.HasIndex(e => e.IdTipoPeriodoAcademico, "idx_plan_academico_tipo_periodo");

            entity.HasIndex(e => e.Codigo, "uq_plan_academico_codigo").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ActivoDesde).HasColumnName("activo_desde");
            entity.Property(e => e.ActivoHasta).HasColumnName("activo_hasta");
            entity.Property(e => e.CantidadCicloAcademico).HasColumnName("cantidad_ciclo_academico");
            entity.Property(e => e.CantidadPeriodoAcademico).HasColumnName("cantidad_periodo_academico");
            entity.Property(e => e.Codigo)
                .HasMaxLength(30)
                .HasColumnName("codigo");
            entity.Property(e => e.IdCarrera).HasColumnName("id_carrera");
            entity.Property(e => e.IdCentroUniversitario).HasColumnName("id_centro_universitario");
            entity.Property(e => e.IdModalidad).HasColumnName("id_modalidad");
            entity.Property(e => e.IdTipoPeriodoAcademico).HasColumnName("id_tipo_periodo_academico");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCarreraNavigation).WithMany(p => p.PlanAcademicos)
                .HasForeignKey(d => d.IdCarrera)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_plan_academico_carrera");

            entity.HasOne(d => d.IdCentroUniversitarioNavigation).WithMany(p => p.PlanAcademicos)
                .HasForeignKey(d => d.IdCentroUniversitario)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_plan_academico_centro");

            entity.HasOne(d => d.IdModalidadNavigation).WithMany(p => p.PlanAcademicos)
                .HasForeignKey(d => d.IdModalidad)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_plan_academico_modalidad");

            entity.HasOne(d => d.IdTipoPeriodoAcademicoNavigation).WithMany(p => p.PlanAcademicos)
                .HasForeignKey(d => d.IdTipoPeriodoAcademico)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_plan_academico_tipo_periodo");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rol_pkey");

            entity.ToTable("rol");

            entity.HasIndex(e => e.Code, "uq_rol_code").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Code)
                .HasMaxLength(30)
                .HasColumnName("code");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoAula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_aula_pkey");

            entity.ToTable("tipo_aula");

            entity.HasIndex(e => e.Codigo, "uq_tipo_aula_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_tipo_aula_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoEvento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_evento_pkey");

            entity.ToTable("tipo_evento");

            entity.HasIndex(e => e.Codigo, "uq_tipo_evento_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_tipo_evento_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoPeriodoAcademico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_periodo_academico_pkey");

            entity.ToTable("tipo_periodo_academico");

            entity.HasIndex(e => e.Codigo, "uq_tipo_periodo_academico_codigo").IsUnique();

            entity.HasIndex(e => e.Nombre, "uq_tipo_periodo_academico_nombre").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .HasColumnName("codigo");
            entity.Property(e => e.DuracionHorasMaximo).HasColumnName("duracion_horas_maximo");
            entity.Property(e => e.DuracionHorasMinimo).HasColumnName("duracion_horas_minimo");
            entity.Property(e => e.DuracionMesesMaximo).HasColumnName("duracion_meses_maximo");
            entity.Property(e => e.DuracionMesesMinimo).HasColumnName("duracion_meses_minimo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.IdRol, "idx_usuario_id_rol");

            entity.HasIndex(e => e.IdUser, "idx_usuario_id_user");

            entity.HasIndex(e => e.Cedula, "usuario_cedula_key").IsUnique();

            entity.HasIndex(e => e.IdUser, "usuario_id_user_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .HasColumnName("cedula");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuario_rol");

            entity.HasOne(d => d.AuthUser).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_usuario_user");
        });

        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.ToTable("users", "auth");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.EncryptedPassword).HasMaxLength(255).HasColumnName("encrypted_password");
            entity.Property(e => e.EmailConfirmedAt).HasColumnName("email_confirmed_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
