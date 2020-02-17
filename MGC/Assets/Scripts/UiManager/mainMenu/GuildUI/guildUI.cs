using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guildUI : UI
{
    public enum INDEX_OF_GUILD_UI
    {
        MAKE_GUILD_UI,
        RESULT_UI,
        GUILD_INFO_UI,
    }
    
    [SerializeField]
    GameObject          m_GuildMemberSlotPrefabs;

    bool                m_bExistGuild;

    #region GuildInfo Zone

    //S_GUILD_INFO        m_guildInfo;



    [SerializeField]
    GameObject          m_guildInfoZoneOBJ;
    [SerializeField]
    RectTransform       m_parentOfGuildInfoList; // re size y
    [SerializeField]
    Text                m_guildNameText;
    [SerializeField]
    GameObject          m_EmptyRandomListOBJ;   // 길드 랜덤 리스트가 없을시
    List<guildMemberSlot>   m_guildMemeberSlotList;
    [SerializeField]
    int                 m_guildSlotOffsetSizeY;

    #region InfoOBJ
    [SerializeField]
    GameObject          m_infoOBJ;
    [SerializeField]
    Text                m_infoText;
    #endregion

    #endregion

    #region GuildSearch Zone
    [SerializeField]
    GameObject              m_guildSearchZoneOBJ;
    [SerializeField]
    InputField              m_guildSearchIF;
    [SerializeField]
    RectTransform           m_parentOfGuildSearchList; // re size y
    List<guildInfoSlot>     m_searchSlotList;
    [SerializeField]
    guildInfoSlot           m_guildInfoSlotPrefab;


    [SerializeField]
    InputField              m_searchGuildNameIF;

    #endregion

    #region CHAT BOX WHISPER
    public delegate void setWhisperTarget(string nickname);
    public setWhisperTarget m_dWhisper;

    #endregion

    #region Basic UI
    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
        m_searchSlotList = new List<guildInfoSlot>();
        m_guildMemeberSlotList = new List<guildMemberSlot>();
    }
    public override void update(C_BasePacket eventData)
    {
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        if (isExistSubUI(eventData) == true)
            return;
        else if (m_listener != null)
            m_listener(eventData);
    }

    public override void releaseUI()
    {
        // 중단
        clearObjList();
    }

    protected override void setUI()
    {

        // 초기화
        stopWaiting();
        m_infoOBJ.SetActive(false);
        m_guildInfoZoneOBJ.SetActive(false);
        m_guildSearchZoneOBJ.SetActive(false);


        isEmptyGuildList(true);
        // 1. 일단 false 로해놓고 서버에 요청
        m_bExistGuild = false;
        // guild info req
        requestGuildInfo();

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


    void clearObjList()
    {
        // guildInfo list clear
        clearMemeberList();

        // guildsaerch list clear
        clearSearchList();
    }

    void clearMemeberList()
    {
        for (int i = 0; i < m_guildMemeberSlotList.Count; ++i)
            DestroyImmediate(m_guildMemeberSlotList[i].gameObject);
        m_guildMemeberSlotList.Clear();
    }

    void clearSearchList()
    {
        for (int i = 0; i < m_searchSlotList.Count; ++i)
            DestroyImmediate(m_searchSlotList[i].gameObject);
        m_searchSlotList.Clear();
    }

    public override void setOnCloseSubUI()
    {
        if (m_guildInfoZoneOBJ.activeSelf)
        {
            if (GameManager.m_Instance.getExistGuild() == false)
            {
                openGuildSearchZone();
            }
        }
        else
        {
            if (GameManager.m_Instance.getExistGuild())
            {
                //openGuildInfoZone(GameManager.m_Instance.getGuildInfo());
            }
            else
            {
                onFocusIF();
                requestRandomList();
            }
        }
    }
    #endregion


    #region Event

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
    /// 서버에 길드 탈퇴 요청
    /// </summary>
    public void clickWithdrawalGuildBTN()
    {
        requestWidthdrawalGuild();
    }

    /// <summary>
    /// 길드 정보 UI를 open
    /// </summary>
    public void clickOpenShowInfoUI()
    {
        //showGuildInfo(false, m_guildInfo);
    }
    public void clickRequestSearchGuild()
    {
        requestSearchGuild();
    }

    void onFocusIF()
    {
        if(m_guildInfoZoneOBJ.activeSelf)
        {
        }
        else
        {
            m_guildSearchIF.ActivateInputField();
            m_guildSearchIF.text = "";
        }
    }

    /// <summary>
    /// 랜덤 길드 리스트 존재시, list obj 활성화...
    /// </summary>
    /// <param name="bEmpty"></param>
    void isEmptyGuildList(bool bEmpty)
    {
        m_EmptyRandomListOBJ.SetActive(bEmpty);
        m_parentOfGuildSearchList.gameObject.SetActive(!bEmpty);
    }

    #endregion 

    #region Server REQ / RES

    /// <summary>
    /// 길드 정보를 서버로부터 요청한다.
    /// </summary>
    void requestGuildInfo()
    {
        //startWaiting(responseGuildInfo);

        //eventData data = new guildInfoRequest();
        //GameManager.m_Instance.makePacket(data);
    }


    /// <summary>
    /// 길드 정보 responce 처리 함수
    /// </summary>
    /// <param name="curEventData"></param>
    public void responseGuildInfo(C_BasePacket curEventData)
    {
        //stopWaiting();
        //socialData curSocialData = (socialData)curEventData;
        //if (curSocialData.m_type != SOCIAL_TYPE.GUILD_INFO_RESPONSE) return; // 현재 이벤트에 맞는 타입이 아니면 return

        //guildInfoResponse data = (guildInfoResponse)curSocialData; // 실제 데이터

        //if( data.m_eventData == null)
        //    openGuildSearchZone();          // 길드 없을 시(data == null), 길드 search zone 활성화
        //else
        //    openGuildInfoZone((S_GUILD_INFO)data.m_eventData);            // 길드 존재 시, 길드 info zone 활성화
    }
    void requestRandomList()
    {
        //startWaiting(responseRandomList);

        //eventData data = new guildRandomListRequest();
        //GameManager.m_Instance.makePacket(data);
    }


    /// <summary>
    /// 길드 정보 responce 처리 함수
    /// </summary>
    /// <param name="curEventData"></param>
    void responseRandomList(C_BasePacket curEventData)
    {
        //stopWaiting();
        //socialData curSocialData = (socialData)curEventData;
        //if (curSocialData.m_type != SOCIAL_TYPE.GUILD_RANDOM_LIST_RESPONSE) return; // 현재 이벤트에 맞는 타입이 아니면 return

        //guildRandomListResponse data = (guildRandomListResponse)curSocialData; // 실제 데이터

        //if (data.m_eventData != null)
        //{
        //    // 에러 MSG 팝업
        //    ((ResultUI)m_uiList[(int)INDEX_OF_GUILD_UI.RESULT_UI]).setResultMSG("길드 리스트를 얻어 올 수 없습니다.");
        //    openUI((int)INDEX_OF_GUILD_UI.RESULT_UI);
        //    isEmptyGuildList(true);
        //}
        //else
        //{
        //    clearSearchList();
        //    // 리스트 만들어주기
        //    Debug.Log("리스트 만들어주기");

        //    // 임시로 3개의 길드 생성
        //    for(int i = 0; i < 3; ++i)
        //    {
        //        guildInfoSlot newSlot = Instantiate(m_guildInfoSlotPrefab, m_parentOfGuildSearchList).GetComponent<guildInfoSlot>();
        //        S_GUILD_INFO newInfo = new S_GUILD_INFO();
        //        newInfo.m_name = "테스트길드" + i;
        //        newInfo.m_rank = (byte)(i + 1);
        //        newInfo.m_info = "대충 길드 정보";
        //        newSlot.setSlot(newInfo, showGuildInfo);
        //        m_searchSlotList.Add(newSlot);
        //    }

        //    isEmptyGuildList(false);
        //}
        //m_guildSearchZoneOBJ.SetActive(true);
    }

    void requestSearchGuild()
    {
        if (m_guildSearchIF.text.Length <= 0) return; // 적합성 검사

        //startWaiting(responseSearchGuild);

        //eventData data = new guildSearchRequest();
        //data.deserialize(m_guildSearchIF.text);
        //GameManager.m_Instance.makePacket(data);
    }

    /// <summary>
    /// 길드 검색 responce 처리 함수
    /// </summary>
    /// <param name="curEventData"></param>
    public void responseSearchGuild(C_BasePacket curEventData)
    {
        //stopWaiting();
        //socialData curSocialData = (socialData)curEventData;
        //if (curSocialData.m_type != SOCIAL_TYPE.GUILD_SEARCH_RESPONSE) return; // 현재 이벤트에 맞는 타입이 아니면 return

        //guildSearchResponse data = (guildSearchResponse)curSocialData; // 실제 데이터
        //onFocusIF();

        //if (data.m_eventData == null)
        //{
        //    // 기존 랜덤 리스트는 유지
        //    ((ResultUI)m_uiList[(int)INDEX_OF_GUILD_UI.RESULT_UI]).setResultMSG("해당 길드를 찾을 수 없습니다!");
        //    openUI((int)INDEX_OF_GUILD_UI.RESULT_UI);       // 해당 길드 없다고 팝업
        //}
        //else
        //{
        //    clearSearchList();      // 기존 랜덤 리스트 clear
        //    // 검색된 길드 slot 생성
        //    guildInfoSlot newSlot = Instantiate(m_guildInfoSlotPrefab, m_parentOfGuildSearchList).GetComponent<guildInfoSlot>();
        //    S_GUILD_INFO newInfo = (S_GUILD_INFO)data.m_eventData;
        //    newSlot.setSlot(newInfo, showGuildInfo);
        //    m_searchSlotList.Add(newSlot);
        //}
    }

    void requestWidthdrawalGuild()
    {
        //startWaiting(responseWithdrawalGuild);
        //eventData data = new guildWithdrawalRequest();
        //GameManager.m_Instance.makePacket(data);
    }

    void responseWithdrawalGuild(C_BasePacket curEventData)
    {
        //stopWaiting();
        //socialData curSocialData = (socialData)curEventData;
        //if (curSocialData.m_type != SOCIAL_TYPE.GUILD_WITHDRAWAL_RESPONSE) return; // 현재 이벤트에 맞는 타입이 아니면 return

        //guildWithdrawalResponse data = (guildWithdrawalResponse)curSocialData; // 실제 데이터

        //if(data.m_eventData == null || (byte)data.m_eventData != 1)
        //{
        //    // 실패
        //    ((ResultUI)m_uiList[(int)INDEX_OF_GUILD_UI.RESULT_UI]).setResultMSG("길드 탈퇴 실패!!");
        //    openUI((int)INDEX_OF_GUILD_UI.RESULT_UI);
        //}
        //else if( (byte)data.m_eventData == 1) // 성공
        //{
        //    // 클라이언트 유저 데이터 갱신
        //    GameManager.m_Instance.setUserGuildData(false, new S_GUILD_INFO());

        //    ((ResultUI)m_uiList[(int)INDEX_OF_GUILD_UI.RESULT_UI]).setResultMSG("길드 탈퇴 성공!!");
        //    openUI((int)INDEX_OF_GUILD_UI.RESULT_UI);
        //}
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

    

    /// <summary>
    /// 길드 정보 UI 열기
    /// 세팅 정보 보내줘야함
    /// </summary>
    /// <param name="data"></param>
    void openGuildInfoZone()
    {
        clearMemeberList();
        m_guildInfoZoneOBJ.SetActive(true);
        m_guildSearchZoneOBJ.SetActive(false);

        // 플레이어 정보 갱신
        //m_guildInfo = data;
        //GameManager.m_Instance.setUserGuildData(true, m_guildInfo);
        //m_guildNameText.text = m_guildInfo.m_name;

        //// 임시 유저 데이터 리스트 생성
        //int nSize = 8;
        //for(int i = 0; i < nSize; ++i)
        //{
        //    S_GUILD_MEMBER info = new S_GUILD_MEMBER();
        //    info.m_nickname = "TestName" + i;
        //    info.m_nRank = i;
        //    info.m_nScore = i * 100;
        //    info.m_positionType = S_GUILD_MEMBER.GUILD_POSITION.NORMAL;
        //    if (i == 0)
        //        info.m_positionType = S_GUILD_MEMBER.GUILD_POSITION.MASTER;
        //    guildMemberSlot newSlot = Instantiate(m_GuildMemberSlotPrefabs, m_parentOfGuildInfoList).GetComponent<guildMemberSlot>();
        //    newSlot.setSlot(info, clickWhisper);
        //    m_guildMemeberSlotList.Add(newSlot);
        //}
        //// userData add
        //S_GUILD_MEMBER userInfo = new S_GUILD_MEMBER();
        //userInfo.m_nickname = GameManager.m_Instance.getUserNickName();
        //userInfo.m_nRank = 0;
        //userInfo.m_nScore = 0;
        //userInfo.m_positionType = S_GUILD_MEMBER.GUILD_POSITION.NORMAL;
        //guildMemberSlot userSlot = Instantiate(m_GuildMemberSlotPrefabs, m_parentOfGuildInfoList).GetComponent<guildMemberSlot>();
        //userSlot.setSlot(userInfo, clickWhisper);
        //m_guildMemeberSlotList.Add(userSlot);

        //// re size RectTransform Y
        //m_parentOfGuildInfoList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10+m_guildSlotOffsetSizeY * m_guildMemeberSlotList.Count);

        onFocusIF();
    }

    void openGuildSearchZone()
    {
        requestRandomList();
        m_guildInfoZoneOBJ.SetActive(false);
        m_guildSearchZoneOBJ.SetActive(true);
        onFocusIF();
    }

    /// <summary>
    /// 길드 소개 BTN 클릭시 해당 UI 호출
    /// </summary>
    /// <param name="infoText"></param>
    public void openInfoOBJ()
    {
        //m_infoText.text = m_guildInfo.m_info;
    }
    /// <summary>
    /// INFO OBJ close
    /// </summary>
    public void closeInfoOBJ()
    {
        m_infoOBJ.SetActive(false);
    }

    public void openMakeGuildUI()
    {
        openUI((int)INDEX_OF_GUILD_UI.MAKE_GUILD_UI);
    }

    void showGuildInfo(bool bIsSearchMode)
    {
        //((guildInfoUI)m_uiList[(int)INDEX_OF_GUILD_UI.GUILD_INFO_UI]).setGuildInfo(bIsSearchMode, info);
        openUI((int)INDEX_OF_GUILD_UI.GUILD_INFO_UI);
    }
    #endregion
}