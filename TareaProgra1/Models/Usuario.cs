using System.ComponentModel.DataAnnotations.Schema;

namespace TareaProgra1.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        [NotMapped]

        public bool MantenerActivo { get; set; }

    }
}
