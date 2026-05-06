using EFCoreApp.Data;
using System.ComponentModel.DataAnnotations;

namespace EFCoreApp.Models
{
    public class KursViewModel
    {
        public int KursId { get; set; }
        [Required]
        [StringLength(50)]
        public string? Baslik { get; set; }
        public int OgretmenId { get; set; }
        public ICollection<KursKayit> KursKayilari { get; set; } = new List<KursKayit>();
    }
}
