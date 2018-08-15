using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObjectManager : MonoBehaviour
{
    public string ObjectName;
    public GameObject Prefab;
    public List<GameObject> ObjectList;
    public int StepsFromMiddle;
    public int StepsFromEdge;

    private List<GameObject> m_objectPool;

    void Awake()
    {
        ObjectList = new List<GameObject>();
        m_objectPool = new List<GameObject>();
    }

    //Public function to create a level object
    public virtual GameObject SpawnObject(int side, int levelStage)
    {
        float _minFromMid = Screen.height / 2 - Screen.height / StepsFromMiddle;
        float _maxFromMid = Screen.height / 2 + Screen.height / StepsFromMiddle;
        float _yPos = 0.0f;
        if (side == 0)
            _yPos = Random.Range(0, 2) == 0 ? Random.Range(0 + Screen.height / StepsFromEdge, _minFromMid) : Random.Range(Screen.height - Screen.height / StepsFromEdge, _maxFromMid);
        else if (side < 0)
            _yPos = Random.Range(0 + Screen.height / StepsFromEdge, _minFromMid);
        else if (side > 0)
            _yPos = Random.Range(Screen.height - Screen.height / StepsFromEdge, _maxFromMid);
        Vector3 _spawnLocation = Vector3.Scale(new Vector3(1.0f, 1.0f, 0.0f), Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, _yPos, 0)));

        return AddObject(_spawnLocation);
    }

    public GameObject SpawnObject(Vector3 position)
    {
        return AddObject(position);
    }

    //Return the pool object back to the manager pool
    public void ReturnObject(GameObject poolObject)
    {
        if(!m_objectPool.Contains(poolObject))
            m_objectPool.Add(poolObject);

        ObjectList.Remove(poolObject);
        poolObject.gameObject.SetActive(false);
    }

    //Add the object to our active object list and set it's position as the parameter
    protected GameObject AddObject(Vector3 position)
    {
        GameObject _object = getObject();
        _object.transform.position = position;
        _object.gameObject.SetActive(true);
        ObjectList.Add(_object);
        return _object;
    }

    //Get an object from the pool, or if the pool is empty, create a new pool object
    protected GameObject getObject()
    {
        GameObject _object;
        int _availableObjectIndex = m_objectPool.Count - 1;
        if (_availableObjectIndex >= 0)
        {
            _object = m_objectPool[_availableObjectIndex];
            m_objectPool.RemoveAt(_availableObjectIndex);
        }
        else
        {
            _object = Instantiate<GameObject>(Prefab);
            _object.transform.SetParent(transform, false);
        }
        return _object;
    }




}
