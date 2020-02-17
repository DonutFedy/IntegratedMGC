#define DEBUGMODE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PACKET;
using PROTOCOL;
using UnityEngine;
using UnityEngine.UI;

public class socketManager : management
{

    client              m_client;


    public override void init()
    {
        // 한번만 불림
        if (m_bInit == true) return;
        m_bInit = true;

        m_eventBuffer = new Queue<C_BasePacket>();

        m_client = new client();
        m_client.m_handler_loginServer += new client.EventHandler(enqueueEvent);
        m_client.m_handler_mainServer += new client.EventHandler(enqueueEvent);
    }

    public override void release()
    {
        release_loginServer();
        release_mainServer();
        m_eventBuffer.Clear();
    }

    void release_loginServer()
    {
        m_client.disconnect_loginServer();
    }
    void release_mainServer()
    {
        m_client.disconnect_mainServer();
    }


    protected override void processEvent(C_BasePacket curEvent)
    {
        // 각 이벤트 타입에 따라 매니저먼트의 이벤트 버퍼에 꽂아준다.
        // 요청/응답 구분 해야함

        //GameManager.m_Instance.writeErrorLog("NOT FOUND PROCESS EVENT");

        if(curEvent.m_bResponse == true)
        {

            if(curEvent.m_basicType == BasePacketType.basePacketTypeGame)
            {
                GameManager.m_Instance.makeInGameEvent(curEvent);
            }
            else
            {
                GameManager.m_Instance.makeUiEvent(curEvent);
            }
        }
        else 
        {
            m_client.sendMessage(curEvent);
        }
    }

    public bool connect_loginServer(string serverIP, int nPortNUM)
    {
        return m_client.connect_loginServer(serverIP, nPortNUM);
    }
    public bool connect_mainServer(string serverIP, int nPortNUM)
    {
        return m_client.connect_mainServer(serverIP, nPortNUM);
    }

    public void disconnect_loginServer()
    {
        m_client.disconnect_loginServer();
    }
    public void disconnect_mainServer()
    {
        m_client.disconnect_mainServer();
    }

    public void sendAnomalyClose()
    {

    }



}

