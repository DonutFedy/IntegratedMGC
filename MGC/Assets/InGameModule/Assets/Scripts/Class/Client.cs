  #define DEBUGMODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading;
using System;
using System.Linq;

public class Client
{ 
    TcpClient m_tcp;
    NetworkStream m_stream;
    Thread m_tHandler;
    bool m_bConnected;

    public void Init()
    {
        m_bConnected = false;    
    }

    void InitClient()
    {
        m_tcp = new TcpClient();
        m_stream = default(NetworkStream);
        m_bConnected = true;
    }

    public void Disconnect()
    {
        m_bConnected = false;
        if (m_tcp != null)
        {
            m_tcp.Close();
            m_tcp = null;
        }
        if (m_stream != null)
        {
            m_stream.Close();
            m_stream = null;
        }
        m_tHandler.Abort();
    }

    public void Connect(string serverIP, int nPortNUM)
    {
        if (m_bConnected == true)
            return;
        
        InitClient();
        m_tcp.Connect(serverIP, nPortNUM);
        m_stream = m_tcp.GetStream();
        
        m_tHandler = new Thread(Receive);
        m_tHandler.IsBackground = true;
        m_tHandler.Start();
    }

    public void SendPacket(byte[] packet)
    {
        GameManager.m_Instance.makePacket(packet);

        //if (m_tcp == null)
        //    return;
        //m_stream.Write(packet, 0, packet.Length);
        //m_stream.Flush();
    }

    public void SendMessage(string message)
    {
        if (m_tcp == null)
            return;
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        m_stream.Write(buffer, 0, buffer.Length);
        m_stream.Flush();
    }

    public void Receive()
    {
        try
        {
            while (m_bConnected)
            {
                // 0.01초 간격
                Thread.Sleep(10);

                m_stream = m_tcp.GetStream();
                int nBufSize = m_tcp.ReceiveBufferSize;
                byte[] buffer = new byte[nBufSize];
                int nBytes = m_stream.Read(buffer, 0, buffer.Length);

                Array.Resize(ref buffer, nBytes);

                EventBuffer.m_Instance.PushData(buffer);
            }
        }
        catch (Exception ex)
        {
            EventBuffer.m_Instance.PushErrorMsg(ex.Message); 
        }
    }
}