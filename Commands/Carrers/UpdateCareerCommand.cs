using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Carrers
{
    public sealed class UpdateCareerCommand : TransactionalCommandBase<CareerDto>
    {
        private readonly ICareerRepository _careerRepository;
        private Guid _id;
        private UpdateCareerDto? _request;

        public UpdateCareerCommand(IDapperRlsContext dbContext, ICareerRepository careerRepository)
            : base(dbContext)
        {
            _careerRepository = careerRepository;
        }

        public UpdateCareerCommand WithRequest(Guid id, UpdateCareerDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("REquest payloads es requerido.");
            ListCareersCommand.ValidateFields(dto.DepartmentId, dto.Code, dto.Name);
        }

        protected override async Task<CareerDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _careerRepository.UpdateAsync(
                _id,
                new CareerEntity
                {
                    Id = _id,
                    DepartmentId = dto.DepartmentId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Carrera con id '{_id}' no fue encontrada.");

            return ListCareersCommand.MapToDto(updated);
        }
    }
}
