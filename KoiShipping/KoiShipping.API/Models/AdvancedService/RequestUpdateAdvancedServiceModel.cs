﻿using System.ComponentModel.DataAnnotations;

namespace KoiShipping.API.Models.AdvancedServiceModel
{
    public class RequestUpdateAdvancedServiceModel
    {
        public string ServiceName { get; set; } = null!;

        public decimal Price { get; set; }

        public bool DeleteStatus { get; set; } // Include DeleteStatus for update
    }
}
