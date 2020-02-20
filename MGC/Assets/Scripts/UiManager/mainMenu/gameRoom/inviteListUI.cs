using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inviteListUI : UI
{
    [SerializeField]
    GameObject              m_friendSlotPrefabs;
    [SerializeField]
    RectTransform           m_ParentOfFriendSlot;

    List<friendSlot>        m_friendList = new List<friendSlot>();

    [SerializeField]
    float                   m_SlotSizeOffsetY;

    [SerializeField]
    GameObject              m_emptyFriendOBJ;


    List<userInfoPrefab>    m_curMemebers;

    #region Basic UI

    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
    }
    public override void update(C_BasePacket eventData)
    {
        if (m_listener != null)
            m_listener(eventData);
    }

    public override void releaseUI()
    {
        // 중단
    }

    public void setCurrentGameroomMember(List<userInfoPrefab> curMemebers)
    {
        m_curMemebers = curMemebers;
    }

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;
        clearFriendList();
        m_emptyFriendOBJ.SetActive(true);
        m_ParentOfFriendSlot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + m_SlotSizeOffsetY * m_friendList.Count);

        //waiting
        startWaiting(reponseFriendList);
        C_SocialPacketFriendListRequest data = new C_SocialPacketFriendListRequest();
        GameManager.m_Instance.makePacket(data);
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


    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            exitUI(1);
    }
    #endregion

    void reponseFriendList(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeSocial || eventData.getSubType() != (int)SocialPacketType.packetTypeSocialFriendListResponse) return;

        stopWaiting();
        clearFriendList();

        C_SocialPacketFriendListResponse curData = (C_SocialPacketFriendListResponse)eventData;
        // data setting....
        for (int i = 0; i < curData.m_size; ++i)
        {
            if (curData.m_friends[i].m_bIsOnLine == true && findCurrentRoomMember(curData.m_friends[i].m_nickName) ==false)
                makeFriendSlot(curData.m_friends[i]);
        }

        m_emptyFriendOBJ.SetActive(m_friendList.Count <= 0);

        m_ParentOfFriendSlot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + m_SlotSizeOffsetY * m_friendList.Count);
    }

    bool findCurrentRoomMember(string nickname)
    {
        return m_curMemebers.Find(x => x.getNickName() == nickname) != null;
    }


    void makeFriendSlot(S_FriendInfo info)
    {
        friendSlot newSlot = (Instantiate(m_friendSlotPrefabs,
            m_ParentOfFriendSlot).GetComponent<friendSlot>());  // slot 생성

        newSlot.setSlot(info, requestInviteFriend, null,true);          // 각 slot에 deleteFriend함수 연결시켜줘야함
        m_friendList.Add(newSlot);                              // add list
    }


    void requestInviteFriend(string nickname)
    {
        C_SocialPacketInviteFriendRequest data = new C_SocialPacketInviteFriendRequest();
        data.m_friendName = nickname;
        GameManager.m_Instance.makePacket(data);
    }

    void clearFriendList()
    {
        for(int i = 0; i < m_friendList.Count;++i)
        {
            DestroyImmediate(m_friendList[i].gameObject);
        }
        m_friendList.Clear();
    }


}