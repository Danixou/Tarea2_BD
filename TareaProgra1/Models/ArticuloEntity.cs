using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TareaProgra1.Models
{
    public class ArticuloEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(128)]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Clase")]
      
        public int IdClaseArticulo { get; set; }

        [Required]

        public decimal Precio { get; set; }

        [Required]
        public bool EsActivo { get; set; }
    }
}
