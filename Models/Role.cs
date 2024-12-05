using System.ComponentModel.DataAnnotations;

namespace SistemaGestionPersonal.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }

        // Relación con Usuarios
        public ICollection<User> Users { get; set; }
    }
}
