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

    object              m_roomInfo;
    string              m_changeIP = "";
    int                 m_nChangePort = 0;
    string              m_changeChannelName = "";

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
    public void setInviteInfo()
    {
        // setting data
        m_roomInfo = null;
        m_changeIP = "127.0.0.1";
        m_nChangePort = 10011;
        m_changeChannelName = "테스트채널";

        // ui set
        m_gameRoomNameText.text ="";
        m_gameTypeText.text = "";
        m_srcUserNameText.text = "";
    }

    #endregion


    #region Event

    public void clickAcceptanceBTN()
    {
        GameManager.m_Instance.changeMainServer(m_changeIP, m_nChangePort, m_changeChannelName, m_roomInfo);
    }


    #endregion



    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }

}
