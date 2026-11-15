using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.GroupSubjects
{
    public sealed class GroupSubjectListItemDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_grupo")]
        public Guid GroupId { get; set; }

        [JsonPropertyName("grupo_codigo")]
        public string GroupCode { get; set; } = string.Empty;

        [JsonPropertyName("id_malla_curricular")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_materia")]
        public Guid SubjectId { get; set; }

        [JsonPropertyName("materia_codigo")]
        public string SubjectCode { get; set; } = string.Empty;

        [JsonPropertyName("materia_nombre")]
        public string SubjectName { get; set; } = string.Empty;

        [JsonPropertyName("id_docente")]
        public Guid TeacherId { get; set; }

        [JsonPropertyName("docente_nombre_completo")]
        public string TeacherFullName { get; set; } = string.Empty;

        [JsonPropertyName("cupo_limite")]
        public short QuotaLimit { get; set; }

        [JsonPropertyName("total_inscritos")]
        public int TotalEnrolled { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class GroupSubjectDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_grupo")]
        public Guid GroupId { get; set; }

        [JsonPropertyName("id_malla_curricular")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_docente")]
        public Guid TeacherId { get; set; }

        [JsonPropertyName("cupo_limite")]
        public short QuotaLimit { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateGroupSubjectDto
    {
        [JsonPropertyName("id_grupo")]
        public Guid GroupId { get; set; }

        [JsonPropertyName("id_malla_curricular")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_docente")]
        public Guid TeacherId { get; set; }

        [JsonPropertyName("cupo_limite")]
        public short QuotaLimit { get; set; } = 35;

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }

    public sealed class UpdateGroupSubjectDto
    {
        [JsonPropertyName("id_docente")]
        public Guid TeacherId { get; set; }

        [JsonPropertyName("cupo_limite")]
        public short QuotaLimit { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }
}