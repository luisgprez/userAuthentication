using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserAuthentication.Models.DB;

[Index("UserName", Name = "UQ__Users__C9F28456FB90CB58", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string UserName { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    public int? FailedLoginCount { get; set; }

    public bool? Locked { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateLockedEnd { get; set; }
    public DateTime? Created { get; set; }
    public int EstatusId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();
}
