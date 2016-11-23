using weather.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weather
{
    // Objaśnienie jednostek http://openweathermap.org/weather-data
    // Znaki specjalne w konsoli http://www.unicode.org/charts/charindex.html - dodać przed każdym \x np. \x00B0

    class Program
    {
        public static ConsoleColor DefaultColor = Console.ForegroundColor;
        public static ConsoleColor DefaultBgColor = Console.BackgroundColor;

        static void Main(string[] args)
        {
            TryIt();
            Console.ReadLine();
        }

        static void TryIt()
        {
            Console.WriteLine("Podaj lokalizację, aby sprawdzić dla niej pogodę np. Gdańsk");
            string city = Console.ReadLine();
            string api = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}", city);

            string content = Http.Get(api);

            var a = JSON.Parse(content);
            if (a["cod"] != null)
            {
                if (a["cod"].AsInt == 200)
                {
                    Console.WriteLine(Environment.NewLine);

                    Console.WriteLine("============================================================");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("                 SERWIS POGODOWY                            ");
                    Console.ForegroundColor = DefaultColor;
                    Console.WriteLine("============================================================");

                    Console.WriteLine("Koordynaty: lon:{0}; lat:{1}", a["coord"]["lon"].Value, a["coord"]["lat"].Value);
                    Console.WriteLine("Kraj: {0}; {1}{2}", a["sys"]["country"].Value, a["name"].Value, Environment.NewLine);

                    Console.WriteLine("Ogólne warunki pogodowe: {0}{1}", a["weather"][0]["main"].Value, Environment.NewLine);

                    double temp = Math.Round(double.Parse(a["main"]["temp"].Value.ToString().Replace('.', ',')));
                    double tmin = Math.Round(K2C(double.Parse(a["main"]["temp_min"].Value.ToString().Replace('.', ','))));
                    double tmax = Math.Round(K2C(double.Parse(a["main"]["temp_max"].Value.ToString().Replace('.', ','))));

                    Console.Write("Temperatura {0}\x00B0C (", K2C(temp));
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("{0}\x00B0C", tmax);
                    Console.ForegroundColor = DefaultColor;
                    Console.BackgroundColor = DefaultBgColor;
                    Console.Write("/");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write("{0}\x00B0C", tmin);
                    Console.ForegroundColor = DefaultColor;
                    Console.BackgroundColor = DefaultBgColor;
                    Console.WriteLine(")");

                    Console.WriteLine("Ciśnienie {0}hPa", a["main"]["pressure"].Value);
                    Console.WriteLine("Wilgotność {0}%", a["main"]["humidity"].Value);

                    Console.WriteLine(Environment.NewLine);

                    double wind = Mps2Kph(double.Parse(a["wind"]["speed"].Value.ToString().Replace('.', ',')));
                    Console.WriteLine("Wiatr: {0}km/h", wind);

                    Console.WriteLine("============================================================");
                }
                else
                {
                    Console.WriteLine("Błędna lokalizacja, spróbuj jeszcze raz...");
                    TryIt();
                }
            }
            else
            {
                Console.WriteLine("Błędna lokalizacja, spróbuj jeszcze raz...");
                TryIt();
            }
        }

        static double K2C(double k)
        {
            return k - 273.15;
        }

        static double Mps2Kph(double mps)
        {
            return (mps * 3.6);
        }
    }
}
