using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace repo.Models
{
    public class Auth
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string login { get; set; }

        [Required]
        public required string password { get; set; }

        [Required]
        public bool isAdmin { get; set; }

        // Пустой конструктор для Entity Framework
        public Auth() { }

        // Твой конструктор для создания объектов в коде
        [SetsRequiredMembers]
        public Auth(int id, string login, string password, bool isAdmin) 
        {
            this.Id = id;
            this.login = login;
            this.password = password;
            this.isAdmin = isAdmin;
        }
    }
}