using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Carrers;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Carrers
{
    public sealed class GetCareerByIdCommand : CommandBase<CareerDto>
    {
        private readonly ICareerRepository _careerRepository;
        private Guid _id;

        public GetCareerByIdCommand(ICareerRepository careerRepository)
        {
            _careerRepository = careerRepository;
        }

        public GetCareerByIdCommand WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public override async Task<CareerDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var entity = await _careerRepository.GetByIdAsync(_id, cancellationToken)
                ?? throw new NotFoundApiException($"Carrera con id '{_id}' no fue encontrado.");

            return ListCareersCommand.MapToDto(entity);
        }
    }
}
