#define DEBUGMODE

using PACKET;
using PROTOCOL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class makeRoomUI : UI
{
    enum INDEX_MAKEROOM_UI
    {
        FAIL_MAKE_ROOM = 0,
    }

    [SerializeField]
    InputField          m_roomNameIF;
    [SerializeField]
    Dropdown            m_modeDD;
    [SerializeField]
    Dropdown            m_manCountDD;
    [SerializeField]
    Toggle              m_isPublicToggle;
    [SerializeField]
    InputField          m_psIF;

    RoomGameType        m_selectedModeValue;
    int                 m_nSelectedManCountValue;
    bool                m_bPublic;




    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        if (isExistSubUI(eventData) == true)
            return;

        if(m_listener != null)
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

        m_roomNameIF.text = "";
        m_modeDD.value = 0;
        m_selectedModeValue = (RoomGameType)(m_modeDD.value + 1);
        m_manCountDD.value = 0;
        m_nSelectedManCountValue = 2;
        m_isPublicToggle.isOn = true;
        m_bPublic = true;
        m_psIF.interactable = false;
        m_psIF.text = "";
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

    // mode dd changed
    public void changedModeDD()
    {
        m_selectedModeValue = (RoomGameType)(m_modeDD.value+1);
    }
    // man count dd changed
    public void changedManCountDD()
    {
        m_nSelectedManCountValue = m_manCountDD.value+2;
    }
    // toggle changed
    public void changedToggle()
    {
        m_bPublic = m_isPublicToggle.isOn;
        m_psIF.interactable = !m_bPublic;
    }

    // request make room
    public void requestMakeRoom()
    {
#if DEBUGMODE
        //responseMakeRoom(null);
        //return;
#endif
        startWaiting(responseMakeRoom);
        C_RoomPacketMakeRoomRequest data = new C_RoomPacketMakeRoomRequest();
        data.m_roomName = m_roomNameIF.text;
        data.m_gameType = m_selectedModeValue;
        data.m_maxPlayer = (Int16)m_nSelectedManCountValue;
        data.m_password = (Int16)(m_bPublic == true ? 0 : Int16.Parse( m_psIF.text));
        GameManager.m_Instance.makePacket(data);
    }

    // response make room
    void responseMakeRoom(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeRoom) return;
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.roomPacketTypeMakeRoomResponse) return;
        C_RoomPacketMakeRoomResponse curData = (C_RoomPacketMakeRoomResponse)data;
        stopWaiting();

        if(curData.m_success == true)
        {
            m_uiManager.closeUI(1);
            //// 2. setting  game room 
            //// ...
            string mode = myApi.GetDescription(m_selectedModeValue);
            if (mode == null)
            {
                throw new System.Exception("makeRoom::Enum Error");
            }
            List<S_RoomUserInfo> userInfo = new List<S_RoomUserInfo>();
            userInfo.Add(new S_RoomUserInfo( 1,GameManager.m_Instance.getUserNickName(), 0, false, true));
            // 부모 ui 호출 및 게임 룸 세팅
            ((mainMenuUI)m_uiManager).openGameRoom(true, 1,m_roomNameIF.text, curData.m_roomNumber,
                mode, m_bPublic, m_nSelectedManCountValue, userInfo);
        }
        else
        {
            //실패
            ((failUI)m_uiList[(int)INDEX_MAKEROOM_UI.FAIL_MAKE_ROOM]).setFail();
            openUI((int)INDEX_MAKEROOM_UI.FAIL_MAKE_ROOM);
        }
    }


    public void exitUI()
    {
        m_uiManager.closeUI(1);
    }

    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if( type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            requestMakeRoom();
    }
}
