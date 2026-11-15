using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Carrers;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Carrers
{
    public sealed class CreateCareerCommand : TransactionalCommandBase<CareerDto>
    {
        private readonly ICareerRepository _careerRepository;
        private CreateCareerDto? _request;

        public CreateCareerCommand(IDapperRlsContext dbContext, ICareerRepository careerRepository)
            : base(dbContext)
        {
            _careerRepository = careerRepository;
        }

        public CreateCareerCommand WithRequest(CreateCareerDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListCareersCommand.ValidateFields(dto.DepartmentId, dto.Code, dto.Name);
        }

        protected override async Task<CareerDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _careerRepository.CreateAsync(
                new CareerEntity
                {
                    DepartmentId = dto.DepartmentId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListCareersCommand.MapToDto(created);
        }
    }
}
