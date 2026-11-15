using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.SubjectPrerequisites;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.SubjectPrerequisites
{
    public sealed class CreateSubjectPrerequisiteCommand : TransactionalCommandBase<SubjectPrerequisiteDto>
    {
        private readonly ISubjectPrerequisiteRepository _repository;
        private CreateSubjectPrerequisiteDto? _request;

        public CreateSubjectPrerequisiteCommand(IDapperRlsContext dbContext, ISubjectPrerequisiteRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateSubjectPrerequisiteCommand WithRequest(CreateSubjectPrerequisiteDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");

            Validator.Create()
                .Field("id_malla_curricular_materia", dto.CurricularGridItemId, "Item de malla curricular").RequiredGuid()
                .Field("id_malla_curricular_materia_prerequisito", dto.PrerequisiteCurricularGridItemId, "Item prerequisito de malla curricular").RequiredGuid()
                .Rule(
                    "id_malla_curricular_materia_prerequisito",
                    () => dto.CurricularGridItemId != dto.PrerequisiteCurricularGridItemId,
                    "Un item de malla curricular no puede ser prerequisio de si mismo.")
                .ValidateAndThrow();
        }

        protected override async Task<SubjectPrerequisiteDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new SubjectPrerequisiteEntity
                {
                    CurricularGridItemId = dto.CurricularGridItemId,
                    PrerequisiteCurricularGridItemId = dto.PrerequisiteCurricularGridItemId,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return new SubjectPrerequisiteDto
            {
                Id = created.Id,
                CurricularGridItemId = created.CurricularGridItemId,
                PrerequisiteCurricularGridItemId = created.PrerequisiteCurricularGridItemId,
                IsActive = created.IsActive
            };
        }
    }
}