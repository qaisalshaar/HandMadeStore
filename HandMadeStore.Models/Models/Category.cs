
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace HandMadeStore.Models.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter category name"), StringLength(50)]
        [Display( Name ="Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter category Arabic name"), StringLength(50)]
        [Display( Name="Category Arabic Name")]
        public string ArabicName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
