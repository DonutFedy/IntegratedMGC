using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guildInfoUI : UI
{
    public enum INDEX_OF_GUILDINFO_UI
    {
        RESULT_UI,
    }

    [SerializeField]
    Text                    m_guildNameText;
    [SerializeField]
    Text                    m_guildRankText;
    [SerializeField]
    Text                    m_guildInfoText;

    //S_GUILD_INFO            m_curGuildInfo;

    bool                    m_bIsSearchMode;

    // search mode
    [SerializeField]
    GameObject              m_JoinBTN;

    // info mode


    #region Basic UI
    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
    }
    public override void update(C_BasePacket eventData)
    {
        //// 만약 서브 ui가 열려있다면 그쪽으로 throw
        //if (isExistSubUI(eventData) == true)
        //    return;
        //else if (m_listener != null)
        //    m_listener(eventData);
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

    public override void setOnCloseSubUI()
    {
        exitUI(1);
    }

    #endregion

    #region EVENT
    public void clickRequestJoinGuild()
    {
        requestJoinGuild();
    }


    #endregion


    #region Server REQ / RES

    /// <summary>
    /// 길드 가입 요청
    /// </summary>
    void requestJoinGuild()
    {
        //startWaiting(requestJoinGuild);
        //eventData data = new guildJoinRequest();
        //data.deserialize(m_curGuildInfo.m_name);
        //GameManager.m_Instance.makePacket(data);
        requestJoinGuild(null);
    }

    void requestJoinGuild(C_BasePacket curEventData)
    {
        //stopWaiting();
        //socialData curSocialData = (socialData)curEventData;
        //if (curSocialData.m_type != SOCIAL_TYPE.GUILD_JOIN_RESPONSE) return; // 현재 이벤트에 맞는 타입이 아니면 return

        //guildJoinResponse data = (guildJoinResponse)curSocialData; // 실제 데이터

        if (false) // 가입 실패
        {
            // 실패UI 팝업
            ((ResultUI)m_uiList[(int)INDEX_OF_GUILDINFO_UI.RESULT_UI]).setResultMSG("길드 가입 실패");
            openUI((int)INDEX_OF_GUILDINFO_UI.RESULT_UI);
        }
        else
        {
            // 플레이어 정보 갱신
            //GameManager.m_Instance.setUserGuildData(true, m_curGuildInfo);

            // 성공UI 팝업
            ((ResultUI)m_uiList[(int)INDEX_OF_GUILDINFO_UI.RESULT_UI]).setResultMSG("길드 가입 성공");
            openUI((int)INDEX_OF_GUILDINFO_UI.RESULT_UI);
        }


    }
    #endregion



    #region UI CLOSE/OPEN
    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }

    public void setGuildInfo(bool bIsSearchMode)
    {
        //m_curGuildInfo = info;

        //m_guildNameText.text = info.m_name;
        //m_guildRankText.text = info.m_rank+"위";
        //m_guildInfoText.text = info.m_info;

        m_bIsSearchMode = bIsSearchMode;
        if(m_bIsSearchMode) // search 모드에서 부른 것이라면
        {
            m_JoinBTN.SetActive(true);
        }
        else                // info 모드에서 부른 것이라면
        {
            m_JoinBTN.SetActive(false);
        }
    }

    #endregion
}