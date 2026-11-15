using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Days;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Days
{
    public sealed class ListDaysCommand : CommandBase<IReadOnlyList<DayDto>>
    {
        private readonly IDayRepository _repository;

        public ListDaysCommand(IDayRepository repository)
        {
            _repository = repository;
        }

        public override async Task<IReadOnlyList<DayDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items
                .Select(d => new DayDto
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToList();
        }
    }
}
