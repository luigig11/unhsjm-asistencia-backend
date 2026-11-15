using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Groups;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Groups
{
    public sealed class CreateGroupCommand : TransactionalCommandBase<GroupDto>
    {
        private readonly IGroupRepository _repository;
        private CreateGroupDto? _request;

        public CreateGroupCommand(IDapperRlsContext dbContext, IGroupRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateGroupCommand WithRequest(CreateGroupDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListGroupsCommand.ValidateFields(dto.AcademicInstanceId, dto.Code, dto.Name, dto.Capacity);
        }

        protected override async Task<GroupDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new GroupEntity
                {
                    AcademicInstanceId = dto.AcademicInstanceId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Capacity = dto.Capacity,
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListGroupsCommand.MapToDto(created);
        }
    }
}