using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class singleGameUI : UI
{
    enum INDEX_OF_SINGLEGAME_UI
    {
        RESULT_UI,
    }

    [SerializeField]
    singleGameManager           m_singleGameMGR;


    [SerializeField]
    Text                        m_hpText;
    [SerializeField]
    Text                        m_floorText;
    int                         m_nCurFloor;
    bool                        m_bEndGame;
    Coroutine                   m_onHitCoroutine;
    

    float                       m_fOnHitWarningTime = 1;
    float                       m_fCurOnHitWarningTime;
    [SerializeField]
    Image                       m_onHitWarningOBJ;


    #region Basic
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




        m_singleGameMGR.closeGame();
    }

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;
        m_bEndGame = true;

        // func setting
        m_singleGameMGR.m_funcList[(int)singleGameManager.FUNC_TYPE.START_GAME] = startGame;
        m_singleGameMGR.m_funcList[(int)singleGameManager.FUNC_TYPE.ON_HIT]     = onHit;
        m_singleGameMGR.m_funcList[(int)singleGameManager.FUNC_TYPE.UP_FLOOR]   = upFloor;
        m_singleGameMGR.setGame();
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
        if (type == inputKeyManager.S_KeyInput.KeyType.ESC)
            exitUI(1);
    }
    #endregion


    #region GAME

    void startGame(object data)
    {
        Debug.Log("start game");
        m_bEndGame = false;
        int nCurHP = (int)data;
        // text update
        m_hpText.text = nCurHP + "";
        m_floorText.text = 1+"층";

        // start game ui popup
        // -> ui close 시에 startGame 호출
        string titleText = "<size=50>게임 시작!</size>\n<Size=30>돌아다니는 얼음을 피해 위로 올라가자!\n맞으면 아프니 조심!</Size>";
        ((ResultUI)m_uiList[(int)INDEX_OF_SINGLEGAME_UI.RESULT_UI]).setResultMSG(titleText, 1);
        openUI((int)INDEX_OF_SINGLEGAME_UI.RESULT_UI);
    }

    void endGame()
    {
        if (m_bEndGame) return;
        m_bEndGame = true;
        Debug.Log("end game");
        if(m_onHitCoroutine != null)
            StopCoroutine(m_onHitCoroutine);
        // end game ui popup
        // -> 종료
        ((ResultUI)m_uiList[(int)INDEX_OF_SINGLEGAME_UI.RESULT_UI]).setResultMSG("<size=70>게임 종료!</size>\n<Size=50>도달 층 수 : "+m_nCurFloor+" 층</size>", 2);
        openUI((int)INDEX_OF_SINGLEGAME_UI.RESULT_UI);
    }

    void onHit(object data)
    {
        if (m_bEndGame) return;
        Debug.Log("on hit");
        int nCurHP = (int)data;
        // text update
        m_hpText.text = nCurHP+"";

        if (nCurHP <= 0)
            endGame();
        else
            m_onHitCoroutine = StartCoroutine(onHitCoroutine());
    }

    
    IEnumerator onHitCoroutine()
    {
        m_fCurOnHitWarningTime = 0;
        m_onHitWarningOBJ.gameObject.SetActive(true);
        Color curColor = m_onHitWarningOBJ.color;
        curColor.a = 0;
        while (m_fCurOnHitWarningTime < m_fOnHitWarningTime)
        {
            m_fCurOnHitWarningTime += Time.deltaTime;

            curColor.a = m_fCurOnHitWarningTime / (m_fOnHitWarningTime / 2);
            if (curColor.a>=1)
            {
                curColor.a = 2 - curColor.a;
            }
            m_onHitWarningOBJ.color = curColor;
            yield return null;
        }
        m_onHitWarningOBJ.gameObject.SetActive(false);
    }

    void upFloor(object data)
    {
        if (m_bEndGame) return;
        m_nCurFloor = (int)data;
        m_floorText.text = m_nCurFloor + "층";
    }


    public override void setOnCloseSubUI()
    {
        if(m_bEndGame==false)
            m_singleGameMGR.startGame();
    }

    #endregion

}