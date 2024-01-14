using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Dtos
{
    public class VillaNumberCreateDTO
    {
        [Required]
        public int VillaNum { get; set; }

        [Required]
        public int VillaID { get; set; }

        public string SpecialDetails { get; set; }
    }
}
