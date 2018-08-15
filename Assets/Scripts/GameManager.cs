using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public enum EGameStage
{
    Start,
    Play,
    End
}
public class GameManager : MonoBehaviour
{
    public static Player Player
    { get { return m_instance.m_player; } }

    private static GameManager m_instance;
    public static GameManager Instance
    { get { return m_instance; } }

    public GameObject PlayerPrefab;

    private EGameStage m_gameStage;
    private Player m_player;
    private float m_gameTime;
    private int m_enemiesKilled;

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        m_gameStage = EGameStage.Start;
        UIManager.Instance.ToggleUI((int)Instance.m_gameStage);
    }

    void Update()
    {
            switch (m_gameStage)
        {
            case EGameStage.Start:
                if (Input.GetButtonDown("Submit"))
                    startGame();
                break;
            case EGameStage.Play:
                m_gameTime += Time.deltaTime;
                break;
            case EGameStage.End:
                if (Input.GetButtonDown("Submit"))
                    restartGame();
                break;
        }
    }

    //Start ending game
    public static void EndGame()
    {
        LevelManager.ToggleSpawning(false);
        Instance.m_player.gameObject.SetActive(false);
        Instance.m_gameStage = EGameStage.End;
        UIManager.Instance.ToggleUI((int)Instance.m_gameStage);
    }

    public static int GetScore()
    {
        return Mathf.FloorToInt(m_instance.m_gameTime) + (m_instance.m_enemiesKilled * 10);
    }

    public void AddKill()
    {
        m_enemiesKilled++;
    }


    //Start a game and initialize player and UI
    private void startGame()
    {
        Vector3 _spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4, 0, 0));
        _spawnPoint.y = 0;
        _spawnPoint.z = 1;
        GameObject _player = Instantiate(PlayerPrefab, _spawnPoint, Quaternion.identity);
        m_player = _player.GetComponent<Player>();
        m_player.name = "Player";
        m_player.SpawnSpot = _spawnPoint;
        LevelManager.ToggleSpawning(true);
        m_gameStage = EGameStage.Play;
        UIManager.Instance.ToggleUI((int)m_gameStage);
    }
    
    private void restartGame()
    {
        m_gameStage = EGameStage.Start;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    
}
