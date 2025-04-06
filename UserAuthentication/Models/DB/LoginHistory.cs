using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserAuthentication.Models.DB;

[Table("LoginHistory")]
public partial class LoginHistory
{
    [Key]
    public int HistoryId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateHistory { get; set; }

    public bool Success { get; set; }

    public required string MessageLogin { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("LoginHistories")]
    public virtual User? User { get; set; }
}
