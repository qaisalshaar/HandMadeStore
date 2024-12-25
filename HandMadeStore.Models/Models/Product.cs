using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Models.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter product name"), StringLength(50)]
        [Display(Name="Product Name")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Please Enter product arabic name"), StringLength(50)]
        [Display(Name="Product Arabic Name")]
        public string ArabicName { get; set; }


        [Column(TypeName = "nvarchar(MAX)")]
        [UIHint("tinymce_jquery_full")]
       // [Required(ErrorMessage = "Please Enter Description"), StringLength(10000)]
        [Display(Name="Product Description")]
        public string Description { get; set; }


        [Column(TypeName = "nvarchar(MAX)")]
        [UIHint("tinymce_jquery_full")]
       // [Required(ErrorMessage = "Please Enter Arabic Description"), StringLength(10000)]
        [Display(Name="Arabic Product Description")]
        public string ArabicDescription { get; set; }





        [Required(ErrorMessage = "Please Enter product price"), Range(1, Double.PositiveInfinity)]
        public double? Price { get; set; }


        [Required(ErrorMessage = "Please Enter product Price10plus"), Range(1, Double.PositiveInfinity), Display(Name="Price from 11-30")]
        public double? Price10plus { get; set; }

        [Required(ErrorMessage = "Please Enter product Price30plus"), Range(1, Double.PositiveInfinity), Display(Name="Price from 31+")]
        public double? Price30plus { get; set; }
        // Create One To many Relationship
        [Required(ErrorMessage = "Please Enter category name"), Display(Name="Category Name")]
        // Expilict to define forrinkey 
       
        public int CategoryId { get; set; }
        // Principal entity ( Category )to Depended Entity  ( Product )
        public Category Category { get; set; }

        // Create One To many Relationship
        [Required(ErrorMessage = "Please Enter Brand Name"), Display(Name="Brand Name")]
       
        // Expilict to define forrinkey 
        public int BrandId { get; set; }
        // Principal entity ( Brand )to Depended Entity  ( Product )
        public Brand Brand { get; set; }

        // [Required(ErrorMessage = "Please select a file.")]
        // [DataType(DataType.Upload)]


        [Display(Name="Image")]
        [Required(ErrorMessage = "Please upload an image")]
        public String ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;




    }
}
