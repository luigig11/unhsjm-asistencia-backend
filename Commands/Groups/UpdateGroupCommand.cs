using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Groups;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Groups
{
    public sealed class UpdateGroupCommand : TransactionalCommandBase<GroupDto>
    {
        private readonly IGroupRepository _repository;
        private Guid _id;
        private UpdateGroupDto? _request;

        public UpdateGroupCommand(IDapperRlsContext dbContext, IGroupRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateGroupCommand WithRequest(Guid id, UpdateGroupDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new GroupEntity
                {
                    Id = _id,
                    AcademicInstanceId = dto.AcademicInstanceId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Capacity = dto.Capacity,
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"GRupo con id '{_id}' no fue encontrado.");

            return ListGroupsCommand.MapToDto(updated);
        }
    }
}