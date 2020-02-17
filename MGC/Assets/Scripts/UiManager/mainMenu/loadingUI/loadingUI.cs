using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingUI : UI
{
    enum INDEX_OF_LODING_UI
    {
        UPUP_GAME,

    }

    [SerializeField]
    List<Sprite>        m_spriteList = new List<Sprite>();
    [SerializeField]
    Image               m_loadingCharacterImage;
    [SerializeField]
    Image               m_loadingBarImage;
    [SerializeField]
    Text                m_loadingText;

    float               m_fCurLoadingTime;
    float               m_fCurLoadingState;
    [SerializeField]
    float               m_fCharacterImageSpeed;


    Coroutine           m_loadingCoroutine;



    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅
    }
    public override void update(C_BasePacket eventData)
    {
        // type check . if preData Type => setData
        if (eventData.m_basicType == BasePacketType.basePacketTypePreLoad)
            recvLoadingData(eventData);
    }

    public override void releaseUI()
    {
        // 중단
        if (m_loadingCoroutine != null)
            StopCoroutine(m_loadingCoroutine);
    }
    private void OnDisable()
    {
        if (m_loadingCoroutine != null)
            StopCoroutine(m_loadingCoroutine);
    }

    protected override void setUI()
    {
        // 초기화
        m_loadingCoroutine = StartCoroutine(loadingData());
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


    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
        m_uiManager.setOnCloseSubUI();
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (isExistSubUI(type) == true) return;
    }

    #region LOADING

    IEnumerator loadingData()
    {
        m_fCurLoadingState = 0;
        m_fCurLoadingTime = 0.2f;
        int nCurImageIndex = -1;
        Color nowColor = Color.white;
        while (m_fCurLoadingState<1)
        {
            m_fCurLoadingTime += Time.deltaTime;

            if (m_fCurLoadingTime > 0.5f)
                nowColor.a = 2 - m_fCurLoadingTime/ 0.5f;
            else
                nowColor.a = 1-m_fCurLoadingTime /0.5f;
            m_loadingText.color = nowColor;
            if (m_fCurLoadingTime* m_fCharacterImageSpeed >= 1f)
            {
                m_fCurLoadingTime = 0;
                ++nCurImageIndex;
                if (nCurImageIndex >= m_spriteList.Count)
                    nCurImageIndex = 0;
                m_loadingCharacterImage.sprite = m_spriteList[nCurImageIndex];
            }
            yield return null;
        }

        exitUI(1);
    }

    void recvLoadingData(C_BasePacket eventData)
    {
        C_BasePreLoadPacket data = (C_BasePreLoadPacket)eventData;

        if (data.m_preLoadType != PreLoadType.preLoadPlayerInfo) return;

        C_PreLoadPacketLoadPlayerInfo curData = (C_PreLoadPacketLoadPlayerInfo)data;

        GameManager.m_Instance.setUserData(curData);
        // 일단은 1개만 받으면 되니 완료 시킴
        m_fCurLoadingState = 1;
    }

    #endregion
}