using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AbsenceReasons;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AbsenceReasons
{
    public sealed class UpdateAbsenceReasonCommand : TransactionalCommandBase<AbsenceReasonDto>
    {
        private readonly IAbsenceReasonRepository _repository;
        private Guid _id;
        private UpdateAbsenceReasonDto? _request;

        public UpdateAbsenceReasonCommand(IDapperRlsContext dbContext, IAbsenceReasonRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAbsenceReasonCommand WithRequest(Guid id, UpdateAbsenceReasonDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAbsenceReasonsCommand.ValidateFields(dto.Code, dto.Name, dto.Description, dto.AppliesTo);
        }

        protected override async Task<AbsenceReasonDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new AbsenceReasonEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    JustifiesAbsence = dto.JustifiesAbsence,
                    AppliesTo = dto.AppliesTo.Trim().ToUpperInvariant(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Motivo de ausencia con id '{_id}' no fue encontrado.");

            return ListAbsenceReasonsCommand.MapToDto(updated);
        }
    }
}
