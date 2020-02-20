using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inviteListUI : UI
{
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

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;

        //waiting
        startWaiting(reponseFriendList);
        C_SocialPacketFriendListRequest data = new C_SocialPacketFriendListRequest();
        GameManager.m_Instance.makePacket(data);
    }

    void reponseFriendList(C_BasePacket eventData)
    {
        if (eventData.m_basicType != BasePacketType.basePacketTypeSocial) return;
        C_BaseSocialPacket data = (C_BaseSocialPacket)eventData;
        if (data.m_socialType != SocialPacketType.packetTypeSocialFriendListResponse) return;
        C_SocialPacketFriendListResponse curData = (C_SocialPacketFriendListResponse)data;

        stopWaiting();

        // data setting....


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

}