using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

class TcpJsonClient
{
	static void Main(string[] args)
	{
		try
		{
            TcpClient client = new TcpClient("127.0.0.1", 8888); // Opretter  en TCP forbindelse til sever, IP-adresse og port
            NetworkStream ns = client.GetStream(); // Opret en netværkstream der kan sende og modtage data via netværket fra TCP-forbindelsen.
            StreamReader reader = new StreamReader(ns);// Opret en veriable=reader til at modtage tekstdata fra netværksstrømmen.
            StreamWriter writer = new StreamWriter(ns) { AutoFlush = true }; //opretter enveriable=writer til at sende tekstdata oer nætværkstream
                                                                             //AutoFlush =True (gør at data sendes med det samme)

            string command = "";// Initialiserer en tom streng variabel til at gemme brugerens kommandoer.

            Console.WriteLine("Connected to server. Type 'stop' to disconnect.");
            
            // Fortsæt med at sende kommandoer indtil brugeren skriver "stop"
            while (command.ToLower() != "stop")
			{
				// Spørg brugeren om kommandoen
				Console.Write("Enter command (Random, Add, Subtract, Stop): ");
				command = Console.ReadLine();//læser hvad og gemmer ned hvad brugeren bruger af command
				if (command.ToLower() == "stop")
					break; // Afslut forbindelsen, hvis brugeren siger stop"

				// Spørg brugeren om to tal
				Console.Write("Enter first number: ");
				int num1 = int.Parse(Console.ReadLine()); //konventere brugeren svar til int
				Console.Write("Enter second number: ");
				int num2 = int.Parse(Console.ReadLine());//konventere brugerens sar til int

				
				var request = new JsonRequest//opretter et json objeckt med de intastede date(command, num1, num2)
				{
					Method = command,
					Tal1 = num1,
					Tal2 = num2
					/// så inde i det her opject (Request) bliver de informationer gemt 
				};

				// Send JSON-forespørgsel til serveren
				string jsonRequest = JsonSerializer.Serialize(request);//omdanner jsonobjektet til en streng
				writer.WriteLine(jsonRequest);//writer sender jsonRequest til server

				// Modtag JSON-svar fra serveren
				string jsonResponse = reader.ReadLine();//reader læser serverens svar
				var response = JsonSerializer.Deserialize<JsonResponse>(jsonResponse);//omdanner svaret fra Json-format/json-strengen til et objekt igen

				if (response.Status == "success")//tjekker om serverens svar var en success
				{
					Console.WriteLine($"Result from server: {response.Result}");//hvis ja, udskriver svaret til konsollen
				}
				else//hvis nej
				{
					Console.WriteLine($"Error: {response.Message}");//kommer error besked
				}
			}

			client.Close(); // lukker frbinelse når man skriver stop
			Console.WriteLine("Disconnected from server.");//besked om at forbinelleen er slut
		}
		catch (Exception e)//hvis der sker fejl skal der kastes en execption
		{
			Console.WriteLine($"Error: {e.Message}");//udskrevet en fejl besked
		}
	}
}

public class JsonRequest
{
    // Definerer et JSON-request objekt med tre felter: Method (kommandoen), Tal1 og Tal2 (tallene fra brugeren).
    public string Method { get; set; }
	public int Tal1 { get; set; }
	public int Tal2 { get; set; }
}

public class JsonResponse
{
    // Definerer et JSON-response objekt med tre felter: Status (svarstatus), Result (resultat af operationen) og Message (fejlbesked).
    public string Status { get; set; }
	public string Result { get; set; }
	public string Message { get; set; }
}
