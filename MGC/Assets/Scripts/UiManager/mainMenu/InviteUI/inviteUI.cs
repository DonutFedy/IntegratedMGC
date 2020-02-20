using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inviteUI : UI
{
    [SerializeField]
    Text                m_gameRoomNameText;
    [SerializeField]
    Text                m_gameTypeText;
    [SerializeField]
    Text                m_srcUserNameText;

    S_GameRoomInfo      m_roomInfo;
    S_GameServerInfo    m_gameServerInfo;

    #region Basic UI

    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
    }
    public override void update(C_BasePacket eventData)
    {

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
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            exitUI(1);
    }

    /// <summary>
    /// 초대 UI 를 열기 전에 세팅
    /// </summary>
    public void setInviteInfo(C_SocialPacketConfirmInviteFriendRequest data)
    {
        // setting data
        m_roomInfo          = data.m_gameroomInfo;
        m_gameServerInfo    = data.m_gameServerInfo;

        // ui set
        m_gameRoomNameText.text         = m_roomInfo.m_roomName;
        m_gameTypeText.text             = myApi.GetDescription( m_roomInfo.m_gameMode );
        m_srcUserNameText.text          = data.m_friendName;
    }

    #endregion


    #region Event

    public void clickAcceptanceBTN()
    {
        GameManager.m_Instance.changeMainServer(m_gameServerInfo, m_roomInfo);
    }


    #endregion



    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }

}
