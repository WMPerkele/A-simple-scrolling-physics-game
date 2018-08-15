using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    private enum ESpawnType
    {
        Pillar,
        Enemy,
        PillarEnemy,
        EnemyEnemy,
        PillarPillar,
        EnumSize
    }
    public float ScrollSpeed;
    public float MultiplierTime;
    public Transform SpawnLocation;
    public float SpawnRate;

    public EnemyObjectManager EnemyObjectManager;
    public LevelObjectManager ObstacleObjectManager;
    public LevelObjectManager PickUpObjectManager;

    private Coroutine m_spawnerRoutine;
    private float m_multiplierCounter;
    private bool m_keepUpdating;

    private static LevelManager m_instance;
    public static LevelManager Instance
    { get{ return m_instance; } }

    void Awake()
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

	
	//Update all the level objects
	void Update ()
    {
        if (m_keepUpdating == false)
            return;
        for(int i = 0; i < EnemyObjectManager.ObjectList.Count; i++)
        { 
            Enemy _object = EnemyObjectManager.ObjectList[i].GetComponent<Enemy>();
            _object.Move(new Vector3(-ScrollSpeed * Time.deltaTime, 0.0f, 0.0f));
            if (Camera.main.WorldToViewportPoint(_object.transform.position).x < 0.0)
            {
                EnemyObjectManager.ReturnObject(EnemyObjectManager.ObjectList[i]);
            }
        }
        for (int i = 0; i < ObstacleObjectManager.ObjectList.Count; i++)
        {
            GameObject _object = ObstacleObjectManager.ObjectList[i];
            _object.transform.position -= new Vector3(ScrollSpeed * Time.deltaTime, 0.0f, 0.0f);
            if (Camera.main.WorldToViewportPoint(_object.transform.position).x < 0.0)
            {
                ObstacleObjectManager.ReturnObject(_object);
            }
        }
        for (int i = 0; i < PickUpObjectManager.ObjectList.Count; i++)
        {
            GameObject _object = PickUpObjectManager.ObjectList[i];
            _object.transform.position -= new Vector3(ScrollSpeed * Time.deltaTime, 0.0f, 0.0f);
            if (Camera.main.WorldToViewportPoint(_object.transform.position).x < 0.0)
            {
                PickUpObjectManager.ReturnObject(_object);
            }
        }

        m_multiplierCounter += Time.deltaTime;
    }

    //Public function to control levelmanager
    public static void ToggleSpawning(bool isOn)
    {
        if (isOn)
            m_instance.StartSpawn();
        else
            m_instance.StopSpawn();
    }
    //The spawner routine increases variety and intensity as time goes on
    private IEnumerator spawnRoutine()
    {
        float _timer = 0.0f;
        int _levelStage = 0;
        ESpawnType _spawnType = ESpawnType.Enemy;

        while(true)
        {
            _levelStage = Mathf.FloorToInt(m_multiplierCounter / MultiplierTime);
            _timer += Time.deltaTime;

            _spawnType = (ESpawnType)Random.Range(0, Mathf.Clamp(_levelStage, 0, (int)ESpawnType.EnumSize));
            if (_timer > SpawnRate)
            {
                switch(_spawnType)
                {
                    case ESpawnType.Pillar:
                        ObstacleObjectManager.SpawnObject(0, _levelStage);
                        break;
                    case ESpawnType.Enemy:
                        EnemyObjectManager.SpawnObject(0, _levelStage);
                        break;
                    case ESpawnType.PillarEnemy:
                        bool _side = Random.Range(0, 2) != 0;
                        EnemyObjectManager.SpawnObject(_side ? -1 : 1, _levelStage);
                        ObstacleObjectManager.SpawnObject(_side ? 1 : -1, _levelStage);
                        break;
                    case ESpawnType.EnemyEnemy:
                        EnemyObjectManager.SpawnObject(1, _levelStage);
                        EnemyObjectManager.SpawnObject(-1, _levelStage);
                        break;
                    case ESpawnType.PillarPillar:
                        int _tempStepsMid = ObstacleObjectManager.StepsFromMiddle;
                        int _tempStepsEdge = ObstacleObjectManager.StepsFromEdge;
                        ObstacleObjectManager.StepsFromEdge = -6;
                        ObstacleObjectManager.StepsFromMiddle = 2;
                        ObstacleObjectManager.SpawnObject(1, _levelStage);
                        ObstacleObjectManager.SpawnObject(-1, _levelStage);
                        ObstacleObjectManager.StepsFromEdge = _tempStepsEdge;
                        ObstacleObjectManager.StepsFromMiddle = _tempStepsMid;
                        break;
                }
                _timer = 0.0f;
            }

            yield return null;
        }
    }



    //Start spawning and updating of level objects
    private void StartSpawn()
    {
        m_spawnerRoutine = StartCoroutine(spawnRoutine());
        m_multiplierCounter = 0.0f;
        m_keepUpdating = true;
    }

    //Seize spawning and updating of level objects
    private void StopSpawn()
    {
        if (m_spawnerRoutine != null)
            StopCoroutine(m_spawnerRoutine);
        m_spawnerRoutine = null;
        m_keepUpdating = false;
    }
}