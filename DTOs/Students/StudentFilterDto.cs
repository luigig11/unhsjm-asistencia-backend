using Microsoft.AspNetCore.Mvc;

namespace unhsjm_asistencia_backend.DTOs.Students
{
    public sealed class StudentFilterDto
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "codigo_unico")]
        public string? UniqueCode { get; set; }

        [FromQuery(Name = "carnet")]
        public string? StudentCarnet { get; set; }

        [FromQuery(Name = "id_carrera")]
        public Guid? CareerId { get; set; }

        [FromQuery(Name = "id_plan_academico")]
        public Guid? AcademicPlanId { get; set; }

        [FromQuery(Name = "activo")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 20;
    }
}
