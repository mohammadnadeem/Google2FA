using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserApi.Models
{
    [Table("Users")]
    public class User
    {
        [Column("UserId")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int UserId { get; set; }

        [Column("Username")]
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Column("Password")]
        [Required]
        [StringLength(20)]
        public string Password { get; set; }

        [Column("TwoFAStatus")]
        [Required]
        public bool TwoFAStatus { get; set; }
    }
}
