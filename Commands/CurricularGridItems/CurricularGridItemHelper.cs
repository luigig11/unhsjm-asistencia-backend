using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.CurricularGridItems;
using unhsjm_asistencia_backend.Entities;

namespace unhsjm_asistencia_backend.Commands.CurricularGridItems
{
    internal static class CurricularGridItemHelper
    {
        internal static CurricularGridItemDto MapToDto(CurricularGridItemEntity entity, IReadOnlyList<Guid> prerequisiteIds) => new()
        {
            Id = entity.Id,
            AcademicPlanDetailId = entity.AcademicPlanDetailId,
            SubjectId = entity.SubjectId,
            TotalMeetings = entity.TotalMeetings,
            MeetingsPerWeek = entity.MeetingsPerWeek,
            SubjectHours = entity.SubjectHours,
            Credits = entity.Credits,
            IsMandatory = entity.IsMandatory,
            IsActive = entity.IsActive,
            PrerequisiteCurricularGridItemIds = prerequisiteIds
        };

        internal static void ValidateFields(
            Guid? itemId,
            Guid academicPlanDetailId,
            Guid subjectId,
            short totalMeetings,
            short meetingsPerWeek,
            decimal subjectHours,
            short credits,
            IReadOnlyList<Guid>? prerequisiteIds)
        {
            var validator = Validator.Create()
                .Field("id_detalle_plan_academico", academicPlanDetailId, "detalle de plan academico").RequiredGuid()
                .Field("id_materia", subjectId, "materia").RequiredGuid()
                .Field("encuentros_totales", totalMeetings, "encuentros totales").Positive()
                .Field("encuentros_por_semana", meetingsPerWeek, "encuentros por semana").Between(1, 7)
                .Rule("encuentros_totales", () => totalMeetings >= meetingsPerWeek, "Encuentros totales debe ser mayor o igual a encuentros por semana.")
                .Field("cantidad_horas_materia", subjectHours, "Horas de la materia").Between(0.01m, 999.99m)
                .Field("creditos", credits, "Credits").Positive();

            if (prerequisiteIds is not null)
            {
                validator
                    .Rule("ids_malla_prerequisitos", () => prerequisiteIds.All(id => id != Guid.Empty), "Todos los ids de los prerequisitos deben ser validos.")
                    .Rule("ids_malla_prerequisitos", () => prerequisiteIds.Distinct().Count() == prerequisiteIds.Count, "No se permite duplicar ids de prerequisitos.");

                if (itemId.HasValue)
                {
                    validator.Rule(
                        "ids_malla_prerequisitos",
                        () => !prerequisiteIds.Contains(itemId.Value),
                        "Una materia no se puede referencias a si misma como prerequisitos.");
                }
            }

            validator.ValidateAndThrow();
        }
    }
}