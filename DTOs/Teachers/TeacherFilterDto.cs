using Microsoft.AspNetCore.Mvc;

namespace unhsjm_asistencia_backend.DTOs.Teachers
{
    public sealed class TeacherFilterDto
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "id_direccion")]
        public Guid? DepartmentId { get; set; }

        [FromQuery(Name = "tipo_contratacion")]
        public string? ContractType { get; set; }

        [FromQuery(Name = "activo")]
        public bool? IsActive { get; set; }
    }
}
