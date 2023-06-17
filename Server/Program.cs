using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;

namespace Server
{
    internal class Program
    {
        private static Dictionary<string, string> capitals = new Dictionary<string, string>();

        private static string FindCapital(string country)
        {
            try
            {
                return capitals[country];
            }
            catch (KeyNotFoundException)
            {
                return "Country not found in database";
            }
        }

        private static string FindCountry(string country)
        {
            try
            {
                return capitals.Single(c => c.Value == country).Key;
            }
            catch (InvalidOperationException)
            {
                return "Capital not found in database";
            }
        }

        private static void Main(string[] args)
        {
            using (FileStream fs = new FileStream(
                @"C:\Users\maxik\source\repos\NetworkProgrammingHW_UdpProtocol\Server\Capitals.json",
                FileMode.Open, FileAccess.Read))
            {
                capitals = JsonSerializer.Deserialize(fs, typeof(Dictionary<string, string>))
                    as Dictionary<string, string> ?? capitals;
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.7.150.41"), 1234);
            UdpClient server = new UdpClient(endPoint);

            while (true)
            {
                IPEndPoint? clientEndPoint = null;
                Console.WriteLine("Waiting for the request...");
                byte[] request = server.Receive(ref clientEndPoint);

                string message = Encoding.UTF8.GetString(request);
                if (message == "END")
                    continue;
                Console.WriteLine($"Received message: {message} : {DateTime.Now.ToShortTimeString()} from: {clientEndPoint}");

                string response;
                if (message.StartsWith("Country"))
                    response = FindCapital(message.Substring(8));
                else if (message.StartsWith("Capital"))
                    response = FindCountry(message.Substring(8));
                else
                    response = "Invalid input";

                byte[] response_bytes = Encoding.UTF8.GetBytes(response);
                server.Send(response_bytes, response_bytes.Length, clientEndPoint);
            }
        }
    }
}