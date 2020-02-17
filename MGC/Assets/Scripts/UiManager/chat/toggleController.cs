using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class toggleController : MonoBehaviour
{
    public List<Image>          m_toggleImageList;
    public List<Text>           m_toggleTextList;

    public void onChange(int nIndex)
    {
        for(int i = 0; i < m_toggleImageList.Count; ++i)
        {
            if(nIndex == i)
            {
                m_toggleImageList[i].color = Color.black;
                m_toggleTextList[i].color = Color.white;
            }
            else
            {
                m_toggleImageList[i].color = Color.gray;
                m_toggleTextList[i].color = Color.black;
            }
        }
    }



}
