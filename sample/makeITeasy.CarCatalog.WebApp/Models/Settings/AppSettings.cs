using System.ComponentModel.DataAnnotations;

namespace makeITeasy.CarCatalog.WebApp.Models.Settings
{
    public class AppSettings
    {
        [Required, Url]
        public string MiscUrl { get; set; }
        [Required]
        public bool MiscBool { get; set; }
        public int MiscInt { get; set; }
    }
}
