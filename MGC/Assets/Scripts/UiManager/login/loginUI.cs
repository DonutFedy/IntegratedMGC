#define NOTLOGINSERVER
#define DEBUGMODE


using System;
using System.Collections;
using System.Collections.Generic;
using PACKET;
using PROTOCOL;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class loginUI : UI
{
    public enum INDEX_LOGIN_UI
    {
        JOIN = 0,
        LOGIN_FAIL = 1,
        SELECT_CHANNEL = 2,
        RESULT_DELETE_USER_UI,
        RESULT_UI,
        SELECT_GAME_UI,
    }


    [SerializeField]
    Image           m_backgroundImage;
    [SerializeField]
    Text            m_clientVersonText;
    [SerializeField]
    RectTransform   m_banerTransform;
    Coroutine       m_banerCoroutine;
    [SerializeField]
    float           m_fBanerSpeed;
    [SerializeField]
    int             m_nPrevPoint = 1;
    [SerializeField]
    const float     m_fCoolTime = 2;
    [SerializeField]
    GameObject[]    m_banerList;



    // login part
    [SerializeField]
    GameObject      m_loginZoneOBJ;
    [SerializeField]
    GameObject      m_userInfoOBJ;


    [SerializeField]
    InputField      m_idInputfield;
    [SerializeField]
    InputField      m_pwInputfield;

    [SerializeField]
    Text            m_userNickNameText;


    float           m_fCurCoolTime = 2;


    private void OnDisable()
    {
        StopCoroutine(m_banerCoroutine);
    }
    private void OnEnable()
    {
        // coroutine start
        m_banerCoroutine = StartCoroutine(banerCoroutine());
    }

    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
        m_banerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,512 * m_banerList.Length);
    }

    public override void update(C_BasePacket eventData)
    {
        base.update(eventData);
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        if (isExistSubUI(eventData) == true)
            return;
        // 아니면 여기서 처리
        if (m_listener != null)
            m_listener(eventData);
    }

    public override void releaseUI()
    {
        // 중단
        stopWaiting();
        clearUserInfo(false);
    }

    protected override void setUI()
    {
        // 초기화
        m_idInputfield.text = "";
        m_pwInputfield.text = "";
        m_bWaiting = false;

        m_nPrevPoint = 1;
        m_banerTransform.localPosition = Vector3.zero;
        m_fCurCoolTime = 2;

        clearUserInfo(false);

        m_idInputfield.ActivateInputField();
    }




    /// <summary>
    /// 배너 움직이는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator banerCoroutine()
    {
        int nOffset = m_banerList.Length - 1;
        m_banerTransform.localPosition = new Vector2();
        while (true)
        {
            if (m_fCurCoolTime > 0)
            {
                m_fCurCoolTime -= Time.deltaTime;
            }
            else if (nOffset > 0)
            {
                m_banerTransform.localPosition -= Vector3.right * Time.deltaTime * m_fBanerSpeed;
                if ((m_nPrevPoint * -512) >= m_banerTransform.localPosition.x)
                {
                    if (m_nPrevPoint >= nOffset)
                    {
                        m_nPrevPoint = 1;
                        yield return new WaitForSecondsRealtime(m_fCoolTime);
                    }
                    else
                    {
                        ++m_nPrevPoint;
                    }
                    m_banerTransform.localPosition = Vector3.right * -512 * (m_nPrevPoint - 1);
                    m_fCurCoolTime = m_fCoolTime;
                }
            }
            yield return 0;
        }
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

    public void tryLogin()
    {

#if NOTLOGINSERVER
        try
        {
            if (GameManager.m_Instance.connect_mainServer())
            {
                C_LoginResponsePacket userData = new C_LoginResponsePacket();
                S_UserAccessData accessData = new S_UserAccessData();
                accessData.m_accessID = m_idInputfield.text;
                accessData.m_accessPW = m_pwInputfield.text;
                GameManager.m_Instance.setAccessData(accessData);
                GameManager.m_Instance.disconnect_loginServer();
                // openMainMenu
                openMainMenuUI();
            }
        }
        catch (Exception ex)
        {
            GameManager.m_Instance.writeErrorLog("can not connect main server");
        }
        return;
#endif
#if DEBUGMODE
        //responseLogin(null);
        //return;
#endif
        if (GameManager.m_Instance.isConnectedLoginServer() == false)
            return;

        if (m_idInputfield.text.Length < 4 ||
            (m_pwInputfield.text.Length < 8))
        {
            openFailUI("<size=30>아이디나 \n비밀번호의 길이가\n너무 짧습니다</size>");
            return;
        }

        // 나중에는 
        // 1. id, pw 유효성 검사
        string id = m_idInputfield.text;
        string pw = m_pwInputfield.text;
        startWaiting(responseLogin);

        C_LoginRequestPacket eventData = new C_LoginRequestPacket();
        eventData.m_accountID = id;
        eventData.m_accountPW = pw;
        GameManager.m_Instance.makePacket(eventData);
    }

    public void openJoinUI()
    {
        if (GameManager.m_Instance.isConnectedLoginServer() == false)
        {
            return;
        }
        openUI((int)INDEX_LOGIN_UI.JOIN);
    }

    public void openChannelList()
    {
#if DEBUGMODE
        //responseChannelList(null);
        //return;
#endif
        startWaiting(responseChannelList);
        // channel list req
        C_ChannelListRequestPacket eventData = new C_ChannelListRequestPacket();
        GameManager.m_Instance.makePacket(eventData);
    }

    void responseChannelList(C_BasePacket curEventData)
    {
#if DEBUGMODE
        //try
        //{
        //    if (GameManager.m_Instance.connect_mainServer())
        //    {
        //        GameManager.m_Instance.disconnect_loginServer();
        //        // openMainMenu
        //        openMainMenuUI();
        //    }
        //}
        //catch (Exception ex)
        //{
        //    GameManager.m_Instance.writeErrorLog("can not connect main server");
        //}
        //return;
#endif
        if (curEventData.m_basicType != BasePacketType.basePacketTypeLogin) return;
        C_LoginPacket data = (C_LoginPacket)curEventData;
        if (data.m_loginType != LoginPacketType.loginPacketTypeShowChannelResponse) return;
        stopWaiting();

        C_ChannelListResponsePacket curData = (C_ChannelListResponsePacket)data;

        ((selectChannelUI)m_uiList[(int)INDEX_LOGIN_UI.SELECT_CHANNEL]).setChannel(curData.m_channelList);

        openUI((int)INDEX_LOGIN_UI.SELECT_CHANNEL);
    }

    /// <summary>
    /// 로그아웃 버튼 클릭시
    /// </summary>
    public void clickLogoutBTN()
    {
        C_BasePacket data = new C_LogoutRequestPacket();
        C_LogoutRequestPacket eventData = new C_LogoutRequestPacket();
        GameManager.m_Instance.makePacket(data);
        setLogout();
    }

    void setLogout()
    {
        C_LogoutRequestPacket eventData = new C_LogoutRequestPacket();
        GameManager.m_Instance.makePacket(eventData);

        clearUserInfo(false);
        GameManager.m_Instance.setLoginState(false);
    }

    public void clearUserInfo(bool bIsLogin)
    {
        m_userNickNameText.text = "";
        m_loginZoneOBJ.SetActive(!bIsLogin);
        m_userInfoOBJ.SetActive(bIsLogin);
    }


    void responseLogin(C_BasePacket eventData)
    {
#if DEBUGMODE
#endif
        if (eventData.m_basicType != BasePacketType.basePacketTypeLogin) return;
        C_LoginPacket data = (C_LoginPacket)eventData;
        if (data.m_loginType != LoginPacketType.loginPacketTypeLoginResponse) return;
        stopWaiting();

        C_LoginResponsePacket curData = (C_LoginResponsePacket)data;

        if(curData.m_bFlag == true) // 로그인 성공
        {
            S_UserAccessData accessData = new S_UserAccessData();
            accessData.m_accessID = m_idInputfield.text;
            accessData.m_accessPW = m_pwInputfield.text;
            GameManager.m_Instance.setAccessData(accessData);
            clearUserInfo(true);
            m_userNickNameText.text = curData.m_nickname;
        }
        else
        {
            openFailUI("<Size=40>로그인 실패!</Size>\n<size=30>아이디와 비밀번호를 다시 입력해주세요</size>");
            openUI((int)INDEX_LOGIN_UI.RESULT_UI);
        }

        m_idInputfield.text = "";
        m_pwInputfield.text = "";
    }


    /// <summary>
    /// 회원탈퇴
    /// </summary>
    public void clickDeleteUserBTN()
    {
        startWaiting(responseDeleteUser);
        C_DeleteRequestPacket eventData = new C_DeleteRequestPacket();
        GameManager.m_Instance.makePacket(eventData);
    }

    void responseDeleteUser(C_BasePacket curEventData)
    {
        if (curEventData.m_basicType != BasePacketType.basePacketTypeLogin) return;
        C_LoginPacket data = (C_LoginPacket)curEventData;
        if (data.m_loginType != LoginPacketType.loginPacketTypeDeleteResponse) return;
        stopWaiting();

        C_DeleteResponsePacket curData = (C_DeleteResponsePacket)data;

        string msg = "";
        if (curData.m_bResult)
        {
            msg = "탈퇴 성공";
            setLogout();
        }
        else
            msg = "탈퇴 실패";
        ((ResultUI)m_uiList[(int)INDEX_LOGIN_UI.RESULT_DELETE_USER_UI]).setResultMSG(msg);
        openUI((int)INDEX_LOGIN_UI.RESULT_DELETE_USER_UI);
    }
    

    public void openMainMenuUI()
    {
        m_uiManager.openUI((int)controllerUI.INDEX_OF_CONTROLLER_UI.MAINMENU_UI);
    }

    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (isExistSubUI(type) == true) return;
        else if (GameManager.m_Instance.isLogin() == false && type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            tryLogin();
        else if (type == inputKeyManager.S_KeyInput.KeyType.TAP)
            changeFocusIF();
    }
    void changeFocusIF()
    {
        if (m_idInputfield.isFocused)
        {
            m_pwInputfield.ActivateInputField();
        }
        else
        {
            m_idInputfield.ActivateInputField();
        }
    }




#region Input Field Check

    bool checkTextOnlyAscii(char c)
    {
        return (c>=97 && c<=122) || (c >= 65 && c <= 90) || (c >= 48 && c <= 57);
    }

    /// <summary>
    /// 아이디 체크
    /// </summary>
    public void CheckIdIF()
    {
        string curText = m_idInputfield.text;
        if (curText.Length > 8) // 예외 발생
        {
            openFailUI("아이디 제한은 8글자입니다");
            m_idInputfield.text = curText.Substring(0,8);
            m_idInputfield.ActivateInputField();
        }
        else // 영어 및 숫자 제외의 문자는 경고
        {
            for(int i = 0; i < curText.Length; ++i)
            {
                if(checkTextOnlyAscii(curText[i]) == false)
                {
                    openFailUI("영어와 숫자만 가능합니다");
                    m_idInputfield.text = curText.Substring(0, curText.Length-1);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 비번 체크
    /// </summary>
    public void CheckPwIF()
    {
        string curText = m_pwInputfield.text;
        if (m_pwInputfield.text.Length > 16) // 예외 발생
        {
            openFailUI("비밀번호 제한은 16글자입니다");
            m_pwInputfield.text = m_pwInputfield.text.Substring(0, 16);
            m_pwInputfield.ActivateInputField();
        }
        else // 영어 및 숫자 제외의 문자는 경고
        {
            for (int i = 0; i < curText.Length; ++i)
            {
                if (checkTextOnlyAscii(curText[i]) == false)
                {
                    openFailUI("영어와 숫자만 가능합니다");
                    m_pwInputfield.text = curText.Substring(0, curText.Length - 1);
                    return;
                }
            }
        }
    }

#endregion


#region UI CLOSE / OPEN

    void openFailUI(string errorMSG)
    {
        ((ResultUI)m_uiList[(int)INDEX_LOGIN_UI.RESULT_UI]).setResultMSG(errorMSG);
        openUI((int)INDEX_LOGIN_UI.RESULT_UI);
    }

    public void onErrorClient(string errorMsg)
    {
        ((ResultUI)m_uiList[(int)INDEX_LOGIN_UI.RESULT_UI]).setResultMSG(errorMsg);
        openUI((int)INDEX_LOGIN_UI.RESULT_UI);
    }

    public void openSelectGameUI()
    {
        openUI((int)INDEX_LOGIN_UI.SELECT_GAME_UI);
    }

    #endregion



}
