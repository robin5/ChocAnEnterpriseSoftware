using System.ComponentModel.DataAnnotations;

namespace ChocAn.DataCenterConsole.Models
{
    public class ProviderEditViewModel
    {
        [Required]
        public decimal Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public decimal ZipCode { get; set; }
    }
}
