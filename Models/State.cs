using System.ComponentModel.DataAnnotations;

namespace BasePackageModule2.Models
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name ="City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        public string Name { get; set; }
    }
}
