using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AbsenceReasons;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AbsenceReasons
{
    public sealed class CreateAbsenceReasonCommand : TransactionalCommandBase<AbsenceReasonDto>
    {
        private readonly IAbsenceReasonRepository _repository;
        private CreateAbsenceReasonDto? _request;

        public CreateAbsenceReasonCommand(IDapperRlsContext dbContext, IAbsenceReasonRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAbsenceReasonCommand WithRequest(CreateAbsenceReasonDto request)
        {
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
            var created = await _repository.CreateAsync(
                new AbsenceReasonEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    JustifiesAbsence = dto.JustifiesAbsence,
                    AppliesTo = dto.AppliesTo.Trim().ToUpperInvariant(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAbsenceReasonsCommand.MapToDto(created);
        }
    }
}
