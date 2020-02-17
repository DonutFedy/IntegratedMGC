using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guildMemberSlot : MonoBehaviour
{
    [SerializeField]
    Text                    m_memberNameText;
    [SerializeField]
    Text                    m_memberRankText;
    [SerializeField]
    Text                    m_memberScoreText;
    [SerializeField]
    Image                   m_positionImage;
    //...guild info
    //S_GUILD_MEMBER          m_memberInfo;

    [SerializeField]
    RectTransform           m_viewTRANS;
    [SerializeField]
    float                   m_fViewSpeed;
    int                     m_nDirection;

    float                   m_fLimit;
    float                   m_fCurOffset;

    public int              m_LimitSize;

    Coroutine               m_viewCoroutine;
    bool                    m_bIsRunning;

    #region CHAT BOX WHISPER
    public delegate void setWhisperTarget(string nickname);
    public setWhisperTarget m_dWhisper;

    #endregion

    /// <summary>
    /// 삭제 버튼 클릭 이벤트 세팅
    /// </summary>
    public void setSlot(object info, setWhisperTarget dSeTWhisper)
    {
        m_fLimit = m_LimitSize;
        m_nDirection = -1;
        m_bIsRunning = false;

        m_dWhisper = null;
        m_dWhisper = dSeTWhisper;

        //m_memberInfo = info;
        //m_memberScoreText.text = m_memberInfo.m_nScore+"점";
        //m_memberRankText.text = m_memberInfo.m_nRank + "위";
        //m_memberNameText.text = m_memberInfo.m_nickname;

        resourceManager.UIspriteINDEX type = resourceManager.UIspriteINDEX.GUILD_MASTER;
        //switch (m_memberInfo.m_positionType)
        //{
        //    case S_GUILD_MEMBER.GUILD_POSITION.NORMAL:
        //        type = resourceManager.UIspriteINDEX.GUILD_NORMAL;
        //        break;
        //}
        m_positionImage.sprite = GameManager.m_Instance.getUIsprite(type);
    }

    private void OnDestroy()
    {
        if(m_viewCoroutine!=null)
            StopCoroutine(m_viewCoroutine);
    }
    private void OnDisable()
    {
        if (m_viewCoroutine != null)
            StopCoroutine(m_viewCoroutine);
    }

    public void onClick()
    {
        if (m_bIsRunning)
            return;
        m_nDirection *= -1;
        m_viewCoroutine = StartCoroutine(viewChange());
    }

    /// <summary>
    /// 귓속말 BTN 클릭시
    /// </summary>
    public void clickWhisper()
    {
        //if (m_dWhisper != null)
        //    m_dWhisper(m_memberInfo.m_nickname);
    }

    IEnumerator viewChange()
    {
        m_bIsRunning = true;
        Vector3 prevViewPosition = m_viewTRANS.localPosition;

        m_fCurOffset = 0;
        float fCurLimit = m_fLimit;
        Debug.Log(m_nDirection +"///"+ fCurLimit);
        while(m_fCurOffset < fCurLimit)
        {
            m_fCurOffset += Time.deltaTime * m_fViewSpeed;
            m_fLimit = m_fCurOffset;
            m_viewTRANS.localPosition = prevViewPosition+Vector3.up* m_fCurOffset * m_nDirection;
            yield return null;
        }
        if (m_nDirection > 0)
            prevViewPosition = Vector3.up * m_LimitSize;
        else
            prevViewPosition = Vector3.zero;
        m_fLimit = m_LimitSize;
        m_viewTRANS.localPosition = prevViewPosition;
        m_bIsRunning = false;
    }

}