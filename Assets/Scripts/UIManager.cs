using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject PreGameUI;
    public GameObject GameUI;
    public GameObject PostGameUI;

    public Text HealthUI;
    public Text WeaponUI;
    public Text ScoreUI;


    private GameObject m_currentUI;

    private static UIManager m_instance;
    public static UIManager Instance
    { get { return m_instance; } }

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        //Update all the UI elements
        if(m_currentUI == GameUI)
        {
            HealthUI.text = "Health " + GameManager.Player.GetHealth() + "/" + GameManager.Player.MaxHealth;
            WeaponUI.text = "Current Weapon is " + GameManager.Player.GetWeapon().Name;
            ScoreUI.text = "Score: " + GameManager.GetScore();
        }

    }

    public void ToggleUI(int stage)
    {
        if(m_currentUI != null)
            m_currentUI.SetActive(false);
        switch(stage)
        {
            case 0: //StartUI
                m_currentUI = PreGameUI;
                break;
            case 1: //GameUI
                m_currentUI = GameUI;
                break;
            case 2: //EndGameUI
                m_currentUI = PostGameUI;
                break;
        }
        m_currentUI.SetActive(true);
    }


}
