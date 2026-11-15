using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.DTOs.Subjects;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Subjects
{
    public sealed class ListSubjectsCommand : CommandBase<IReadOnlyList<SubjectDto>>
    {
        private readonly ISubjectRepository _repository;
        private string? _search;
        private bool? _isActive;

        public ListSubjectsCommand(ISubjectRepository repository)
        {
            _repository = repository;
        }

        public ListSubjectsCommand WithFilter(string? search, bool? isActive)
        {
            _search = search;
            _isActive = isActive;
            return this;
        }

        public override async Task<IReadOnlyList<SubjectDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(_search, _isActive, cancellationToken);
            return items.Select(MapToDto).ToList();
        }

        internal static SubjectDto MapToDto(SubjectEntity entity) => new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsActive = entity.IsActive
        };

        internal static void ValidateFields(string? code, string? name)
        {
            Validator.Create()
                .Field("codigo", code, "Code").NotEmpty().MaxLength(25)
                .Field("nombre", name, "Name").NotEmpty().MaxLength(150)
                .ValidateAndThrow();
        }
    }
}
