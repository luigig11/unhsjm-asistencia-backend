using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.AcademicPeriodTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPeriodTypes
{
    public sealed class CreateAcademicPeriodTypeCommand : TransactionalCommandBase<AcademicPeriodTypeDto>
    {
        private readonly IAcademicPeriodTypeRepository _repository;
        private CreateAcademicPeriodTypeDto? _request;

        public CreateAcademicPeriodTypeCommand(IDapperRlsContext dbContext, IAcademicPeriodTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateAcademicPeriodTypeCommand WithRequest(CreateAcademicPeriodTypeDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListAcademicPeriodTypesCommand.ValidateFields(
                dto.Code, dto.Name, dto.MinMonthsDuration, dto.MaxMonthsDuration, dto.MinHoursDuration, dto.MaxHoursDuration);
        }

        protected override async Task<AcademicPeriodTypeDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new AcademicPeriodTypeEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    MinMonthsDuration = dto.MinMonthsDuration,
                    MaxMonthsDuration = dto.MaxMonthsDuration,
                    MinHoursDuration = dto.MinHoursDuration,
                    MaxHoursDuration = dto.MaxHoursDuration,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListAcademicPeriodTypesCommand.MapToDto(created);
        }
    }
}
