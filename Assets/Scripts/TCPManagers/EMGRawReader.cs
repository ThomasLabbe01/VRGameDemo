using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;

public class EMGRawReader : MonoBehaviour
{
    private string IP = "127.0.0.1";
    private int port = 12346;
    private int sendPort = 12347;
    public string readVal = "";
    public string timestamp = "";
    public float velocity;
    public List<int> receivedData; 

    public int EMG_prediction;
    public float EMG_accuracy;

    private void Start()
    {
        receivedData = new List<int>(Enumerable.Repeat(0, 64));
    }

    // read Thread
    Thread readThread;
    // udpclient object
    UdpClient client;
    UdpClient server;
    IPEndPoint serverTarget;
    
    private static EMGRawReader playerInstance;

    void Awake() 
    {
        DontDestroyOnLoad (this);
         
        if (playerInstance == null) {
            playerInstance = this;
        } else {
            DestroyObject(gameObject);
        }
        StartReadingData(); //Somethign weird happening
    }

    public void StartReadingData()
    {
        // create thread for reading UDP messages
        readThread = new Thread(new ThreadStart(ReceiveData));
        readThread.IsBackground = true;
        readThread.Start();
        server = new UdpClient(sendPort);
        serverTarget = new IPEndPoint(IPAddress.Parse(IP), sendPort);
    }

    // Unity Application Quit Function
    void OnApplicationQuit()
    {
        stopThread();
    }

    // Stop reading UDP messages
    public void stopThread()
    {
        if (readThread.IsAlive)
        {
            readThread.Abort();
        }
        server.Close();
        client.Close();
    }

    // receive thread function
    //HERE
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                // receive bytes
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] buff = client.Receive(ref anyIP);

                // encode UTF8-coded bytes to text format
                string text = Encoding.UTF8.GetString(buff);
                //Debug.Log(text);

                // Split the received data
                string[] splitData = text.Split(' ');
                //Debug.Log(splitData[2]);
                // Extract EMG prediction and accuracy
                EMG_prediction = int.Parse(splitData[0]);

                //Debug.Log(EMG_prediction);
                EMG_accuracy = float.Parse(splitData[1], CultureInfo.InvariantCulture);

                //Debug.Log(EMG_accuracy);

                // Extract and parse the rest of the data as integers into a list
                string[] rawInputs = splitData[2].Split(';');
                //Debug.Log(rawInputs);
                 
                for (int i = 0; i < rawInputs.Length; i++)
                {
                    if (int.TryParse(rawInputs[i], out int intValue))
                    {
                        receivedData[i] = intValue;
                    }
                    else
                    {
                        Debug.LogError("Failed to parse input as integer: " + rawInputs[i]);
                    }
                }

            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
                // Handle the exception appropriately for your use case
            }
        }
    }

    public void Write(string strMessage) 
    {
        byte[] arr = System.Text.Encoding.UTF8.GetBytes(strMessage);
        server.Send(arr, arr.Length, serverTarget);
    }
}
