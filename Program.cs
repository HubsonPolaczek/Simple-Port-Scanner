using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimplePortScanner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Console.Title = "Mój Własny Skaner Portów";
            Console.WriteLine("==========================================");
            Console.WriteLine("    PROSTY SKANER PORTÓW TCP");
            Console.WriteLine("==========================================");

            // 1. Pobieranie adresu od użytkownika
            Console.Write("Podaj adres IP lub nazwę hosta (np. google.com lub 127.0.0.1): ");
            string host = Console.ReadLine();

            // Lista najpopularniejszych portów do sprawdzenia
            // 21-FTP, 22-SSH, 80-HTTP, 443-HTTPS, 3389-RDP (Pulpit zdalny), 53-DNS
            List<int> portsToScan = new List<int> { 21, 22, 25, 53, 80, 443, 8080, 3306, 3389 };

            Console.WriteLine($"\nRozpoczynam skanowanie dla: {host}...\n");

            foreach (int port in portsToScan)
            {
                // Wywołanie funkcji sprawdzającej port
                var status = await CheckPort(host, port);

                if (status)
                {
                    // Ładne kolorowanie wyniku na zielono
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[+] Port {port}: OTWARTY");
                    Console.ResetColor();
                }
                else
                {
                    // Opcjonalnie: można ukryć zamknięte porty, żeby nie śmiecić
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[-] Port {port}: Zamknięty lub filtrowany");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\nSkanowanie zakończone. Naciśnij dowolny klawisz, aby wyjść.");
            Console.ReadKey();
        }

        // Funkcja logiczna sprawdzająca połączenie
        static async Task<bool> CheckPort(string host, int port)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    // Próbujemy się połączyć. Jeśli się uda w ciągu 1 sekundy, zwracamy true.
                    // Używamy ConnectAsync, żeby móc ustawić krótki timeout.
                    var connectTask = client.ConnectAsync(host, port);

                    // Czekamy 1000 milisekund (1 sekunda) na połączenie
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
                    {
                        // Jeśli zadanie połączenia skończyło się pierwsze - sprawdźmy czy nie ma błędu
                        return client.Connected;
                    }
                    else
                    {
                        // Timeout - za długo to trwało
                        return false;
                    }
                }
                catch
                {
                    // Jakikolwiek błąd (np. brak hosta) oznacza brak połączenia
                    return false;
                }
            }
        }
    }
}