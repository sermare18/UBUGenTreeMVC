﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UBUGenTreeMVC.Models
{
    public class Persona
    {
        // Mantendrá su valor entre diferentes objetos de tipo Persona
        [NotMapped]
        public static int contador = 0;

        [Key]
        public int _id;

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Primer apellido")]
        public string Apellido1 { get; set; }

        [Required]
        [Display(Name = "Segundo apellido")]
        public string Apellido2 { get; set; }

        [Required]
        [Display(Name = "Localidad")]
        public string Localidad { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNac { get; set; }

        public DateTime? FechaDef { get; set; } 

        public int? Edad { get; set; }

        // public Persona? A1 { get; set; }

        // public Persona? A2 { get; set; }

        public Persona()
        {
            this._id = contador;
            contador++;
        }

        public override string ToString()
        {
            return $"ID: {_id}, Nombre: {Nombre}, Primer Apellido: {Apellido1}, Segundo Apellido: {Apellido2}, Localidad: {Localidad}, Fecha de Nacimiento: {FechaNac.ToString("yyyy-MM-dd")}, Fecha de Defunción: {(FechaDef.HasValue ? FechaDef.Value.ToString("yyyy-MM-dd") : "N/A")}";
        }
    }
}

