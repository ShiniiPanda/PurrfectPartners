﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PurrfectPartners.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    [PersonalData]
    [Column("Name", TypeName = "nvarchar(256)")]
    public string Name { get; set; } = null!;

    [PersonalData]
    public string? Address { get; set; }

    [PersonalData]
    [NotMapped]
    public int Age
    {
        get
        {
            var years = DateTime.UtcNow - DOB;
            return (int)Math.Floor(years.TotalDays / 365);
        }
    }

    [PersonalData]
    public DateTime DOB { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public string GetFirstName()
    {
        if (Name.Contains(' '))
        {
            return Name.Split(' ')[0];
        }
        return Name;
    }


}

