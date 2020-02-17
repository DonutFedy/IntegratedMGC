#define DEBUGMODE
using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class joinUI : UI
{
    public enum INDEX_JOIN_UI
    {
        TRY_JOIN = 0,
        RESULT_UI,
    }

    [SerializeField]
    InputField          m_idInputField;
    [SerializeField]
    InputField          m_pwInputField;
    [SerializeField]
    InputField          m_nicknameInputField;

    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
        base.update(eventData);
        // 만약 서브 ui가 열려있다면 그쪽으로 throw
        if (isExistSubUI(eventData) == true)
            return;
        // 아니면 여기서 처리
        if(m_listener!=null)
            m_listener(eventData);
    }

    public override void releaseUI()
    {
        // 중단
        clearIF();
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

    void clearIF()
    {
        m_idInputField.text = "";
        m_pwInputField.text = "";
        m_nicknameInputField.text = "";
    }

    public void tryJoin()
    {
        if  (m_idInputField.text.Length <= 0 ||
            (m_pwInputField.text.Length <= 0)||
            (m_nicknameInputField.text.Length <= 0)) return;

#if DEBUGMODE
        //responseJoin(null);
        //return;
#endif
        startWaiting(responseJoin);
        C_SignupRequestPacket data = new C_SignupRequestPacket();
        data.m_accountID = m_idInputField.text;
        data.m_accountPW = m_pwInputField.text;
        data.m_nickname  = m_nicknameInputField.text;
        GameManager.m_Instance.makePacket(data);
    }

    public void exitUI()
    {
        m_uiManager.closeUI(1);
    }

    void responseJoin(C_BasePacket eventData)
    {
#if DEBUGMODE

#endif
        if (eventData.m_basicType != BasePacketType.basePacketTypeLogin) return;

        C_LoginPacket data = (C_LoginPacket)eventData;
        if (data.m_loginType != LoginPacketType.loginPacketTypeSignupResponse) return;

        C_SignupResponsePacket curData = (C_SignupResponsePacket)data;

        stopWaiting();
        if(curData.m_bResult)
        {
            ((tryJoinUI)m_uiList[(int)INDEX_JOIN_UI.TRY_JOIN]).setSuccess(curData.m_bResult);
        }
        else
        {
            ((tryJoinUI)m_uiList[(int)INDEX_JOIN_UI.TRY_JOIN]).setSuccess(curData.m_bResult,"다시 시도해 주세요");
        }
        openUI((int)INDEX_JOIN_UI.TRY_JOIN);
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            tryJoin();
        else if (type == inputKeyManager.S_KeyInput.KeyType.ESC)
            exitUI();
    }

    #region Input Field Check

    bool checkTextOnlyAscii(char c)
    {
        return (c >= 97 && c <= 122) || (c >= 65 && c <= 90) || (c >= 48 && c <= 57);
    }
    /// <summary>
    /// 아이디 체크
    /// </summary>
    public void CheckIdIF()
    {
        string curText = m_idInputField.text;
        if (curText.Length > 8) // 예외 발생
        {
            openFailUI("아이디 제한은 8글자입니다");
            m_idInputField.text = curText.Substring(0, 8);
            m_idInputField.ActivateInputField();
        }
        else // 영어 및 숫자 제외의 문자는 경고
        {
            for (int i = 0; i < curText.Length; ++i)
            {
                if (checkTextOnlyAscii(curText[i]) == false)
                {
                    openFailUI("영어와 숫자만 가능합니다");
                    m_idInputField.text = curText.Substring(0, curText.Length - 1);
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
        string curText = m_pwInputField.text;
        if (m_pwInputField.text.Length > 16) // 예외 발생
        {
            openFailUI("비밀번호 제한은 16글자입니다");
            m_pwInputField.text = curText.Substring(0, 16);
            m_pwInputField.ActivateInputField();
        }
        else // 영어 및 숫자 제외의 문자는 경고
        {
            for (int i = 0; i < curText.Length; ++i)
            {
                if (checkTextOnlyAscii(curText[i]) == false)
                {
                    openFailUI("영어와 숫자만 가능합니다");
                    m_pwInputField.text = curText.Substring(0, curText.Length - 1);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 닉네임 체크
    /// </summary>
    public void CheckNicknameIF()
    {
        if (m_nicknameInputField.text.Length > 8) // 예외 발생
        {
            openFailUI("닉네임 제한은 8글자입니다");
            m_nicknameInputField.text = m_nicknameInputField.text.Substring(0, 8);
        }
    }

    #endregion


    #region UI CLOSE / OPEN

    void openFailUI(string errorMSG)
    {
        ((ResultUI)m_uiList[(int)INDEX_JOIN_UI.RESULT_UI]).setResultMSG(errorMSG);
        openUI((int)INDEX_JOIN_UI.RESULT_UI);
    }



    #endregion
}