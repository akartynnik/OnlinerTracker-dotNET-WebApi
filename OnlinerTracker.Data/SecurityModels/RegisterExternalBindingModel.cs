using System.ComponentModel.DataAnnotations;

namespace OnlinerTracker.Data.SecurityModels
{
    public class RegisterExternalBindingModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }

        [Required]
        public string UserId { get; set; }

    }
}
