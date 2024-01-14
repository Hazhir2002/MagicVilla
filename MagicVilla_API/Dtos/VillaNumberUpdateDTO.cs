using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Dtos
{
    public class VillaNumberUpdateDTO
    {
        [Required]
        public int VillaNum { get; set; }

        [Required]
        public int VillaID { get; set; }

        public string SpecialDetails { get; set; }
    }
}
