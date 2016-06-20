using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	public delegate void ReciveMessage(Waiter receiver, byte[] data);
	public delegate void ClientDisconned(Waiter disconnecter);

	public class TcpServer
	{
		private List<Waiter> waiters = new List<Waiter>();
		private TcpListener tcpListener;
		private object locker = new object();

		public TcpServer(string ip, int port)
		{
			tcpListener = new TcpListener(IPAddress.Parse(ip), port);
		}

		public void Bind()
		{
			tcpListener.Start();
			Console.WriteLine("Server started");

			while (true)
			{
				var tcpClient = tcpListener.AcceptTcpClient();
				Console.WriteLine("New client");
				var waiter = new Waiter(tcpClient);
				
				waiter.ReceiveMessage += Waiter_ReceiveMessage;
				waiter.ClientDisconned += Waiter_ClientDisconned;

				waiter.Start();

				lock (locker)
				{
					waiters.Add(waiter);
				}
			}
		}

		private void Waiter_ClientDisconned(Waiter disconnecter)
		{
			lock (locker)
			{
				if (waiters.Contains(disconnecter))
				{
					Console.WriteLine("Remove a client");
					waiters.Remove(disconnecter);
				}
			}
		}

		private void Waiter_ReceiveMessage(Waiter receiver, byte[] data)
		{
			lock (locker)
			{
				foreach (var waiter in waiters)
				{
					if(waiter != receiver)
					{
						waiter.SendData(data);
					}
				}
			}
		}
	}
}
