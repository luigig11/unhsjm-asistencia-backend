using Microsoft.AspNetCore.Mvc;

namespace unhsjm_asistencia_backend.DTOs.Users
{
    public sealed class UserFilterDto
    {
        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "codigo_unico")]
        public string? Unique_Code { get; set; }

        [FromQuery(Name = "id_rol")]
        public Guid? RoleId { get; set; }

        [FromQuery(Name = "activo")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 20;
    }
}
