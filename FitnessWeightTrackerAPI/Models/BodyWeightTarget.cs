﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessWeightTrackerAPI.Models.CustomValidation;

namespace FitnessWeightTrackerAPI.Models
{
    public class BodyWeightTarget
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Range(0.01, 500.0, ErrorMessage = "Target weight must be between 0.01 and 500.")]
        public float TargetWeight { get; set; }

        [DataType(DataType.Date)]
        [DateAfterNowValidation]
        public DateTime TargetDate { get; set; }
    }
}