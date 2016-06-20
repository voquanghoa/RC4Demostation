namespace Server
{
	public class Program
	{
		static void Main(string[] args)
		{
			new TcpServer("127.0.0.1", 8000).Bind();
		}
	}
}
