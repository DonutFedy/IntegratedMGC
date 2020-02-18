using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textController : MonoBehaviour
{
    public Text         m_text;
    string              m_srcName;

    public delegate void dSetWhisper(string destName);
    dSetWhisper         m_setWhisperFUNC;


    public void setText(string srcName,string chat, Color color, dSetWhisper func)
    {
        m_srcName = srcName;
        m_text.text = chat;
        m_text.color = color;
        m_setWhisperFUNC = func;
    }

    public void onClickChatSlot()
    {
        if (m_srcName != "" && m_setWhisperFUNC != null)
            m_setWhisperFUNC(m_srcName);
    }
    
}
