using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tryJoinUI : UI
{
    bool            m_bSuccess;
    [SerializeField]
    Text            m_titleText;
    [SerializeField]
    Text            m_msgText;

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

    public void setSuccess(bool bSuccess, string failMSG = "환영합니다")
    {
        m_bSuccess = bSuccess;
        if (m_bSuccess)
        {
            m_titleText.text = "회원가입 성공";
            m_msgText.text = failMSG;
        }
        else
        {
            m_titleText.text = "회원가입 실패";
            m_msgText.text = "가입 실패!!\n"+failMSG;
        }
    }


    public void exitUI()
    {
        if (m_bSuccess)
            m_uiManager.closeUI(2);
        else
            m_uiManager.closeUI(1);
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (type == inputKeyManager.S_KeyInput.KeyType.ESC)
            exitUI();
        else if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            exitUI();
    }
}