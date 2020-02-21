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
using PROTOCOL;
using PACKET;

public class client
{
    TcpClient       m_tcp_loginServer;
    TcpClient       m_tcp_mainServer;
    NetworkStream   m_stream_loginServer;
    NetworkStream   m_stream_mainServer;
    Thread          m_tHandler_loginServer;
    Thread          m_tHandler_mainServer;
    bool            m_bConnected_loginServer = false;
    bool            m_bConnected_mainServer = false;

    const int       nBUFMAX = 8000;

    /// <summary>
    /// 이벤트 push를 위한 델리게이트
    /// 데이터 타입은 나중에 변경요망
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="eventData"></param>
    public delegate void EventHandler(C_BasePacket Event);
    public event EventHandler m_handler_loginServer;
    public event EventHandler m_handler_mainServer;

    void initClient_loginServer()
    {
        m_tcp_loginServer = new TcpClient();
        m_stream_loginServer = default(NetworkStream);
    }
    void initClient_mainServer()
    {
        m_tcp_mainServer = new TcpClient();
        m_stream_mainServer = default(NetworkStream);
    }

    public void disconnect_loginServer()
    {
        if (m_bConnected_loginServer == false)
            return;
        //using (m_stream_loginServer)
        //{
        //    eventData data = new AnomalyData();
        //    byte[] buffer = data.serialize();
        //    m_stream_loginServer.Write(buffer, 0, buffer.Length);
        //    m_stream_loginServer.Flush();
        //}
        m_tHandler_loginServer.Abort();
        clearLoginTcpClient();
    }

    void clearLoginTcpClient()
    {
        //Debug.LogError("clear login tcp");
        if (m_tcp_loginServer != null && m_bConnected_loginServer)
        {
            using (m_stream_loginServer)
            {
                m_stream_loginServer.Dispose();
                m_stream_loginServer = null;
            }
            m_tcp_loginServer.Close();
            m_tcp_loginServer = null;
            m_bConnected_loginServer = false;
        }
    }

    public void disconnect_mainServer()
    {
        if (m_bConnected_mainServer == false)
            return;
        //eventData data = new AnomalyData();
        //byte[] buffer = ;
        //using (m_stream_mainServer)
        //{
        //    m_stream_mainServer.Write(buffer, 0, buffer.Length);
        //    m_stream_mainServer.Flush();
        //}
        m_tHandler_mainServer.Abort();
        clearMainTcpClient();
    }
    void clearMainTcpClient()
    {
        //Debug.LogError("clear main tcp");
        if (m_tcp_mainServer != null && m_bConnected_mainServer)
        {
            if (m_stream_mainServer != null)
            {
                m_stream_mainServer.Dispose();
                m_stream_mainServer = null;
            }
            m_tcp_mainServer.Close();
            m_tcp_mainServer = null;
            m_bConnected_mainServer = false;
        }
    }

    public bool connect_loginServer(string serverIP, int nPortNUM)
    {
        if (m_bConnected_loginServer == true)
            return m_bConnected_loginServer;

        try
        {
            initClient_loginServer();
            m_tcp_loginServer.Connect(serverIP, nPortNUM);
            m_stream_loginServer = m_tcp_loginServer.GetStream();

            m_tHandler_loginServer = null;
            m_tHandler_loginServer = new Thread(receive_loginServer);
            m_tHandler_loginServer.IsBackground = true;
            m_bConnected_loginServer = true;
            m_tHandler_loginServer.Start();
            //GameManager.m_Instance.DEBUG("\nSetting Login Server.... tHandler :: "+ m_tHandler_loginServer.IsAlive);

#if DEBUGMODE
            //Debug.Log("connect server");
#endif
            return m_bConnected_loginServer;
        }
        catch (Exception ex)
        {
            GameManager.m_Instance.writeErrorLog(ex.Message);
            return m_bConnected_loginServer;
        }
    }
    public bool connect_mainServer(string serverIP, int nPortNUM)
    {
        if (m_bConnected_mainServer  == true)
            return true;

        try
        {
            initClient_mainServer();
            m_tcp_mainServer.Connect(serverIP, nPortNUM);
            m_stream_mainServer = m_tcp_mainServer.GetStream();

            m_tHandler_mainServer = null;
            m_tHandler_mainServer = new Thread(receive_mainServer);
            m_tHandler_mainServer.IsBackground = true;
            m_bConnected_mainServer = true;
            m_tHandler_mainServer.Start();
#if DEBUGMODE
            Debug.Log("connect main server");
#endif
        }
        catch (Exception ex)
        {
            GameManager.m_Instance.writeErrorLog(ex.Message);
            return false;
        }
        return true;
    }



