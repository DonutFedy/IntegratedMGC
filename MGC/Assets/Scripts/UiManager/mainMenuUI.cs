#define DEBUGMODE

using System;
using System.Collections;
using System.Collections.Generic;
using PACKET;
using PROTOCOL;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuUI : UI
{
    public enum INDEX_MAINMENU_UI
    {
        SHOW_ROOM_INFO_UI = 0,
        MAKE_ROOM_UI = 1,
        EMPTY_PAGE_UI = 2,
        FRIEND_UI = 3,
        GUILD_UI = 4,
        SELECT_GAME_UI,
        LOADING_DATA_UI,
    }
    
    bool                        m_bLoadingData;

    #region RoomLIST
    [SerializeField]
    RectTransform               m_roomListParent;
    [SerializeField]
    GameObject                  m_emptyRoomText;
    [SerializeField]
    GameObject                  m_roomInfoPrefabs;
    [SerializeField]
    List<roomInfoSlot>          m_roomInfoList;

    [SerializeField]
    Text                        m_pageNumText;
    int                         m_nCurPageNUM;
    int                         m_nPrePageNUM;
    int                         m_nOffsetPageNUM;

    #endregion

    #region CHAT BOX
    bool                        m_bIgnoreChat;
    [SerializeField]
    chatBox                     m_chatBox;
    #endregion


    #region Basic UI
    public override void initUI(UI manager, int uiType)
    {
        m_bLoadingData = false;
        base.initUI(manager, uiType);
        // 기본 세팅
        ((guildUI)m_uiList[(int)INDEX_MAINMENU_UI.GUILD_UI]).m_dWhisper = setWhisper;   //귓속말 함수 연결
        ((friendUI)m_uiList[(int)INDEX_MAINMENU_UI.FRIEND_UI]).m_dWhisper = setWhisper; //귓속말 함수 연결
    }
    public override void releaseUI()
    {
        // 중단
        clearRoomList();
        m_bLoadingData = GameManager.m_Instance.isConnectedMainServer();
    }

    protected override void setUI()
    {
        setLobby();
        m_chatBox.setChatBox();
    }



    void clearRoomList()
    {
        for (int i = 0; i < m_roomInfoList.Count; ++i)
        {
            DestroyImmediate(m_roomInfoList[i].gameObject);
        }
        m_roomInfoList.Clear();
    }

    /// <summary>
    /// 해당 방이 없을 때 삭제하는 처리
    /// </summary>
    /// <param name="m_nRoomNum"></param>
    void deleteRoomSlot(int m_nRoomNum)
    {
        bool bDelete = false;
        for (int i = 0; i < m_roomInfoList.Count; ++i)
        {
            if(bDelete  == false && m_nRoomNum == m_roomInfoList[i].getRoomNum())
            {
                DestroyImmediate(m_roomInfoList[i].gameObject);
                m_roomInfoList.RemoveAt(i);
                bDelete = true;
            }
            if (bDelete == true && m_roomInfoList.Count>i)
            {
                m_roomInfoList[i].setRoomListIndex(i);
            }
        }
        if( m_roomInfoList.Count<=0 )
        {
            m_roomListParent.gameObject.SetActive(false);
            m_emptyRoomText.SetActive(true);
        }

    }

    /// <summary>
    /// 해당 방이 시작 됬을때 처리
    /// </summary>
    void alreadyStartRoomSlot(int m_nRoomNum)
    {
        for (int i = 0; i < m_roomInfoList.Count; ++i)
        {
            if (m_nRoomNum == m_roomInfoList[i].getRoomNum())
            {
                m_roomInfoList[i].onStartGame(true);
                return;
            }
        }
    }

    public override void update(C_BasePacket eventData)
    {
        if (eventData.m_basicType == BasePacketType.basePacketTypeSocial)
        {
            responseChat(eventData);
        }
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        isExistSubUI(eventData);
        if (m_listener != null)
            m_listener(eventData);
        else if (eventData.m_basicType == BasePacketType.basePacketTypeRoom)
        {
            C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
            if (data.m_roomType == RoomPacketType.roomPacketTypeRoomListResponse)
                responseRoomList(eventData);
        }
    }

    protected override void startWaiting(responseListener listener)
    {
        base.startWaiting(listener);
        // ...
    }

    protected override void stopWaiting()
    {
        base.stopWaiting();
    }

    public override void setOnCloseSubUI()
    {
        if(m_bLoadingData == false)
            endLoadingData();
        else
           setLobby();
    }

    void setLobby()
    {
        m_bIgnoreChat = false;
        m_nCurPageNUM = 1;
        m_nPrePageNUM = m_nCurPageNUM;
        m_pageNumText.text = m_nCurPageNUM + "";
        m_bisFocuse = false;
        if (m_bLoadingData == false)
            startLoadingData();
        else
            requestRoomList(m_nCurPageNUM);
    }

    #endregion

    #region BTN Event
    /// <summary>
    /// 페이지 좌우 버튼 클릭 이벤트
    /// 좌 : -1
    /// 우 : +1
    /// </summary>
    /// <param name="nDirection"></param>
    public void clickNextPage(int nDirection)
    {
        if ((m_nCurPageNUM + nDirection) <= 0) nDirection = 0;
        m_nOffsetPageNUM = nDirection;
        requestRoomList(m_nCurPageNUM + m_nOffsetPageNUM);
    }

    void roomInfoOnClickEvent(int nRoomListIndex)
    {
        if (nRoomListIndex >= m_roomInfoList.Count) return;
        ((showRoomInfoUI)m_uiList[(int)INDEX_MAINMENU_UI.SHOW_ROOM_INFO_UI]).setRoomInfo(
            m_roomInfoList[nRoomListIndex].getRoomInfo(),
            deleteRoomSlot, alreadyStartRoomSlot);
        openUI((int)INDEX_MAINMENU_UI.SHOW_ROOM_INFO_UI);
        m_bIgnoreChat = true;
    }

    public void openSelectGameUI()
    {
        openUI((int)INDEX_MAINMENU_UI.SELECT_GAME_UI);
        m_bIgnoreChat = true;
    }

    public void logout()
    {
        m_uiManager.closeUI(1);
        m_bLoadingData = false;

        GameManager.m_Instance.setLoginState(false);
        try
        {
            GameManager.m_Instance.connect_loginServer();
        }
        catch (Exception ex)
        {
            Debug.Log("Error :: disconnect login server");
        }
        finally
        {
            try
            {
                GameManager.m_Instance.disconnect_mainServer();
            }
            catch (Exception ex)
            {
                Debug.Log("Error :: disconnect Main server");
            }
        }
    }
    
    public void sendChat()
    {
        m_chatBox.sendChat();
    }

    public void rerequestRoomListBTN()
    {
        requestRoomList(m_nCurPageNUM);
    }


    #endregion

    #region SERVER

    void requestRoomList(int nPage)
    {
#if DEBUGMODE
        //responseRoomList(null);
        ////eventData data = new roomListRequestData();
        ////data.deserialize(GameManager.m_Instance.getUserNickName());
        ////GameManager.m_Instance.makePacket(data);
        //return;
#endif
        C_RoomPacketRoomListRequest data = new C_RoomPacketRoomListRequest();
        data.m_page = (Int16)nPage;
        GameManager.m_Instance.makePacket(data);
    }

    void responseRoomList(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeRoom) return;
        C_BaseRoomPacket data = (C_BaseRoomPacket)eventData;
        if (data.m_roomType != RoomPacketType.roomPacketTypeRoomListResponse) return;
        C_RoomPacketRoomListResponse curData = (C_RoomPacketRoomListResponse)data;


        m_nCurPageNUM += m_nOffsetPageNUM;
        m_nOffsetPageNUM = 0;
        if (curData.m_listCount <= 0)
        {
            if (m_nCurPageNUM != 1)
            {
                m_nCurPageNUM -= 1;
                requestRoomList(m_nCurPageNUM);
            }
            else
            {
                clearRoomList();
                m_roomListParent.gameObject.SetActive(false);
                m_emptyRoomText.SetActive(true);
            }
        }
        else
        {
            m_nPrePageNUM = m_nCurPageNUM;
            m_pageNumText.text = m_nCurPageNUM + "";
            clearRoomList();
            for (int i = 0; i < curData.m_listCount; ++i)
            {
                GameObject roomInfoObject = Instantiate(m_roomInfoPrefabs, m_roomListParent);
                roomInfoSlot curRoomInfo = roomInfoObject.GetComponent<roomInfoSlot>();
                curRoomInfo.setRoomInfo(i, 
                    curData.m_roomList[i],
                    myApi.GetDescription( curData.m_roomType), 
                    new roomInfoSlot.d_onClickEvent(roomInfoOnClickEvent));
                m_roomInfoList.Add(curRoomInfo);
            }
            m_roomListParent.gameObject.SetActive(true);
            m_emptyRoomText.SetActive(false);
        }

    }

    #endregion

    void responseChat(C_BasePacket data)
    {
#if DEBUGMODE
        m_chatBox.updateChat(data);
#endif

    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (m_bIgnoreChat == true && isExistSubUI(type) == true) return;
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            sendChat();
        else if (type == inputKeyManager.S_KeyInput.KeyType.TAP)
            m_chatBox.focusIF();
    }


    #region CHATBOX

    /// <summary>
    /// 각 slot들의 귓속말 BTN 클릭시 귓속말 상대 설정
    /// </summary>
    /// <param name="nickname"></param>
    void setWhisper(string nickname)
    {
        if ( nickname.CompareTo(GameManager.m_Instance.getUserNickName()) == 0) return;// 대상이 유저 본인이면 return

        m_chatBox.setWhisper(nickname);
    }

    #endregion

    #region UI OPNE / CLOSE


    /// <summary>
    /// loading data UI를 열어서 유저 데이터를 받는다.
    /// </summary>
    void startLoadingData()
    {
        openUI((int)INDEX_MAINMENU_UI.LOADING_DATA_UI);
    }

    /// <summary>
    /// 로딩이 끝나면 불릴 함수
    /// </summary>
    void endLoadingData()
    {
        m_bLoadingData  = true;
        m_bisFocuse     = true;
        // 초기화
        m_bWaiting      = false;
        // 방리스트 초기화
        clearRoomList();
        // 방리스트 요청
        requestRoomList(m_nCurPageNUM);
       
    }

    /// <summary>
    /// 게임 룸 세팅 및 open
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="roomMode"></param>
    /// <param name="bPublic"></param>
    /// <param name="nRoomLimit"></param>
    /// <param name="userList"></param>
    public void openGameRoom( bool bMakeRoom,int nSlotIndex, string roomName, int nRoomNumber,string roomMode, bool bPublic, int nRoomLimit, List<S_RoomUserInfo> userList)
    {
        m_chatBox.setChatBox();
        ((controllerUI)m_uiManager).setGameRoom(bMakeRoom, nSlotIndex, roomName, nRoomNumber, roomMode, bPublic, nRoomLimit, userList);
        ((controllerUI)m_uiManager).openUI((int)controllerUI.INDEX_OF_CONTROLLER_UI.GAMEROOM_UI);
        m_bIgnoreChat = true;
    }

    public void openShopUI()
    {
        ((controllerUI)m_uiManager).openUI((int)controllerUI.INDEX_OF_CONTROLLER_UI.SHOP_UI);
        m_bIgnoreChat = true;
    }

    /// <summary>
    /// 방 만들기 ui open
    /// </summary>
    public void openMakeRoomUI()
    {
        openUI((int)INDEX_MAINMENU_UI.MAKE_ROOM_UI);
        m_bIgnoreChat = true;
    }

    public void openFriendUI()
    {
        openUI((int)INDEX_MAINMENU_UI.FRIEND_UI);
    }

    public void openGuildUI()
    {
        openUI((int)INDEX_MAINMENU_UI.GUILD_UI);
    }

    #endregion
}
