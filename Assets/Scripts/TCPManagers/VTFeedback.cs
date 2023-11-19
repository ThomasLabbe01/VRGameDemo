using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class VTFeedback : MonoBehaviour
{
    private string serverIP = "127.0.0.1";
    private int serverPort = 12348;

    // read Thread
    Thread readThread;
    // udpclient object
    private TcpClient client;
    private NetworkStream stream;
    
    private static VTFeedback playerInstance;

    void Awake() 
    {
        DontDestroyOnLoad (this);
         
        if (playerInstance == null) {
            playerInstance = this;
        } else {
            DestroyObject(gameObject);
        }
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();
            Debug.Log("Connected to server.");
            Write(false);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to connect to the server: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        DisconnectFromServer();
    }

    // Stop reading UDP messages
    private void DisconnectFromServer()
    {
        if (client != null)
        {
            stream.Close();
            client.Close();
            Debug.Log("Disconnected from server.");
        }
    }

    public void Write(bool _isOn)
    {
        try
        {
        string message = "";
        if (_isOn) {
            message = "1";
        } else {message = "0";}

        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        Debug.Log("Sent message to server: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending data: " + e.Message);
        }
    }
}
