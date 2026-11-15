using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.Attendances
{
    public sealed class SessionAttendanceSheetDto
    {
        [JsonPropertyName("sesion")]
        public SessionAttendanceInfoDto Session { get; set; } = new();

        [JsonPropertyName("cabecera_asistencia")]
        public SessionAttendanceHeaderDto AttendanceHeader { get; set; } = new();

        [JsonPropertyName("estudiantes")]
        public IReadOnlyList<SessionAttendanceStudentDto> Students { get; set; } = [];
    }

    public sealed class SessionAttendanceInfoDto
    {
        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("aula_codigo")]
        public string ClassroomCode { get; set; } = string.Empty;

        [JsonPropertyName("docente_titular_nombre")]
        public string TitularTeacherName { get; set; } = string.Empty;

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("fecha_fin")]
        public DateTimeOffset EndDate { get; set; }
    }

    public sealed class SessionAttendanceHeaderDto
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("docente_asignado_asistio")]
        public bool? AssignedTeacherAttended { get; set; }

        [JsonPropertyName("id_docente_sustituto")]
        public Guid? SubstituteTeacherId { get; set; }

        [JsonPropertyName("docente_sustituto_nombre")]
        public string? SubstituteTeacherName { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }

        [JsonPropertyName("fecha_registro_docente")]
        public DateTimeOffset? TeacherRecordedAt { get; set; }
    }

    public sealed class SessionAttendanceStudentDto
    {
        [JsonPropertyName("id_asistencia_detalle")]
        public Guid? AttendanceDetailId { get; set; }

        [JsonPropertyName("id_inscripcion_grupo")]
        public Guid GroupEnrollmentId { get; set; }

        [JsonPropertyName("id_estudiante")]
        public Guid StudentId { get; set; }

        [JsonPropertyName("codigo_unico")]
        public string UniqueCode { get; set; } = string.Empty;

        [JsonPropertyName("carnet")]
        public string StudentCarnet { get; set; } = string.Empty;

        [JsonPropertyName("nombre_completo")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("asistio")]
        public bool Attended { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }

        [JsonPropertyName("fecha_registro_estudiante")]
        public DateTimeOffset? StudentRecordedAt { get; set; }
    }

    public sealed class SaveAttendanceSheetDto
    {
        [JsonPropertyName("docente_asignado_asistio")]
        public bool AssignedTeacherAttended { get; set; }

        [JsonPropertyName("id_docente_sustituto")]
        public Guid? SubstituteTeacherId { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }

        [JsonPropertyName("detalles")]
        public IReadOnlyList<SaveAttendanceDetailItemDto> Details { get; set; } = [];
    }

    public sealed class SaveAttendanceDetailItemDto
    {
        [JsonPropertyName("id_inscripcion_grupo")]
        public Guid GroupEnrollmentId { get; set; }

        [JsonPropertyName("asistio")]
        public bool Attended { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }
    }

    public sealed class SaveAttendanceSheetResponseDto
    {
        [JsonPropertyName("id_asistencia")]
        public Guid AttendanceId { get; set; }

        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("docente_asignado_asistio")]
        public bool AssignedTeacherAttended { get; set; }

        [JsonPropertyName("id_docente_sustituto")]
        public Guid? SubstituteTeacherId { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }

        [JsonPropertyName("fecha_registro_docente")]
        public DateTimeOffset TeacherRecordedAt { get; set; }

        [JsonPropertyName("resumen")]
        public AttendanceSummaryCountsDto Summary { get; set; } = new();
    }

    public sealed class AttendanceSummaryCountsDto
    {
        [JsonPropertyName("presentes")]
        public int PresentCount { get; set; }

        [JsonPropertyName("ausentes")]
        public int AbsentCount { get; set; }

        [JsonPropertyName("justificados")]
        public int ExcusedCount { get; set; }
    }

    public sealed class UpdateAttendanceDetailDto
    {
        [JsonPropertyName("asistio")]
        public bool Attended { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }
    }

    public sealed class AttendanceDetailResponseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_asistencia")]
        public Guid AttendanceId { get; set; }

        [JsonPropertyName("id_inscripcion_grupo")]
        public Guid GroupEnrollmentId { get; set; }

        [JsonPropertyName("asistio")]
        public bool Attended { get; set; }

        [JsonPropertyName("id_motivo_ausencia")]
        public Guid? AbsenceReasonId { get; set; }

        [JsonPropertyName("fecha_registro_estudiante")]
        public DateTimeOffset StudentRecordedAt { get; set; }
    }

    public sealed class GroupAttendanceRosterDto
    {
        [JsonPropertyName("curso")]
        public RosterCourseInfoDto Course { get; set; } = new();

        [JsonPropertyName("sesiones_columnas")]
        public IReadOnlyList<RosterSessionColumnDto> SessionColumns { get; set; } = [];

        [JsonPropertyName("alumnos_filas")]
        public IReadOnlyList<RosterStudentRowDto> StudentRows { get; set; } = [];
    }

    public sealed class RosterCourseInfoDto
    {
        [JsonPropertyName("id_materia_por_grupo")]
        public Guid GroupSubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("docente_nombre")]
        public string TeacherName { get; set; } = string.Empty;

        [JsonPropertyName("instancia_nombre")]
        public string AcademicInstanceName { get; set; } = string.Empty;
    }

    public sealed class RosterSessionColumnDto
    {
        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("numero_encuentro")]
        public int MeetingNumber { get; set; }

        [JsonPropertyName("fecha_inicio")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("acta_registrada")]
        public bool HasAttendanceRecord { get; set; }
    }

    public sealed class RosterStudentRowDto
    {
        [JsonPropertyName("id_inscripcion_grupo")]
        public Guid GroupEnrollmentId { get; set; }

        [JsonPropertyName("id_estudiante")]
        public Guid StudentId { get; set; }

        [JsonPropertyName("codigo_unico")]
        public string UniqueCode { get; set; } = string.Empty;

        [JsonPropertyName("carnet")]
        public string StudentCarnet { get; set; } = string.Empty;

        [JsonPropertyName("nombre_completo")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("marcajes")]
        public IReadOnlyList<RosterMarkDto> Marks { get; set; } = [];

        [JsonPropertyName("estadisticas")]
        public RosterStudentStatsDto Stats { get; set; } = new();
    }

    public sealed class RosterMarkDto
    {
        [JsonPropertyName("id_calendario")]
        public Guid CalendarSessionId { get; set; }

        [JsonPropertyName("estado")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("motivo_ausencia_nombre")]
        public string? AbsenceReasonName { get; set; }

        [JsonPropertyName("fecha_registro_estudiante")]
        public DateTimeOffset? StudentRecordedAt { get; set; }
    }

    public sealed class RosterStudentStatsDto
    {
        [JsonPropertyName("total_presentes")]
        public int TotalPresent { get; set; }

        [JsonPropertyName("total_ausentes_injustificados")]
        public int TotalUnexcusedAbsent { get; set; }

        [JsonPropertyName("total_justificados")]
        public int TotalExcused { get; set; }

        [JsonPropertyName("porcentaje_asistencia")]
        public decimal AttendancePercentage { get; set; }

        [JsonPropertyName("alerta_riesgo")]
        public string RiskAlert { get; set; } = string.Empty;
    }
}