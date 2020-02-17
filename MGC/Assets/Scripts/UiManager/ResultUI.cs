using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : UI
{
    [SerializeField]
    Text m_msgText;

    int m_nCloseCount = 0;
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

    public void setResultMSG(string resultMSG = "알수없는 이유", int nCloseCount = 1)
    {
        m_msgText.text = resultMSG;
        m_nCloseCount = nCloseCount;
    }

    public void exitUI(int nCount)
    {
        if (m_nCloseCount > nCount)
        {
            nCount = m_nCloseCount;
            m_nCloseCount = 0;
        }
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (type == inputKeyManager.S_KeyInput.KeyType.ENTER)
            exitUI(1);
    }

}