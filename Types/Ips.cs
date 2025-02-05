using System.ComponentModel.DataAnnotations;

namespace Aaio.SDK.Types;


public class Ips {
    [Required]
    public required List<string> list { get; set; }
}