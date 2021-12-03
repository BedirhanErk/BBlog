﻿using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Concrete
{
    public class Writer
    {
        [Key]
        public int WriterId { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string Image { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
    }
}
