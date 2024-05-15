﻿using System.ComponentModel.DataAnnotations;

namespace ActionTrakingSystem.Model
{
    public class ModelEquipmentType
    {
        [Key]
        public int typeId { get; set; }
        public string typeTitle { get; set; }
        public int isDeleted { get; set; }
    }
}
