//#define NOTLOGINSERVER
#define IGNORLOGIN

using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class controllerUI : UI
{
    [SerializeField]
    GameObject m_waitReconnectUI;
    public enum INDEX_OF_CONTROLLER_UI
    {
        LOGIN_UI = 0,
        MAINMENU_UI= 1,
        GAMEROOM_UI = 2,
        SHOP_UI= 3,
        INVITE_UI,
    }


    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    protected void onAnomalyEvnet(C_BasePacket curevent)
    {
        C_Anomaly data = (C_Anomaly)curevent;

        switch (data.m_type)
        {
            case AnomalyType.loginServer_Reconnect:
                m_waitReconnectUI.SetActive(false);
                break;
            case AnomalyType.loginServer_Disconnect:
#if NOTLOGINSERVER
                return;
#endif
                m_waitReconnectUI.SetActive(true);
                //m_uiList[m_uiIndexStack.Peek()].stopWaitingUI();
                m_uiList[m_uiIndexStack.Peek()].releaseUI();
                m_uiList[m_uiIndexStack.Peek()].closeUI(1);
                break;
            case AnomalyType.mainServer_Reconnect:
                break;
            case AnomalyType.mainServer_Disconnect:
                while (m_uiIndexStack.Count > 1)
                {
                    closeUI(1);
                }
                //openUI((int)INDEX_OF_CONTROLLER_UI.LOGIN_UI);
                m_uiList[m_uiIndexStack.Peek()].releaseUI();
                UI curUI = getLoginUI();
                ((loginUI)curUI).onErrorClient("게임 서버에 접속 할 수 없습니다.");
                GameManager.m_Instance.connect_loginServer();
                break;
        }
    }

    #region Basic UI

    public override void update(C_BasePacket eventData)
    {
        if (eventData.m_basicType == BasePacketType.basePacketTypeSize)
        {
            onAnomalyEvnet(eventData);
            return;
        }
        else if(eventData.m_basicType == BasePacketType.basePacketTypeSocial && eventData.getSubType() == (int)SocialPacketType.packetTypeSocialInviteGameRoom)
        {
            // casting data....
            openInviteInfoUI(/* something data .... */);
        }
        isExistSubUI(eventData);
    }

    public void openInviteInfoUI(object data =null)
    {
        // set invite Ui
        ((inviteUI)m_uiList[(int)INDEX_OF_CONTROLLER_UI.INVITE_UI]).setInviteInfo(/* something data .... */);
        // open invite ui
        openUI((int)INDEX_OF_CONTROLLER_UI.INVITE_UI, true);
    }



    public override void releaseUI()
    {
        // 중단
    }

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;
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
        if (isExistSubUI(type))
            return;
    }

    #endregion





    public void setGameRoom(bool bMakeRoom, int nSlotIndex, string roomName, int nRoomNumber, string roomMode, bool bPublic, int nRoomLimit, List<S_RoomUserInfo> userList)
    {
        ((gameRoomUI)m_uiList[(int)INDEX_OF_CONTROLLER_UI.GAMEROOM_UI]).setGameRoom(
            bMakeRoom, nSlotIndex,roomName, nRoomNumber,roomMode, bPublic, nRoomLimit, userList);
    }


    public void goLobbyUI(object roomInfoTemp)
    {
        while (m_uiIndexStack.Count > 1)
        {
            closeUI(1);
        }
        openUI((int)INDEX_OF_CONTROLLER_UI.MAINMENU_UI);
        S_RoomInfo roomInfo = new S_RoomInfo();
        // roomInfo setting......

        ((mainMenuUI)m_uiList[(int)INDEX_OF_CONTROLLER_UI.MAINMENU_UI]).openShowRoomInfoUI(roomInfo);
    }
    

    public UI getLoginUI()
    {
        return m_uiList[(int)INDEX_OF_CONTROLLER_UI.LOGIN_UI];
    }
}