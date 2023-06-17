using PropertyChanged;
using System.Net.Sockets;
using System.Net;
using System.Windows.Input;
using System.Text;

namespace Client
{
    [AddINotifyPropertyChangedInterface]
    internal class ClientViewModel
    {
        public string Country { get; set; } = string.Empty;

        public string Capital { get; set; } = string.Empty;

        private UdpClient client = new UdpClient();

        private IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("10.7.150.41"), 1234);

        private IPEndPoint? serverEndPoint = null;

        private RelayCommand? findCapitalCommand;

        public ICommand FindCapitalCommand => findCapitalCommand ??
            (findCapitalCommand = new RelayCommand(o => FindCapital()));

        private RelayCommand? findCountryCommand;

        public ICommand FindCountryCommand => findCountryCommand ??
            (findCountryCommand = new RelayCommand(o => FindCountry()));

        public async void FindCapital()
        {
            string message = $"Country {Country}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, data.Length, endPoint);
            Capital = Encoding.UTF8.GetString(client.Receive(ref serverEndPoint));
        }

        public async void FindCountry()
        {
            string message = $"Capital {Capital}";
            byte[] data = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(data, data.Length, endPoint);
            Country = Encoding.UTF8.GetString(client.Receive(ref serverEndPoint));
        }
    }
}