using unhsjm_asistencia_backend.Commands.Abstractions;
using unhsjm_asistencia_backend.Data;
using unhsjm_asistencia_backend.DTOs.ClassroomTypes;
using unhsjm_asistencia_backend.Entities;
using unhsjm_asistencia_backend.Repositories.Interfaces;

namespace unhsjm_asistencia_backend.Commands.ClassroomTypes
{
    public sealed class CreateClassroomTypeCommand : TransactionalCommandBase<ClassroomTypeDto>
    {
        private readonly IClassroomTypeRepository _repository;
        private CreateClassroomTypeDto? _request;

        public CreateClassroomTypeCommand(IDapperRlsContext dbContext, IClassroomTypeRepository repository)
            : base(dbContext)
        {
            _repository = repository;
        }

        public CreateClassroomTypeCommand WithRequest(CreateClassroomTypeDto request)
        {
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
            var created = await _repository.CreateAsync(
                new ClassroomTypeEntity
                {
                    Code = dto.Code.Trim(),
                    Name = dto.Name.Trim(),
                    Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                    IsActive = dto.IsActive
                },
                cancellationToken);

            return ListClassroomTypesCommand.MapToDto(created);
        }
    }
}
