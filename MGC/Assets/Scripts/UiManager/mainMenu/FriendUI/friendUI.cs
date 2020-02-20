using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using PACKET;

public class friendUI : UI
{
    public enum INDEX_OF_FRIEND_UI
    {
        RESULT_UI,
    }

    bool                    m_bSetting;


    bool                    m_bIsListZone;


    #region ListZone
    [SerializeField]
    GameObject              m_ListZoneOBJ;
    [SerializeField]
    GameObject              m_PlzAddFriendText;
    [SerializeField]
    RectTransform           m_ParentOfFriendList;
    [SerializeField]
    GameObject              m_FriendSlotPrefab;
    [SerializeField]
    List<friendSlot>        m_FriendList = new List<friendSlot>();
    [SerializeField]
    int                     m_SlotSizeOffsetY;

    
    #endregion


    #region SearchZone
    [SerializeField]
    GameObject              m_SearchZoneOBJ;

    [SerializeField]
    InputField              m_TargetUserNicknameIF;
    [SerializeField]
    Text                    m_TargetUserInfoText;
    bool                    m_bSuccessSearch;
    string                  m_DestUserNickname;
    #endregion

    #region ConfirmFriend
    string                  m_destUserNickname;

    [SerializeField]
    GameObject              m_emptyConfirnFriendText;
    [SerializeField]
    RectTransform           m_parentOfConfirnFriendList;
    [SerializeField]
    int                     m_confirnSlotSizeOffsetY;
    [SerializeField]
    GameObject              m_confirnSlotPrefabs;
    [SerializeField]
    List<confirnFriendSlot> m_confirnSlotList = new List<confirnFriendSlot>();
    [SerializeField]
    GameObject              m_newConfirnFriendICON;

    #endregion

    #region DELETE FRIEND
    string                  m_curDeleteDestName;


    #endregion



