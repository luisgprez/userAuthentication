using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UserAuthentication.Models.DB;

[Table("TokenBlacklist")]
public partial class TokenBlacklist
{
    [Key]
    public int TokenBlackListId { get; set; }

    public string Token { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Expiration { get; set; }
}
