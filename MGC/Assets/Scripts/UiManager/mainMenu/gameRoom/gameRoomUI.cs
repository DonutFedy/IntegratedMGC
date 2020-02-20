#define DEBUGMODE

using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using PACKET;

public class gameRoomUI : UI
{
    [SerializeField]
    Text                    m_roomNameText;
    [SerializeField]
    Text                    m_roomModeText;
    [SerializeField]
    Text                    m_manCountText;
    [SerializeField]
    Text                    m_passwordText;
    [SerializeField]
    Image                   m_publicImage;
    [SerializeField]
    Sprite[]                m_publicSprites;
    bool m_bPublic;

    [SerializeField]
    chatBox                 m_chatBox;

    [SerializeField]
    GameObject              m_userInfoPrefabs;
    [SerializeField]
    List<userInfoPrefab>    m_userInfoList;
    [SerializeField]
    Transform               m_userInfoParent;

    [SerializeField]
    Text                    m_readyBTNtext;


    int                     m_nRoomLimit;
    int                     m_clientUserSlotIndex;
    bool                    m_bNeedInfo;

    public enum INDEX_OF_GAMEROOM_UI :int
    {
        FAIL_START = 0,
        RESULT_UI,
        INVITE_UI,
    }

    #region BASIC UI


    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
        if (eventData.m_basicType == BasePacketType.basePacketTypeSocial)
        {
            responseChat(eventData);
        }
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        isExistSubUI(eventData);
        m_listener?.Invoke(eventData);
        if (m_bNeedInfo == false && eventData.m_basicType == BasePacketType.basePacketTypeRoom)
        {
            responseRoomInfo(eventData);
            responseStartGame(eventData);
        }
    }

    public override void releaseUI()
    {
        // 중단
    }

    protected override void setUI()
    {
        // 초기화
        m_chatBox.setChatBox();
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

    /// <summary>
    /// 게임 룸을 열기 전, 게임 방에 대한 상세를 정의 한다.
    /// user List 같은 경우, 현재 클라이언트 유저의 값이 맨 마지막에 들어간다.
    /// </summary>
    public void setGameRoom(bool bMakeRoom, int nSlotIndex, string roomName, int nRoomNumber, string roomMode, bool bPublic,
        int nRoomLimit, List<S_RoomUserInfo> userList)
    {
        // clean userInfo list
        cleanUserInfo();
        m_clientUserSlotIndex = nSlotIndex;

        m_nRoomLimit = nRoomLimit;

        m_roomNameText.text = nRoomNumber + "번 방 :" + roomName;
        m_roomModeText.text = roomMode;
        m_manCountText.text = userList.Count+ "/"+ m_nRoomLimit;

        m_bPublic = bPublic;
        if (m_bPublic)
        {
            m_publicImage.sprite = m_publicSprites[1];
            m_passwordText.gameObject.SetActive(false);
        }
        else
        {
            m_publicImage.sprite = m_publicSprites[0];
            m_passwordText.text = "";
            m_passwordText.gameObject.SetActive(true);
        }

        // 유저 리스트에 따른 생성
        for (int i = 0; i < m_nRoomLimit; ++i)
        {
            makeUserInfoEmpty();
            if (i < userList.Count)
                setUserInfo(userList[i]);
            else
                m_userInfoList[i].exitUser();
        }

        // bMakeRoom에 따른 처리.
        // bMakeRoom == false 면 정보 얻기 전까지 대기하자

        if (bMakeRoom == false)
        {
            // waiting 처리
            m_bNeedInfo = true;

            startWaiting(responseRoomInfo);
            // 서버에 info 요청
            C_RoomPacketRoomInfoRequest data = new C_RoomPacketRoomInfoRequest();
            GameManager.m_Instance.makePacket(data);
            return;
        }

        m_bWaiting = false;
    }

    #endregion

    void setUserInfo(S_RoomUserInfo info)
    {
        if (info.m_nSlotIndex - 1 >= m_userInfoList.Count) return;
        userInfoPrefab newInfo = m_userInfoList[info.m_nSlotIndex-1];
        newInfo.setUserInfo(info, setWhisper);
        if(info.m_nSlotIndex == m_clientUserSlotIndex)
        {
            string BTNtext = null;
            if (info.m_bIsMaster)
                BTNtext = "게임 시작";
            else
                BTNtext = "Ready";
            m_readyBTNtext.text = BTNtext;
        }
    }
    void makeUserInfoEmpty()
    {
        userInfoPrefab newInfo = Instantiate(m_userInfoPrefabs, m_userInfoParent).GetComponent<userInfoPrefab>();
        m_userInfoList.Add(newInfo);
    }

    void cleanUserInfo()
    {
        foreach(userInfoPrefab var in m_userInfoList)
        {
            DestroyImmediate(var.gameObject);
        }
        m_userInfoList.Clear();
    }

    public void exitUI()
    {
        startWaiting(responseExitGameroom);
        C_RoomPacketLeaveRoomRequest data = new C_RoomPacketLeaveRoomRequest();
        GameManager.m_Instance.makePacket(data);
    }

    void responseExitGameroom(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeRoom) return;
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.roomPacketTypeLeaveRoomResponse) return;

        C_RoomPacketLeaveRoomResponse curData = (C_RoomPacketLeaveRoomResponse)data;
        if(curData.m_bIsSuccess)
        {
            m_uiManager.closeUI(1);
        }
        else
        {
            ((ResultUI)m_uiList[(int)INDEX_OF_GAMEROOM_UI.RESULT_UI]).setResultMSG("<Size= 50>방 나가기 실패!</Size>\n<size=40>다시 시도해주세요.</size>");
            openUI((int)INDEX_OF_GAMEROOM_UI.RESULT_UI);
        }

    }


    public void sendChat()
    {
        m_chatBox.sendChat();
    }
    void responseChat(C_BasePacket data)
    {

#if DEBUGMODE
        m_chatBox.updateChat(data);
        C_BaseSocialPacket curData = (C_BaseSocialPacket)data;
        if (curData.m_socialType != SocialPacketType.packetTypeSocialChatNormalResponse) return;
        C_SocialPacketChatNormalResponse normalData = (C_SocialPacketChatNormalResponse)curData;

        userInfoPrefab clientUser = m_userInfoList.Find((x) => { return x.getNickName() == normalData.m_nickname; });
        if (clientUser)
        {
            clientUser.chating(normalData.m_message);
        }
#endif

    }

    public void tryChangeReadyState()
    {
        if(m_userInfoList[m_clientUserSlotIndex-1].isMaster())
        {
            if(checkReadyUsers())
            {
                // start game
                C_RoomPacketStartGameRequest startData = new C_RoomPacketStartGameRequest();
                GameManager.m_Instance.makePacket(startData);
            }
            else
            {
                ((failUI)m_uiList[(int)INDEX_OF_GAMEROOM_UI.FAIL_START]).setFail("게임 시작 불가");
                openUI((int)INDEX_OF_GAMEROOM_UI.FAIL_START);
            }
            return;
        }

        C_RoomPacketToogleReadyRequest data = new C_RoomPacketToogleReadyRequest();
        GameManager.m_Instance.makePacket(data);
    }
    
    bool checkReadyUsers()
    {
#if DEBUGMODE
        return true;
#endif
        if (m_userInfoList.Count <= 1)
            return false;
        for(int i = 0; i < m_userInfoList.Count; ++i)
        {
            if (i != (m_clientUserSlotIndex-1) && m_userInfoList[i].getReadyState() == false)
                return false;
        }
        return true;
    }

    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (isExistSubUI(type) == true) return;
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            sendChat();
        else if (type == inputKeyManager.S_KeyInput.KeyType.TAP)
            m_chatBox.focusIF();
    }

    #region SERVER REQ / RES
    void responseRoomInfo(C_BasePacket eventData)
    {
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.packetTypeRoomRoomInfoResponse) return;
        C_RoomPacketRoomInfoResponse curData = (C_RoomPacketRoomInfoResponse)data;
        if (m_bNeedInfo)
        {
            stopWaiting();
            m_bNeedInfo = false;
        }

        for (int i = 0; i < m_userInfoList.Count; ++i)
        {
            m_userInfoList[i].exitUser();
        }
        for (int i = 0; i < curData.m_listCount; ++i)
        {
            setUserInfo(curData.m_userList[i]);
        }
        m_manCountText.text = curData.m_userList.Count + "/" + m_nRoomLimit;
    }

    void responseStartGame(C_BasePacket eventData)
    {
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.packetTypeRoomStartGameResponse) return;
        C_RoomPacketStartGameResponse curData = (C_RoomPacketStartGameResponse)data;

        if(curData.m_bIsSuccess)
        {
            // 게임 시작 준비
            // 2. 현재 ui 닫기
            m_uiManager.closeUI(1);
            // 1. 게임 매니저에 게임 시작 알리기
            GameManager.m_Instance.startGame();
        }
        else if(m_userInfoList[m_clientUserSlotIndex-1].isMaster())
        {
            // 오류 메세지 출력
            ((ResultUI)m_uiList[(int)INDEX_OF_GAMEROOM_UI.RESULT_UI]).setResultMSG("게임을 시작할 수 없습니다.");
            openUI((int)INDEX_OF_GAMEROOM_UI.RESULT_UI);
        }

    }



    #endregion


    #region UI OPEN/CLOSE
    public void openInviteListUI()
    {
        openUI((int)INDEX_OF_GAMEROOM_UI.INVITE_UI);
    }


    #endregion


    #region CHATBOX

    /// <summary>
    /// 각 slot들의 귓속말 BTN 클릭시 귓속말 상대 설정
    /// </summary>
    /// <param name="nickname"></param>
    void setWhisper(string nickname)
    {
        if (nickname.CompareTo(GameManager.m_Instance.getUserNickName()) == 0) return;// 대상이 유저 본인이면 return

        m_chatBox.setWhisper(nickname);
    }

    #endregion
}
