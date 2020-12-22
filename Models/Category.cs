using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    // [Table("Categoria")] //se quiser outro nome para a tabela
    public class Category
    {
        [Key]
        // [Column("Cat_ID")] //caso queira outro nome para a coluna
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        public string Title { get; set; }
    }
}