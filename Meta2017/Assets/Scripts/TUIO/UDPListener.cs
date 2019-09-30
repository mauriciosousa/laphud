using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

public class UDPListener : MonoBehaviour {

	public int port;

	private UdpClient _udpClient;
	private IPEndPoint _anyIP;
	private List<string> _stringsToParse;

	void Start () {
		Debug.Log("[UDP Listener] Start");
		udpRestart();
	}

	public void udpRestart()
	{
		_stringsToParse = new List<string>();
		_anyIP = new IPEndPoint(IPAddress.Any, port);
		_udpClient = new UdpClient(_anyIP);

		_udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
	}

	public void ReceiveCallback(IAsyncResult ar)
	{
		Byte[] receiveBytes = _udpClient.EndReceive(ar, ref _anyIP);
		_stringsToParse.Add(Encoding.ASCII.GetString(receiveBytes));

		print (Encoding.UTF32.GetString(receiveBytes));

		_udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
	}

	void Update () 
	{
		while(_stringsToParse.Count > 0)
		{
			string stringToParse = _stringsToParse.First();
			_stringsToParse.RemoveAt(0);

			Debug.Log (stringToParse);

		}
	}

	void OnApplicationQuit()
	{
		//_udpClient.Close();
	}

	void OnQuit()
	{
		OnApplicationQuit();
	}
}
