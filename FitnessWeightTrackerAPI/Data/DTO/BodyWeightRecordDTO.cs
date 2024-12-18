﻿using System.ComponentModel.DataAnnotations;
using FitnessWeightTrackerAPI.Models.CustomValidation;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightRecordDTO
    {
        [Range(0.01, 500.0, ErrorMessage = "Weight must be between 0.01 and 500.")]
        public float Weight { get; set; }

        [DataType(DataType.Date)]
        [DateBeforeNowValidation]
        public DateTime Date { get; set; }
    }
}
