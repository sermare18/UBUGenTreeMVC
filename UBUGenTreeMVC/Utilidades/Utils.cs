using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace UBUGenTreeMVC.Models
{
    public class Utils
    {
        /// <summary>
        /// Encripta una cadena.
        /// </summary>
        /// <param name="cadena">Cadena a cifrar.</param>
        /// <returns>Cadena cifrada.</returns>
        public static string Encriptar(string cadena)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(cadena);
            SHA256 mySHA256 = SHA256.Create();
            bytes = mySHA256.ComputeHash(bytes);
            return (System.Text.Encoding.ASCII.GetString(bytes));
        }

        /// <summary>
        /// Detrmina el nivel e complegidad de una cadena usada como contraseña.
        /// </summary>
        /// <param name="cadena">Contraseña a evaluar.</param>
        /// <returns>Valor numérico de 0 a 4 siendo cero si la longitud es inferior a .
        /// Indica los juegos de caracteres distintos inluidos:
        /// Letras minúsculas, mayúsculas, numeros o caracteres especiales/signos de puntuación</returns>
        public static int NivelComplejidad(string cadena)
        {
            int nivel = 0;
            int minusculas = 0;
            int mayusculas = 0;
            int digitos = 0;
            int especiales = 0;
            int puntuacion = 0;
            int longitud = cadena.Length; ;
            foreach (char c in cadena)
            {
                if (char.IsUpper(c))
                    mayusculas += 1;
                else if (char.IsLower(c))
                    minusculas += 1;
                else if (char.IsDigit(c))
                    digitos += 1;
                else if (char.IsSymbol(c) || char.IsPunctuation(c))
                    especiales += 1;
            }
            nivel += mayusculas > 0 ? 1 : 0;
            nivel += minusculas > 0 ? 1 : 0;
            nivel += digitos > 0 ? 1 : 0;
            nivel += especiales > 0 ? 1 : 0;
            nivel = nivel * (longitud > 7 ? 1 : 0);
            return nivel;
        }
        /// <summary>
        /// Comprueba si una cadena se corresponde con el formato de un EMail.
        /// </summary>
        /// <param name="cadena">Cadena a comprobar.</param>
        /// <returns>Valor lógico que indica si se corresponde con un EMail o no.</returns>
        public static bool EsEMail(string cadena)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(cadena, pattern);
        }
    }
}

