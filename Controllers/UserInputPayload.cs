using System.ComponentModel.DataAnnotations;

namespace MUS.Controllers
{
    public class UserInputPayload
    {
        [MinLength(0)]
        [MaxLength(500)]
        public string UserInput { get; set; }
    }
}
