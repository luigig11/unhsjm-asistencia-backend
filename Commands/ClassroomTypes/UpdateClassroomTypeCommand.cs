using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.ClassroomTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Exceptions;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.ClassroomTypes
{
    public sealed class UpdateClassroomTypeCommand : TransactionalCommandBase<ClassroomTypeDto>
    {
        private readonly IClassroomTypeRepository _repository;
        private Guid _id;
        private UpdateClassroomTypeDto? _request;

        public UpdateClassroomTypeCommand(IDapperRlsContext dbContext, IClassroomTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public UpdateClassroomTypeCommand WithRequest(Guid id, UpdateClassroomTypeDto request)
        {
            _id = id;
            _request = request;
            return this;
        }

        protected override void ValidateBeforeTransaction()
        {
            var dto = _request ?? throw new InvalidOperationException("Request payload es requerido.");
            ListClassroomTypesCommand.ValidateFields(dto.Code, dto.Name, dto.Description);
        }

        protected override async Task<ClassroomTypeDto> ExecuteInTransactionAsync(CancellationToken cancellationToken)
        {
            var dto = _request!;
            var updated = await _repository.UpdateAsync(
                _id,
                new ClassroomTypeEntity
                {
                    Id = _id,
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken)
                ?? throw new NotFoundApiException($"Tipo de salon de clase con id  '{_id}' no fue eocntrado.");

            return ListClassroomTypesCommand.MapToDto(updated);
        }
    }
}
