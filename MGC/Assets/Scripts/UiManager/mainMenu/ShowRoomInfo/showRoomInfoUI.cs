#define DEBUGMODE

using PACKET;
using PROTOCOL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showRoomInfoUI : UI
{
    enum INDEX_SHOWROOMINFO_UI
    {
        REUSLT_UI = 0,
        PW_ERROR_UI =1,
    }

    [SerializeField]
    Text            m_roomNameText;
    [SerializeField]
    Text            m_modeText;
    [SerializeField]
    Text            m_manCountText;
    [SerializeField]
    InputField      m_pwInputField;
    

    S_RoomInfo      m_roomInfo;

    public delegate void dEventRoomSlotFUNC(int nRoomNumber);
    dEventRoomSlotFUNC m_dOnDeleteRoom;
    dEventRoomSlotFUNC m_dOnStartRoom;

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
        m_pwInputField.text = "";
        if(m_roomInfo.m_password == true)
            m_pwInputField.ActivateInputField();
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


    public void setRoomInfo(S_RoomInfo roomInfo, 
        dEventRoomSlotFUNC deleteFUNC, dEventRoomSlotFUNC startFUNC)
    {
        m_roomInfo = roomInfo;

        m_roomNameText.text = m_roomInfo.m_roomName;
        m_modeText.text = myApi.GetDescription(m_roomInfo.m_roomGameType);
        m_manCountText.text = m_roomInfo.m_playerCount + "/" + m_roomInfo.m_maxPlayerCount; ;

        m_pwInputField.interactable = m_roomInfo.m_password;   // 공개시 비밀번호 IF 비활성화

        m_dOnDeleteRoom = deleteFUNC;
        m_dOnStartRoom = startFUNC;
    }


    public void requestEnterRoom()
    {
#if DEBUGMODE
        //responseEnterRoom(null);
#endif
        if (m_roomInfo.m_password == true && m_pwInputField.text.Length <= 0)
        {
            // pw 에러 ui popup
            openUI((int)INDEX_SHOWROOMINFO_UI.PW_ERROR_UI);
            return;
        }
        startWaiting(responseEnterRoom);
        // 서버에 요청 
        C_RoomPacketEnterRoomRequest data = new C_RoomPacketEnterRoomRequest();
        data.m_roomNumber = (Int16)m_roomInfo.m_number;
        if (m_roomInfo.m_password == false)
            data.m_password = 0;
        else
            data.m_password = Int16.Parse(m_pwInputField.text);
        data.m_nRoomTime = m_roomInfo.m_nCreateTime;
        // set waiting
        GameManager.m_Instance.makePacket(data);

    }

    void responseEnterRoom(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeRoom) return;
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.roomPacketTypeEnterRoomResponse) return;
        C_RoomPacketEnterRoomResponse curData = (C_RoomPacketEnterRoomResponse)data;

        stopWaiting();

        if (curData.m_bIsSuccess == true)
        {
            //성공
            // 1. close this ui
            m_uiManager.closeUI(1);
            // 2. setting  game room 
            List<S_RoomUserInfo> userInfo = new List<S_RoomUserInfo>();
            // 패킷으로 받은 유저들의 정보 add
            // ...
            //userInfo.Add(new S_RoomUserInfo( GameManager.m_Instance.getUserNickName(), 0, false, false));
            // 부모 ui 호출 및 게임 룸 세팅
            ((mainMenuUI)m_uiManager).openGameRoom(false, curData.m_nSlotIndex,
                m_roomInfo.m_roomName, m_roomInfo.m_number, m_modeText.text,
                m_roomInfo.m_password==false, m_roomInfo.m_maxPlayerCount, userInfo);
        }
        else
        {
            string errorMsg = "<Size=50>방 참가 실패!!</Size>\n<size=30>";
            switch (curData.m_errorType)
            {
                case ErrorTypeEnterRoom.errorTypeNone:
                    errorMsg += "알수 없는 이유";
                    break;
                case ErrorTypeEnterRoom.errorTypeNotExistRoom:
                    errorMsg += "해당 방이 존재 하지 않습니다.";
                    m_dOnDeleteRoom?.Invoke(m_roomInfo.m_number);
                    break;
                case ErrorTypeEnterRoom.errorTypeWrongPassword:
                    errorMsg += "비밀 번호가 일치 하지 않습니다.";
                    break;
                case ErrorTypeEnterRoom.errorTypeAlreadyIncluded:
                    errorMsg += "잘 못된 접근입니다.";
                    break;
                case ErrorTypeEnterRoom.errorTypeGameStart:
                    errorMsg += "이미 게임이 시작된 방입니다.";
                    m_dOnStartRoom?.Invoke(m_roomInfo.m_number);
                    break;
                case ErrorTypeEnterRoom.errorTypeMaxPlayer:
                    errorMsg += "꽉 차서 자리가 없습니다.";
                    break;
                case ErrorTypeEnterRoom.errorTypeCanNotEnterRoom:
                    errorMsg += "참여 할 수 없습니다.";
                    break;
                case ErrorTypeEnterRoom.errorTypeCount:
                    errorMsg += "알수 없는 이유";
                    break;
            }
            errorMsg += "</size>";


            //실패
            ((ResultUI)m_uiList[(int)INDEX_SHOWROOMINFO_UI.REUSLT_UI]).setResultMSG(errorMsg,2);
            openUI((int)INDEX_SHOWROOMINFO_UI.REUSLT_UI);
        }
    }

    public void exitUI()
    {
        m_uiManager.closeUI(1);
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (type == inputKeyManager.S_KeyInput.KeyType.ESC)
            exitUI();
        else if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            requestEnterRoom();
    }
}