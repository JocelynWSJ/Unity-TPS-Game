using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;
    public Image DamageImg;
    public Image WinLevelImg;
    public Image LoseLevelImg;
    public bool GameOn = true;
    public Font myFont;

    private void OnGUI()
    {
        GUI.skin.font = myFont;
    }
}
