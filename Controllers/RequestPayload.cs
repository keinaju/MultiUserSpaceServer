using System.ComponentModel.DataAnnotations;

namespace MUS.Controllers
{
    public class RequestPayload
    {
        [MinLength(0)]
        [MaxLength(500)]
        public string UserInput { get; set; }
        
        [MinLength(0)]
        [MaxLength(500)]
        public string Token { get; set; }
    }
}
