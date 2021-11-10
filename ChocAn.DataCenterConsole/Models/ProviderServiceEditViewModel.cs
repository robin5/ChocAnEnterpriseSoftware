using System.ComponentModel.DataAnnotations;

namespace ChocAn.DataCenterConsole.Models
{
    public class ProviderServiceEditViewModel
    {
        [Required]
        public decimal Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Cost { get; set; }
    }
}
