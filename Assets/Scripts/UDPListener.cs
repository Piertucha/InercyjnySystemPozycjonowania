using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
    public RotatorScript rotatorScript;
    
    UdpClient clientData;
    int portData = 54687;
    public int receiveBufferSize = 120000;

    public bool showDebug = false;
    IPEndPoint ipEndPointData;
    private object obj = null;
    private System.AsyncCallback AC;
    byte[] receivedBytes;

    void Start()
    {
        InitializeUDPListener();
    }
    public void InitializeUDPListener()
    {
        ipEndPointData = new IPEndPoint(IPAddress.Any, portData);
        clientData = new UdpClient();
        clientData.Client.ReceiveBufferSize = receiveBufferSize;
        clientData.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
        clientData.ExclusiveAddressUse = false;
        clientData.EnableBroadcast = true;
        clientData.Client.Bind(ipEndPointData);
        clientData.DontFragment = true;
        if (showDebug) Debug.Log("BufSize: " + clientData.Client.ReceiveBufferSize);
        AC = new System.AsyncCallback(ReceivedUDPPacket);
        clientData.BeginReceive(AC, obj);
        Debug.Log("UDP - Start Receiving..");

        byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");
        //clientData.Send(sendBytes,sendBytes.Length, "192.168.173.78", 4210);
        // Piwnica v
        //clientData.Send(sendBytes,sendBytes.Length, "192.168.0.218", 4210);
    }

    void ReceivedUDPPacket(System.IAsyncResult result)
    {
        //stopwatch.Start();
        receivedBytes = clientData.EndReceive(result, ref ipEndPointData);

        ParsePacket();

        clientData.BeginReceive(AC, obj);

        //stopwatch.Stop();
        //Debug.Log(stopwatch.ElapsedTicks);
        //stopwatch.Reset();
    } // ReceiveCallBack

    void ParsePacket()
    {
        // work with receivedBytes
        //Debug.Log("receivedBytes len = " + receivedBytes.Length);
        var str = System.Text.Encoding.Default.GetString(receivedBytes);
        
        rotatorScript.GetMessageFromHardware(str);
        //Debug.Log("UDP Message: " + str);
    }

    void OnDestroy()
    {
        if (clientData != null)
        {
            clientData.Close();
        }

    }
}