using System;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Server
{
	public class Waiter
	{
		public event ReciveMessage ReceiveMessage;
		public event ClientDisconned ClientDisconned;

		private TcpClient tcpClient;

		private NetworkStream stream;

		public Waiter(TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			stream = tcpClient.GetStream();
		}

		public void Start()
		{
			new Thread(Listen).Start();
		}

		private void Listen()
		{
			var buffer = new byte[1024 * 10];

			while (tcpClient.Connected)
			{
				try
				{
					var length = stream.Read(buffer, 0, buffer.Length);
					if (length > 0)
					{
						var data = buffer.Take(length).ToArray();

						ReceiveMessage?.Invoke(this, data);
					}
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				
			}
			ClientDisconned?.Invoke(this);
			stream.Close();
			tcpClient.Close();
		}

		public void SendData(byte[] data)
		{
			stream.Write(data, 0, data.Length);
			stream.Flush();
		}
	}
}
