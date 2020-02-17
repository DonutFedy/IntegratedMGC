using PACKET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roomInfoSlot : MonoBehaviour
{
    [SerializeField]
    public int              m_nRoomListIndex;

    S_RoomInfo              m_roomInfo;


    [SerializeField]
    Text            m_modeText;
    [SerializeField]
    Text            m_roomNumberText;
    [SerializeField]
    Text            m_roomNameText;
    [SerializeField]
    Text            m_manCountText;
    [SerializeField]
    Image           m_publicRoomImage;
    [SerializeField]
    Sprite[]        m_roomPublicSprites;
    [SerializeField]
    Button          m_roomSlotBTN;
    

    public delegate void d_onClickEvent(int nRoomNumber);
    d_onClickEvent  m_donClickEvent;

    public void setRoomInfo(int nRoomListIndex, S_RoomInfo roomInfo,string roomMode,d_onClickEvent clickEvent)
    {

        m_roomInfo = roomInfo;

        m_nRoomListIndex = nRoomListIndex;
        if(m_roomInfo.m_password == false)
        {
            m_publicRoomImage.sprite = m_roomPublicSprites[1];
        }
        else
        {
            m_publicRoomImage.sprite = m_roomPublicSprites[0];
        }
        m_manCountText.text = m_roomInfo.m_playerCount+"/" + m_roomInfo.m_maxPlayerCount;
        m_modeText.text = roomMode;
        m_roomNameText.text = m_roomInfo.m_roomName;
        m_roomNumberText.text = "No." + m_roomInfo.m_number;
        m_donClickEvent += clickEvent;

        onStartGame(m_roomInfo.m_bIsStart);
    }

    /// <summary>
    /// 게임 시작됫으면 비활성화
    /// </summary>
    /// <param name="bIsStart"></param>
    public void onStartGame(bool bIsStart)
    {
        m_roomSlotBTN.interactable = !bIsStart;
    }

    public void setRoomListIndex(int nRoomListIndex)
    {
        m_nRoomListIndex = nRoomListIndex;
    }

    public void onClickEvent()
    {
        if (m_donClickEvent != null)
            m_donClickEvent(m_nRoomListIndex);
    }
    
    public string getMode()
    {
        return m_modeText.text;
    }
    public S_RoomInfo getRoomInfo()
    {
        return m_roomInfo;
    }

    public int getRoomNum()
    {
        return m_roomInfo.m_number;
    }
}
