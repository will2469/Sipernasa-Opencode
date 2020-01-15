using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sipernasa
{
    public static class TerbilangHelper
    {
        public static string Terbilang(int x)
        {
            string[] bilangan = { "", "SATU", "DUA", "TIGA", "EMPAT", "LIMA", "ENAM", "TUJUH", "DELAPAN", "SEMBILAN", "SEPULUH", "SEBELAS" };
            string temp = string.Empty;

            if (x < 12)
            {
                temp = " " + bilangan[x];
            }
            else if (x < 20)
            {
                temp = Terbilang(x - 10) + " BELAS";
            }
            else if (x < 100)
            {
                temp = Terbilang(x / 10) + " PULUH" + Terbilang(x % 10);
            }
            else if (x < 200)
            {
                temp = " SERATUS" + Terbilang(x - 100);
            }
            else if (x < 1000)
            {
                temp = Terbilang(x / 100) + " RATUS" + Terbilang(x % 100);
            }
            else if (x < 2000)
            {
                temp = " SERIBU" + Terbilang(x - 1000);
            }
            else if (x < 1000000)
            {
                temp = Terbilang(x / 1000) + " RIBU" + Terbilang(x % 1000);
            }
            else if (x < 1000000000)
            {
                temp = Terbilang(x / 1000000) + " JUTA" + Terbilang(x % 1000000);
            }

            return temp;
        }
    }
}
