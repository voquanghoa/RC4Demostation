using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
	public partial class ClientForm : Form
	{
		private ClientService clientService;

		public ClientForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			clientService = new ClientService("127.0.0.1", 8000);

			clientService.OnError += ClientService_OnError;
			clientService.ReceivedMessage += ClientService_ReceivedMessage;
			clientService.SentMessage += ClientService_SentMessage;
			clientService.StartListen();
			Text += " - " + clientService.LocalPort;
		}

		private void ClientService_SentMessage(string originMessage, string encryptedMessage)
		{
			txtLog.AppendText($"\r\nMessage: {originMessage}\r\n");
			txtLog.AppendText($"Sent: {encryptedMessage}\r\n");
			txtLog.AppendText($"----------------------------\r\n");
		}

		private void ClientService_OnError(string message)
		{
			Action action = () =>
			{
				if (!IsDisposed)
				{
					MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					Close();
				}
			};
			
			Invoke(action);
		}

		private void ClientService_ReceivedMessage(string originMessage, string descriptedMessage)
		{
			Action appendText = () => 
			{

				txtLog.AppendText($"\r\nReceved: {originMessage}\r\n");
				txtLog.AppendText($"Message: {descriptedMessage}\r\n");
				txtLog.AppendText($"----------------------------\r\n");
			};
			Invoke(appendText);
		}

		private void btSend_Click(object sender, EventArgs e)
		{
			if (txtMessage.TextLength > 0)
			{
				clientService.Send(txtMessage.Text);
				txtMessage.Text = string.Empty;
			}
		}

		private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			clientService?.Stop();
		}
	}
}
