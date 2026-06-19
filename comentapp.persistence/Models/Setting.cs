

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace comentapp.persistence.Models
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
