
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace HandMadeStore.Models.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please Enter brand name"), StringLength(50)]
        [DisplayName("Brand Name")]
       
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
