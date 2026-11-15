using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPeriodTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriodTypes
{
    public sealed class UpdateAcademicPeriodTypeCommand : TransactionalCommandBase<AcademicPeriodTypeDto>
    {
        private readonly IAcademicPeriodTypeRepository _repository;
        private Guid _id;
        private UpdateAcademicPeriodTypeDto? _request;

        public UpdateAcademicPeriodTypeCommand(IDapperRlsContext dbContext, IAcademicPeriodTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateAcademicPeriodTypeCommand WithRequest(Guid id, UpdateAcademicPeriodTypeDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("REquest payload es requerido.");
            ListAcademicPeriodTypesCommand.ValidateFields(
                dto.Code, dto.Name, dto.MinMonthsDuration, dto.MaxMonthsDuration, dto.MinHoursDuration, dto.MaxHoursDuration);
        }

        protected override async Task<AcademicPeriodTypeDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new AcademicPeriodTypeEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    MinMonthsDuration = dto.MinMonthsDuration,
                    MaxMonthsDuration = dto.MaxMonthsDuration,
                    MinHoursDuration = dto.MinHoursDuration,
                    MaxHoursDuration = dto.MaxHoursDuration,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Tipo de periodo academico con id '{_id}' no fue encontrado.");

            return ListAcademicPeriodTypesCommand.MapToDto(updated);
        }
    }
}
