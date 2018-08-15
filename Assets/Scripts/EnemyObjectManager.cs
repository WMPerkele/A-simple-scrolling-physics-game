using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectManager : LevelObjectManager
{
    public int SetType = 0;
    //Spawn the enemy object and initialize it
    public override GameObject SpawnObject(int side, int levelStage) 
    {
        GameObject _object = base.SpawnObject(side, levelStage);
        Enemy _enemy = _object.GetComponent<Enemy>();
        _enemy.Initialize(SetType, levelStage >= 4 ? 1.5f : 1.0f);
        _enemy.ObjectManager = this;
        return _object;
    }

}
