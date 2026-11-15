using System.Text.Json.Serialization;

namespace unhsjm_asistencia_backend.DTOs.SubjectPrerequisites
{
    public sealed class SubjectPrerequisiteDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("id_malla_curricular_materia")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_malla_curricular_materia_prerequisito")]
        public Guid PrerequisiteCurricularGridItemId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; }
    }

    public sealed class CreateSubjectPrerequisiteDto
    {
        [JsonPropertyName("id_malla_curricular_materia")]
        public Guid CurricularGridItemId { get; set; }

        [JsonPropertyName("id_malla_curricular_materia_prerequisito")]
        public Guid PrerequisiteCurricularGridItemId { get; set; }

        [JsonPropertyName("activo")]
        public bool IsActive { get; set; } = true;
    }
}