using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace unhsjm_asistencia_backend.Models;

[Table("users", Schema = "auth")]
public partial class AuthUser
{
    public Guid Id { get; set; }

    public string? Email { get; set; }

    public string? EncryptedPassword { get; set; }

    public DateTime? EmailConfirmedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
