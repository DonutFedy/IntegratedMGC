using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textController : MonoBehaviour
{
    public Text         m_text;
    public void setText(string chat, Color color)
    {
        m_text.text = chat;
        m_text.color = color;
    }
}
