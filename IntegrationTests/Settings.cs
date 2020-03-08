using System.ComponentModel.DataAnnotations;

namespace IntegrationTests
{
    public class Settings
    {
        [Required]
        [MinLength(5)]
        public string UserName { get; set; }

        [Required]
        [MinLength(5)]
        public string UserPassword { get; set; }
    }
}
