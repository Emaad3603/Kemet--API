﻿using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class PlacesDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
     
        public List<string> ImageURLs { get; set;}

    }
}
