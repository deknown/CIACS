using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Windows.Forms;
using CIACS;

namespace CIACS_win_client
{
	public partial class Form1 : Form
	{
		static ServerInfo server = null;
		static string ip = null;
		bool connected = false;
		public Form1()
		{
			InitializeComponent();

			ip = Program.GetServerIP();

			if (ip == null) textBox2.Enabled = true;
			else
			{
				textBox2.Text = ip;
				FormSwitch();
			}
		}
		void FormSwitch()
		{
			try
			{
				server = Program.ReceiveLocation();
				if (server == null || server.locations == null || server.locations.Count == 0) throw new Exception();
				else
				{
					connected = true;
					textBox1.Enabled = true;
					textBox2.Enabled = false;
				}
			}
			catch
			{
				AlertMessage("Can't load locations, make sure server IP address is correct");
				return;
			}
			List<Location> locations = server.locations;
			treeView1.Enabled = true;
			treeView1.Nodes.AddTreeLocations(locations);

			if (server.id != null && server.id != "")
			{
				button1.Text = "Update information";
				textBox1.Text = server.id;
				if (server.location != 0)
				{
					TreeNode find = treeView1.Nodes.FindNodeByTag(server.location);
					if (find != null)
					{
						treeView1.SelectedNode = find;
						while (find != null)
						{
							find.Expand();
							find = find.Parent;
						}
					}
				}
			}
			else button1.Text = "Add computer";
		}
		private void button1_Click(object sender, EventArgs e)
		{
			if (connected == false)
			{
				if (textBox2.TextLength > 0)
				{
					ip = textBox2.Text;
					Program.WriteIP(ip);
					FormSwitch();
				}
			}
			else if (textBox1.TextLength > 0 && treeView1.SelectedNode != null)
			{
				ClientInfo client = new ClientInfo()
				{
					id = textBox1.Text,
					location = (int)treeView1.SelectedNode.Tag
				};
				try
				{
					if (client.id == server.id && client.location == server.location)
					{
						if (AnswerMessage("No changes detected. Or you may want update your computer preferences?")) Program.RequestUpdate();
						else return;
					}
					else
					{
						string JSON = Program.SerializeJSON(client);
						Program.UpdateLocation(JSON);
					}
				}
				catch (Exception ex)
				{
					FailMessage("Can't establish connection to server", ex);
				}
				SuccessMessage("Request has sent");
			}
			else AlertMessage("Please ensure, that all fields are filled");
		}
		private void SuccessMessage(string message)
		{
			MessageBox.Show(message, "Success!");
		}
		private void FailMessage(string message, Exception ex)
		{
			MessageBox.Show(message + ".\nPlease, check logs and contact to administrator.", "Fail!");
			Application.Exit();
			throw ex;
		}
		private void AlertMessage(string message)
		{
			MessageBox.Show(message, "Alert!");
		}
		private bool AnswerMessage(string message)
		{
			DialogResult result = MessageBox.Show(message, "Fail!", MessageBoxButtons.YesNo);
			if (result == DialogResult.Yes) return true;
			return false;
		}
	}
}