    #region Basic UI
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
        else if (m_listener != null)
            m_listener(eventData);
        if(eventData.m_basicType == BasePacketType.basePacketTypeSocial)
        {
            responseFriendList(eventData);
            responseConfirmFriend(eventData);
        }
    }

    public override void releaseUI()
    {
        // 중단
        clearFriendList();
        clearConfirnFriendSlot();
        m_bSetting = false;
    }

    protected override void setUI()
    {
        m_bSetting = false;
        // 초기화
        m_bWaiting = false;

        openListZone();
        m_bSetting = true;
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

    private void OnDisable()
    {
        clearFriendList();
        clearConfirnFriendSlot();
    }

    /// <summary>
    /// 친구 리스트 초기화
    /// </summary>
    void clearFriendList() 
    {
        // clear 작업
        while(m_FriendList.Count>0)
            removeSlot_list(0);
        m_FriendList.Clear();
    }

    #endregion


    #region CHAT BOX WHISPER
    public delegate void setWhisperTarget(string nickname);
    public setWhisperTarget m_dWhisper;

    #endregion


    #region EVENT

    /// <summary>
    /// 길드원 귓속말 BTN 클릭시 
    /// </summary>
    /// <param name="nickname"></param>
    void clickWhisper(string nickname)
    {
        if (m_dWhisper != null)
            m_dWhisper(nickname);
    }

    /// <summary>
    /// 친구 리스트의 slot을 삭제한다.
    /// </summary>
    /// <param name="nIndex"></param>
    void removeSlot_list(int nIndex)
    {
        if (m_FriendList.Count <= nIndex) return;
        DestroyImmediate(m_FriendList[nIndex].gameObject);
        m_FriendList.RemoveAt(nIndex);
        m_ParentOfFriendList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + m_SlotSizeOffsetY * m_FriendList.Count);
    }

    void makeFriendSlot(S_FriendInfo info)
    {
        friendSlot newSlot = (Instantiate(m_FriendSlotPrefab,
            m_ParentOfFriendList).GetComponent<friendSlot>());  // slot 생성
        
        newSlot.setSlot(info, requestDeleteFriend,clickWhisper);          // 각 slot에 deleteFriend함수 연결시켜줘야함
        m_FriendList.Add(newSlot);                              // add list
    }
    void clearSearchZone()
    {
        // clear part
        m_bSuccessSearch = false;
        m_TargetUserNicknameIF.text = "";        // clear IF
        m_TargetUserInfoText.text = "";             // clear target User Info
    }

    void clearConfirnFriendSlot() // 친구 요청 슬롯 초기화
    {
        while (m_confirnSlotList.Count > 0)
        {
            DestroyImmediate(m_confirnSlotList[0].gameObject);
            m_confirnSlotList.RemoveAt(0);
        }
    }

    void removeConfirnFriendSlot(string destName)
    {
        int nTargetIndex = m_confirnSlotList.FindIndex(x => x.getSlotDestName() == destName);
        if (nTargetIndex == -1) return;
        DestroyImmediate(m_confirnSlotList[nTargetIndex].gameObject);
        m_confirnSlotList.RemoveAt(nTargetIndex);
    }

    #endregion

    #region Server REQ / RES

    /// <summary>
    /// 친구 요청에 대한 응답
    /// </summary>
    void AcceptFriendRequest(string destName, bool bPermission)
    {
        C_SocialPacketAcceptFriendRequest data = new C_SocialPacketAcceptFriendRequest();
        data.m_destName = destName;
        if (bPermission)
        {
            // 해당 슬롯 삭제
            startWaiting(responseAcceptFriendResponse);
            removeConfirnFriendSlot(destName);
            data.m_isAccept = true;
        }
        else
        {
            data.m_isAccept = false;
            removeConfirnFriendSlot(destName);
        }
        GameManager.m_Instance.makePacket(data);
    }

    void responseAcceptFriendResponse(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeSocial)return;
        C_BaseSocialPacket data = (C_BaseSocialPacket)eventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialAcceptFriendResponse) return;
        C_SocialPacketAcceptFriendResponse curData = (C_SocialPacketAcceptFriendResponse)data;
        stopWaiting();
        Debug.Log("수락 응답 처리");
    }


    void requestConfirmFriend()
    {
        C_SocialPacketConfirmFriendRequest data = new C_SocialPacketConfirmFriendRequest();
        GameManager.m_Instance.makePacket(data);
    }


    void responseConfirmFriend(C_BasePacket eventData)
    {
        C_BaseSocialPacket data = (C_BaseSocialPacket)eventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialConfirmFriendResponse) return;

        C_SocialPacketConfirmFriendResponse curData = (C_SocialPacketConfirmFriendResponse)data;

        if(curData.m_size > 0)
        {
            m_emptyConfirnFriendText.SetActive(false);
            m_parentOfConfirnFriendList.gameObject.SetActive(true);

            // list make
            for(int i = 0; i < curData.m_size; ++i)
            {
                confirnFriendSlot slot = Instantiate(m_confirnSlotPrefabs, m_parentOfConfirnFriendList).GetComponent<confirnFriendSlot>();
                slot.setSlot(curData.m_names[i], AcceptFriendRequest);
                m_confirnSlotList.Add(slot);
            }
            m_parentOfConfirnFriendList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                10 + m_confirnSlotSizeOffsetY * m_confirnSlotList.Count);
            if(m_bIsListZone == true)
                m_newConfirnFriendICON.SetActive(true);
        }
        else
        {
            m_parentOfConfirnFriendList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                10);
            m_newConfirnFriendICON.SetActive(false);
            // 요청이 없습니다.Text
            m_emptyConfirnFriendText.SetActive(true);
            m_parentOfConfirnFriendList.gameObject.SetActive(false);
        }
    }

    void requestFriendList()
    {
        m_PlzAddFriendText.SetActive(true);
        //// 서버에 유저의 친구 리스트 req
        C_SocialPacketFriendListRequest data = new C_SocialPacketFriendListRequest();
        GameManager.m_Instance.makePacket(data);
    }

    /// <summary>
    /// 친구 리스트 response process
    /// </summary>
    /// <param name="curEventData"></param>
    void responseFriendList(C_BasePacket curEventData)
    {
        C_BaseSocialPacket data = (C_BaseSocialPacket)curEventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialFriendListResponse) return;
        C_SocialPacketFriendListResponse curData = (C_SocialPacketFriendListResponse)data;

        if (curData.m_size>0)    
        {
            m_PlzAddFriendText.SetActive(false);
            for (int i = 0; i < curData.m_size; ++i)                                     // 일단, 임시로 5명의 친구를 추가
            {
                S_FriendInfo newInfo = new S_FriendInfo();            // info 객체 생성
                newInfo.m_nickName = curData.m_friends[i].m_nickName;
                newInfo.m_bIsOnLine = curData.m_friends[i].m_bIsOnLine;
                makeFriendSlot(newInfo);                                // 객체 생성
            }
            // recttransform resizing
            m_ParentOfFriendList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + m_SlotSizeOffsetY * m_FriendList.Count);
        }
    }

    void requestDeleteFriend(string userNickname)
    {
        startWaiting(responseDeleteFriend);
        m_curDeleteDestName = userNickname;
        C_SocialPacketDeleteFriendRequest data = new C_SocialPacketDeleteFriendRequest();
        data.m_destName = userNickname;
        GameManager.m_Instance.makePacket(data);
    }

    void responseDeleteFriend(C_BasePacket curEventData)
    {
        if (curEventData.m_basicType != BasePacketType.basePacketTypeSocial) return;
        C_BaseSocialPacket data = (C_BaseSocialPacket)curEventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialDeleteFriendResponse) return;
        stopWaiting();
        C_SocialPacketDeleteFriendResponse curData = (C_SocialPacketDeleteFriendResponse)data;

        if (curData.m_success)    // 성공 recv 시 성공 팝업 및 현재 친구 리스트에서 제거
        {
            int nIndex = m_FriendList.FindIndex(x => x.compareNickname(m_curDeleteDestName) == 0);
            if (nIndex < 0)
                return;
            removeSlot_list(nIndex);
        }
        else        // 실패 recv 시 실패 팝업
        {
            ((ResultUI)m_uiList[(int)INDEX_OF_FRIEND_UI.RESULT_UI]).setResultMSG("삭제 실패");
            openUI((int)INDEX_OF_FRIEND_UI.RESULT_UI);
        }
    }

    public void requestAddFriend()
    {
        if (m_TargetUserNicknameIF.text.Length <= 0 || m_TargetUserNicknameIF.text == GameManager.m_Instance.getUserNickName()) return;

        startWaiting(responseAddFriend);
        C_SocialPacketAddFriendRequest data = new C_SocialPacketAddFriendRequest();
        data.m_destName = m_TargetUserNicknameIF.text;
        GameManager.m_Instance.makePacket(data);
    }

    void responseAddFriend(C_BasePacket curEventData)
    {
        C_BaseSocialPacket data = (C_BaseSocialPacket)curEventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialAddFriendResponse) return;

        C_SocialPacketAddFriendResponse curData = (C_SocialPacketAddFriendResponse)data;
        stopWaiting();

        if (curData.m_success) // 신청 성공
        {
            clearSearchZone();
            ((ResultUI)m_uiList[(int)INDEX_OF_FRIEND_UI.RESULT_UI]).setResultMSG("친구 추가 신청 완료!");
            openUI((int)INDEX_OF_FRIEND_UI.RESULT_UI);
        }
        else        // 실패 recv 시 실패 팝업
        {
            string errorMSG = myApi.GetDescription(curData.m_errorCode);

            ((ResultUI)m_uiList[(int)INDEX_OF_FRIEND_UI.RESULT_UI]).setResultMSG(errorMSG);  // 실패 recv 시 실패 팝업 return
            openUI((int)INDEX_OF_FRIEND_UI.RESULT_UI);
        }
    }
    #endregion

    #region UI OPEN / CLOSE
    /// <summary>
    /// 현재 UI 닫기 BTN
    /// </summary>
    public void exitUI()
    {
        m_uiManager.closeUI(1);
        m_uiManager.setOnCloseSubUI();
    }


    public void openListZone()
    {
        m_bIsListZone = true;
        m_newConfirnFriendICON.SetActive(false);
        m_SearchZoneOBJ.SetActive(false);
        m_ListZoneOBJ.SetActive(true);

        if (m_bSetting == false)
        {
        }
        else // else if change list -> list sorting
        {

        }
        clearConfirnFriendSlot();
        requestConfirmFriend();
        clearFriendList();          // 리스트 초기화
        requestFriendList();        // 리스트 요청
    }



    public void openSearchZone()
    {
        m_bIsListZone = false;
        m_SearchZoneOBJ.SetActive(true);
        m_ListZoneOBJ.SetActive(false);
        clearConfirnFriendSlot();
        requestConfirmFriend();
        clearSearchZone();
    }
    
    #endregion
}