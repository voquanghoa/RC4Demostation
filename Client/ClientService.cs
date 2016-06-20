using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	public delegate void RecevedMessage(string originMessage, string decryptedMessage);
	public delegate void SentMessage(string originMessage, string encryptedMessage);

	public delegate void OnError(string message);

	public class ClientService
	{
		private string ip;
		private int port;

		private TcpClient client;
		private RC4Stream stream;
		private Thread thread;
		public event RecevedMessage ReceivedMessage;
		public event SentMessage SentMessage;
		public event OnError OnError;

		public int LocalPort
		{
			get
			{
				if (client != null)
				{
					return ((IPEndPoint)client.Client.LocalEndPoint).Port;
				}
				return 0;
			}
		}

		public ClientService(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
			thread = new Thread(Listen);
		}

		public void StartListen()
		{
			try
			{
				client = new TcpClient(ip, port);
				stream = new RC4Stream(client.GetStream());
				thread.Start();
			}
			catch (Exception ex)
			{
				OnError?.Invoke(ex.Message);
			}
		}

		private void Listen()
		{
			while (client.Connected)
			{
				try
				{
					var readTuple = stream.Read();
					ReceivedMessage?.Invoke(ConvertByteArrayToString(readTuple.Item1), readTuple.Item2);
				}
				catch (ThreadAbortException) { }
				catch (Exception ex)
				{
					OnError?.Invoke(ex.Message);
				}
				
			}
		}

		public void Send(string message)
		{
			var readTuple = stream.Send(message);
			SentMessage?.Invoke(readTuple.Item1, ConvertByteArrayToString(readTuple.Item2));
		}

		public void Stop()
		{
			if (thread.IsAlive)
			{
				thread.Abort();
			}
		}

		private string ConvertByteArrayToString(byte[] data)
		{
			return string.Join(" ",data.Select(x => x.ToString("X2")).ToArray());
		}
	}
}
