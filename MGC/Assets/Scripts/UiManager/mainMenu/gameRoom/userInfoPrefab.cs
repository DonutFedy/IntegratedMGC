using PACKET;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class userInfoPrefab : MonoBehaviour
{
    S_RoomUserInfo              m_userInfo;
    [SerializeField]
    Text                        m_userNickNameText;
    [SerializeField]            
    GameObject                  m_ChatBox;
    [SerializeField]
    Text                        m_ChatBoxText;
    [SerializeField]
    GameObject                  m_ReadyImageObj;
    [SerializeField]
    Image                       m_ReadyImage;
    
    [SerializeField]
    Image                       m_CharacterImage;

    Coroutine                   m_chatCoroutine;
    float                       m_fChatRemainTime;
    const float                 m_fChatTime = 5;

    public int                  m_splitChatLength;

    public delegate void dClickEvent(string nickname);
    dClickEvent m_dWhisperClickEvent;

    public void exitUser()
    {
        m_dWhisperClickEvent = null;
        m_ChatBox.SetActive(false);
        m_ChatBoxText.text = "";
        m_userNickNameText.text = "";

        m_chatCoroutine = null;
        // 이미지 인덱스에 따라서 세팅
        m_CharacterImage.sprite = GameManager.m_Instance.getCharacterSprite(-1);
        m_ReadyImage.sprite = GameManager.m_Instance.getUIsprite(resourceManager.UIspriteINDEX.GAMEROOM_READY_SP);
        m_ReadyImageObj.SetActive(false);
    }
    public void setUserInfo(S_RoomUserInfo info, dClickEvent funcWhisper)
    {
        m_ChatBox.SetActive(false);
        m_dWhisperClickEvent = funcWhisper;

        m_userInfo.m_userNickname = info.m_userNickname;
        m_userNickNameText.text = m_userInfo.m_userNickname;

        m_userInfo.m_nCharacterImageIndex = info.m_nCharacterImageIndex;
        m_userInfo.m_bIsMaster = info.m_bIsMaster;
        m_userInfo.m_bReadyState = info.m_bReadyState;

        if (m_userInfo.m_bIsMaster)
        {
            m_ReadyImage.sprite = GameManager.m_Instance.getUIsprite(resourceManager.UIspriteINDEX.GAMEROOM_MASTER_SP);
            m_ReadyImageObj.SetActive(true);
        }
        else
        {
            m_ReadyImage.sprite = GameManager.m_Instance.getUIsprite(resourceManager.UIspriteINDEX.GAMEROOM_READY_SP);
            m_ReadyImageObj.SetActive(m_userInfo.m_bReadyState);
        }

        // 이미지 인덱스에 따라서 세팅
        m_CharacterImage.sprite = GameManager.m_Instance.getCharacterSprite(m_userInfo.m_nCharacterImageIndex);
    }

    public void release()
    {
        if (m_chatCoroutine != null)
            StopCoroutine(m_chatCoroutine);
    }

    public string getNickName()
    {
        return m_userInfo.m_userNickname;
    }
    public void chating(string chat)
    {
        if (m_chatCoroutine != null)
            StopCoroutine(m_chatCoroutine);
        m_chatCoroutine = StartCoroutine(chatingCorourine(chat));
    }

    public bool getReadyState()
    {
        return m_userInfo.m_bReadyState;
    }
    public bool isMaster()
    {
        return m_userInfo.m_bIsMaster;
    }
    public void changeReadyState()
    {
        if (!m_userInfo.m_bIsMaster)
        {
            m_userInfo.m_bReadyState = !(m_userInfo.m_bReadyState);
            m_ReadyImageObj.SetActive(m_userInfo.m_bReadyState);
        }
    }

    /// <summary>
    /// 유저info Slot을 클릭시 귓속말 설정
    /// </summary>
    public void clickUserInfo()
    {
        if (m_dWhisperClickEvent != null)
            m_dWhisperClickEvent(m_userInfo.m_userNickname);
    }

    IEnumerator chatingCorourine(string chat)
    {
        m_fChatRemainTime = m_fChatTime;
        m_ChatBox.SetActive(true);
        chat = addEnterStr(chat);
        m_ChatBoxText.text = chat;


        while (m_fChatRemainTime > 0)
        {
            m_fChatRemainTime -= Time.deltaTime;
            yield return 0;
        }
        m_ChatBox.SetActive(false);
    }

    string addEnterStr(string chat)
    {
        StringBuilder builder = new StringBuilder();


        while(chat.Length > m_splitChatLength)
        {
            builder.Append(chat.Substring(0, m_splitChatLength)).Append("\n");
            chat = chat.Substring(m_splitChatLength, chat.Length - m_splitChatLength);
        }
        builder.Append(chat.Substring(0, chat.Length));

        return builder.ToString();
    }
}
