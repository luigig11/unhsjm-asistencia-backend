using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Classrooms;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Classrooms
{
    public sealed class CreateClassroomCommand : TransactionalCommandBase<ClassroomDto>
    {
        private readonly IClassroomRepository _repository;
        private CreateClassroomDto? _request;

        public CreateClassroomCommand(IDapperRlsContext dbContext, IClassroomRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateClassroomCommand WithRequest(CreateClassroomDto request)
        {
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListClassroomsCommand.ValidateFields(dto.ClassroomTypeId, dto.Code, dto.Name, dto.Capacity, dto.Location);
        }

        protected override async Task<ClassroomDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var created = await _repository.CreateAsync(
                new ClassroomEntity
                {
                    ClassroomTypeId = dto.ClassroomTypeId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Capacity = dto.Capacity,
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListClassroomsCommand.MapToDto(created);
        }
    }
}
