using System.ComponentModel.DataAnnotations;

namespace MUS.Controllers
{
    public class RequestPayload
    {
        [MinLength(1)]
        [MaxLength(1000)]
        public string[] Commands { get; set; }
        
        [MinLength(0)]
        [MaxLength(1000)]
        public string Token { get; set; }
    }
}
