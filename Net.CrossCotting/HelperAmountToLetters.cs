using System;
using System.Text;
namespace Net.CrossCotting
{
    public class HelperAmountToLetters
    {
        public static string AmountToLetters(decimal amount, string moneda = "Dolares americanos")
        {
            long parteEntera = (long)Math.Floor(amount);
            int parteDecimal = (int)Math.Round((amount - parteEntera) * 100, 0);

            string letras = ConvertirEntero(parteEntera).Trim();

            return $"SON : {letras} CON {parteDecimal:00}/100 {moneda}";
        }

        private static string ConvertirEntero(long amount)
        {
            if (amount == 0) return "CERO";

            if (amount < 0)
                return "MENOS " + ConvertirEntero(Math.Abs(amount));

            StringBuilder sb = new();

            if (amount >= 1_000_000)
            {
                sb.Append(amount / 1_000_000 == 1
                    ? "UN MILLON "
                    : ConvertirEntero(amount / 1_000_000) + " MILLONES ");
                amount %= 1_000_000;
            }

            if (amount >= 1000)
            {
                sb.Append(amount / 1000 == 1
                    ? "MIL "
                    : ConvertirEntero(amount / 1000) + " MIL ");
                amount %= 1000;
            }

            if (amount >= 100)
            {
                sb.Append(Centenas(amount / 100));
                amount %= 100;
            }

            if (amount > 0)
                sb.Append(Decenas(amount));

            return sb.ToString();
        }

        private static string Centenas(long amount)
        {
            return amount switch
            {
                1 => "CIENTO ",
                2 => "DOSCIENTOS ",
                3 => "TRESCIENTOS ",
                4 => "CUATROCIENTOS ",
                5 => "QUINIENTOS ",
                6 => "SEISCIENTOS ",
                7 => "SETECIENTOS ",
                8 => "OCHOCIENTOS ",
                9 => "NOVECIENTOS ",
                _ => ""
            };
        }

        private static string Decenas(long amount)
        {
            return amount switch
            {
                < 10 => Unidades(amount),
                < 20 => Especiales(amount),
                < 30 => amount == 20 ? "VEINTE " : "VEINTI" + Unidades(amount % 10),
                < 40 => "TREINTA" + ConY(amount),
                < 50 => "CUARENTA" + ConY(amount),
                < 60 => "CINCUENTA" + ConY(amount),
                < 70 => "SESENTA" + ConY(amount),
                < 80 => "SETENTA" + ConY(amount),
                < 90 => "OCHENTA" + ConY(amount),
                < 100 => "NOVENTA" + ConY(amount),
                _ => ""
            };
        }

        private static string ConY(long amount)
        {
            return amount % 10 == 0 ? " " : " Y " + Unidades(amount % 10);
        }

        private static string Especiales(long amount)
        {
            return amount switch
            {
                10 => "DIEZ ",
                11 => "ONCE ",
                12 => "DOCE ",
                13 => "TRECE ",
                14 => "CATORCE ",
                15 => "QUINCE ",
                16 => "DIECISEIS ",
                17 => "DIECISIETE ",
                18 => "DIECIOCHO ",
                19 => "DIECINUEVE ",
                _ => ""
            };
        }

        private static string Unidades(long amount)
        {
            return amount switch
            {
                1 => "UNO ",
                2 => "DOS ",
                3 => "TRES ",
                4 => "CUATRO ",
                5 => "CINCO ",
                6 => "SEIS ",
                7 => "SIETE ",
                8 => "OCHO ",
                9 => "NUEVE ",
                _ => ""
            };
        }
    }
}
