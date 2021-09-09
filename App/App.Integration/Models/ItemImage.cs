using App.Integration.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Integration.Models
{
    public class ItemImage
    {
        public ItemImage(ItemImageType itemImageType, string propertyName, string propertyNameLayer2 = null)
        {
            ItemImageType = itemImageType;
            PropertyNameLayer1 = propertyName;
            PropertyNameLayer2 = propertyNameLayer2;
        }

        public ItemImageType ItemImageType { get; set; }

        public string PropertyNameLayer1 { get; set; }

        public string PropertyNameLayer2 { get; set; }
    }
}
