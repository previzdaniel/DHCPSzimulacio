using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DHCPSzimulacio
{
    class Program
    {
        static List<string> excluded = new List<string>();
        static Dictionary<string, string> reserved = new Dictionary<string, string>();
        static Dictionary<string, string> dhcp = new Dictionary<string, string>();
        static List<string> commands = new List<string>();

        static void BeolvasList(List<string> l, string filenev)
        {
            try
            {
                StreamReader file = new StreamReader(filenev);
                try
                {
                    while (!file.EndOfStream)
                    {
                        l.Add(file.ReadLine());
                    }

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                finally
                {
                    file.Close();
                }
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void BeolvasDictionary(Dictionary<string, string> d, string filenev)
        {
            try
            {
                StreamReader file = new StreamReader(filenev);
                while (!file.EndOfStream)
                {
                    string[] adatok = file.ReadLine().Split(';');
                    d.Add(adatok[0], adatok[1]);
                }
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string CimEgyelNo(string cim)
        {
            /*
            cim = "192.168.10.100"
            return "192.168.10.101"

            szétvágni"."
            az utolsót inté konvertálni
            egyet hozzáadni (255 ne lépjük túl)

            összefűzni stringé
            */
              string[] adatok = cim.Split('.');
              int okt4 = Convert.ToInt32(adatok[3]);
              if (okt4 < 255)
              {
                   okt4++;
              }
            return adatok[0] + "." + adatok[1] + "." + adatok[2] + "." + okt4.ToString();
        }

        static void Feladat(string parancs)
        {
            /* parancs = "request;D19313570A82"
             * először csak request paranccsal foglalkozunk
             * 
             * x Megnézzük hogy request-e?
             * ki kell szedni a mac-címet a parancsból
             */
            
            if (parancs.Contains("request"))
            {
                string[] a = parancs.Split(';');
                string mac = a[1];

                if (dhcp.ContainsKey(mac))
                {
                    Console.WriteLine($"{mac} --> {dhcp[mac]}");
                }
                else
                {
                    if (reserved.ContainsKey(mac))
                    {
                        Console.WriteLine($"Res. {mac} --> {reserved[mac]}");
                        dhcp.Add(mac, reserved[mac]);
                    }
                    else
                    {
                        string indulo = "192.168.10.100";
                        int okt4 = 100;

                        while (okt4 < 200 && (dhcp.ContainsValue(indulo)
                                          || reserved.ContainsValue(indulo)
                                          || excluded.Contains(indulo)))
                        {
                            okt4++;
                            indulo = CimEgyelNo(indulo);
                        }
                        if (okt4 < 200)
                        {
                            Console.WriteLine($"Kiosztott {mac} --> {indulo}");
                            dhcp.Add(mac, indulo);
                        }
                        else
                        {
                            Console.WriteLine($"{mac} nincs IP!");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Nem ok");
            }
        }

        static void Feladatok()
        {
            foreach (var command in commands)
            {
                Feladat(command);
            }
        }

        static void Main(string[] args)
        {
            #region Beolvasások
            BeolvasList(excluded, "excluded.csv");
            BeolvasList(commands, "test.csv");
            BeolvasDictionary(dhcp, "dhcp.csv");
            BeolvasDictionary(reserved, "reserved.csv");
            #endregion

            Feladatok();

            Console.WriteLine("\nvége...");

            Console.ReadKey();
        }
    }
}
