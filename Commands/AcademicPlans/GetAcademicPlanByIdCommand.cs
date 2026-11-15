using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.AcademicPlans;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.AcademicPlans
{
    public sealed class GetAcademicPlanByIdCommand : CommandBase<AcademicPlanDto>
    {
        private readonly IAcademicPlanRepository _repository;
        private Guid _id;

        public GetAcademicPlanByIdCommand(IAcademicPlanRepository repository)
        {
            _repository = repository;
        }

        public GetAcademicPlanByIdCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public override async Task<AcademicPlanDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(_id, cancellationToken)
                ?? throw new NotFoundApiException($"Plan academico con id '{_id}' no fue encontrado.");

            return ListAcademicPlansCommand.MapToDto(entity);
        }
    }
}