    public void sendMessage(C_BasePacket eventData)
    {
        if (eventData == null) return;
        C_Buffer curBuf = eventData.serialize();
        if(eventData.m_basicType == BasePacketType.basePacketTypeLogin && m_bConnected_loginServer)
        {
            m_stream_loginServer.Write(curBuf.m_buf, 0, curBuf.m_length);
            m_stream_loginServer.Flush();
        }
        else if(m_tcp_mainServer != null && m_bConnected_mainServer)
        {
            m_stream_mainServer.Write(curBuf.m_buf, 0, curBuf.m_length);
            m_stream_mainServer.Flush();
        }
    }

    private void receive_loginServer()
    {
        try
        {
            m_stream_loginServer = m_tcp_loginServer.GetStream();
            while (m_bConnected_loginServer)
            {
                Thread.Sleep(10);

                int nBufSize = m_tcp_loginServer.ReceiveBufferSize;
                byte[] buffer = new byte[nBufSize];


                //GameManager.m_Instance.DEBUG("\nalready Recv....");
                int nBytes = m_stream_loginServer.Read(buffer, 0, buffer.Length);
                if(nBytes>0)
                {
                    // event Enqueue
                    C_BasePacket eventData = pakcetMaker.makePacket(new C_Buffer( buffer, nBytes));
                    if (eventData != null)
                        m_handler_loginServer(eventData);
                }
                else
                {
                    m_bConnected_loginServer = false;
                    GameManager.m_Instance.setConnectStateLoginServer(false, true);
                    clearLoginTcpClient();
                }
            }
        }
        catch (ThreadAbortException abortex)
        {
            //Debug.Log("정상 연결 종료");
            //clearLoginTcpClient();
        }
        catch (Exception ex)
        {
            m_bConnected_loginServer = false;
            GameManager.m_Instance.setConnectStateLoginServer(false, true);
            clearLoginTcpClient();
        }
    }
    private void receive_mainServer()
    {
        byte[] buffer = null;
        try
        {
            while (m_bConnected_mainServer)
            {
                Thread.Sleep(10);

                m_stream_mainServer = m_tcp_mainServer.GetStream();
                int nBufSize = m_tcp_mainServer.ReceiveBufferSize;
                buffer = new byte[nBufSize];
                int nBytes = m_stream_mainServer.Read(buffer, 0, buffer.Length);

                if (nBytes > 0)
                {
                    // event Enqueue
                    C_BasePacket eventData = pakcetMaker.makePacket(new C_Buffer(buffer, nBytes));
                    if (eventData != null)
                    {
                        m_handler_mainServer(eventData);
                        Debug.Log("Client Event Type :: " + eventData.m_basicType.ToString());
                    }
                }
                else
                {
                    m_bConnected_mainServer = false;
                    GameManager.m_Instance.setConnectStateMainServer(false, true);
                    clearMainTcpClient();
                }
            }
        }
        catch (ThreadAbortException abortex)
        {
            //Debug.Log("정상 연결 종료");
            //clearMainTcpClient();
        }
        catch (Exception ex)
        {
            m_bConnected_mainServer = false;
            GameManager.m_Instance.setConnectStateMainServer(false, true);
            clearMainTcpClient();
        }
    }
}
