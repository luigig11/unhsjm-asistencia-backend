using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.Classrooms;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.Classrooms
{
    public sealed class UpdateClassroomCommand : TransactionalCommandBase<ClassroomDto>
    {
        private readonly IClassroomRepository _repository;
        private Guid _id;
        private UpdateClassroomDto? _request;

        public UpdateClassroomCommand(IDapperRlsContext dbContext, IClassroomRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateClassroomCommand WithRequest(Guid id, UpdateClassroomDto request)
        {
            _id = id;
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
            var updated = await _repository.UpdateAsync(
                _id,
                new ClassroomEntity
                {
                    Id = _id,
                    ClassroomTypeId = dto.ClassroomTypeId,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Capacity = dto.Capacity,
                    Location = string.IsNullOrWhiteSpace(dto.Location) ? null : dto.Location.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Salon de clase con id '{_id}' no fue encontrado.");

            return ListClassroomsCommand.MapToDto(updated);
        }
    }
}
