#define DEBUGMODE

using PACKET;
using PROTOCOL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectChannelUI : UI
{
    [SerializeField]
    GameObject          m_ChannelSlotPrefab;
    [SerializeField]
    List<channelSlot>   m_ChannelList;
    [SerializeField]
    RectTransform       m_ChannelSlotParent;
    [SerializeField]
    Scrollbar           m_scrollbar;

    int                 m_nSelectChannelIndex;

    public float        m_default_sizeX;
    public float        m_offset_sizeY;

    enum INDEX_OF_SELECT_CHANNEL_UI
    {
        CAN_NOT_CONNET_CHANNEL_UI = 1,
    }
    
    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
        base.update(eventData);
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        if (isExistSubUI(eventData) == true)
            return;
        // 아니면 여기서 처리
        if(m_listener!=null)
            m_listener(eventData);
    }

    public override void releaseUI()
    {
        // 중단
    }

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;
        m_scrollbar.value = 1.0f;

    }

    protected override void startWaiting(responseListener listener)
    {
        base.startWaiting(listener);
        // ~~
    }
    protected override void stopWaiting()
    {
        base.stopWaiting();
    }

    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {

    }

    public void exitUI()
    {
        m_uiManager.closeUI(1);
    }

    /// <summary>
    /// 서버에서 받은 서버 리스트대로 세팅
    /// </summary>
    public void setChannel(List<S_Channel> channelList)
    {
        clearChannelList();
        m_nSelectChannelIndex = 0;
        // 리스트 대로 생성
        for (int i = 0; i < channelList.Count; ++i)
        {
            channelSlot slot = Instantiate(m_ChannelSlotPrefab, m_ChannelSlotParent.transform).GetComponent<channelSlot>();
            slot.setSlot(channelList[i].m_channelName,i,
                channelList[i].m_nNumberOfPeople,
                channelList[i].m_nLimitOfPeople,
                new channelSlot.onButtonClicked(selectChannel));
            slot.m_clickEvent = selectChannel;
            m_ChannelList.Add(slot);
        }
        m_ChannelSlotParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_default_sizeX);
        m_ChannelSlotParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_offset_sizeY*m_ChannelList.Count+20);

    }

    void selectChannel(int nChannelIndex)
    {
        m_nSelectChannelIndex = nChannelIndex;
        tryEnterChannel();
    }

    /// <summary>
    /// 채널 접속 시도
    /// </summary>
    public void tryEnterChannel()
    {
        startWaiting(responseEnterChannel);
        C_ChannelInRequestPacket eventData = new C_ChannelInRequestPacket();
        eventData.m_nChannelNumber = (byte)m_nSelectChannelIndex;
        GameManager.m_Instance.makePacket(eventData);
    }

    void responseEnterChannel(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeLogin) return;

        C_LoginPacket data = (C_LoginPacket)eventData;
        if (data.m_loginType != LoginPacketType.loginPacketTypeChannelInResonse) return;
        stopWaiting();

        C_ChannelInResponsePacket curData = (C_ChannelInResponsePacket)data;

        if(curData.m_bResult)
        {
            try
            {
                //GameManager.m_Instance.setMainServerInfo("10.255.252.83", 10000);

                if (GameManager.m_Instance.connect_mainServer())
                {
                    GameManager.m_Instance.disconnect_loginServer();
                    // openMainMenu
                    m_uiManager.closeUI(1);
                    ((loginUI)m_uiManager).openMainMenuUI();
                }
                else
                    m_uiManager.closeUI(1);
            }
            catch (Exception ex)
            {
                GameManager.m_Instance.writeErrorLog("can not connect main server");
            }
        }
        else
        {
            openUI((int)INDEX_OF_SELECT_CHANNEL_UI.CAN_NOT_CONNET_CHANNEL_UI);
        }
    }

    void clearChannelList()
    {
        foreach(channelSlot var in m_ChannelList)
        {
            DestroyImmediate(var.gameObject);
        }
        m_ChannelList.Clear();
    }
}