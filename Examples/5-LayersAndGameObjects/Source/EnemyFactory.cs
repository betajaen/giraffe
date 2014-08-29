using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyWave
{
  [SerializeField]
  public GameObject[] prefabs;

  [SerializeField]
  public int minCount = 3;

  [SerializeField]
  public int maxCount = 9;

  [HideInInspector]
  [NonSerialized]
  public List<Enemy> enemies = new List<Enemy>(4);
}

public class EnemyFactory : MonoBehaviour
{

  [NonSerialized]
  private Dictionary<GameObject, List<Enemy>> mEnemies;

  [SerializeField]
  public EnemyWave[] waves;

  [NonSerialized]
  private EnemyWave currentWave;

  [NonSerialized]
  private GiraffeLayer mLayer;

  [NonSerialized]
  private MissileFactory mMissileFactory;

  void Awake()
  {
    mEnemies = new Dictionary<GameObject, List<Enemy>>(4);
    mLayer = GetComponent<GiraffeLayer>();
    mMissileFactory = GetComponent<MissileFactory>();
  }

  public Enemy Add(GameObject prefab)
  {
    List<Enemy> enemies = null;
    if (mEnemies.TryGetValue(prefab, out enemies) == false)
    {
      enemies = new List<Enemy>(16);
      mEnemies.Add(prefab, enemies);
    }

    Enemy enemy = null;
    foreach (var m in enemies)
    {
      if (m.gameObject.activeSelf == false)
      {
        enemy = m;
        break;
      }
    }
    if (enemy == null)
    {
      GameObject go = Instantiate(prefab) as GameObject;
      go.transform.parent = transform;
      enemy = go.GetComponent<Enemy>();
      enemy.factory = this;
      enemy.missileFactory = mMissileFactory;
      enemies.Add(enemy);
    }
    else
    {
      enemy.gameObject.SetActive(true);
    }

    return enemy;
  }

  void Start()
  {
    SpawnWave();
  }

  void FixedUpdate()
  {
    if (currentWave == null || currentWave.enemies.Count == 0)
    {
      SpawnWave();
    }
  }

  void SpawnWave()
  {
    int nextWaveId = Time.frameCount % waves.Length;
    currentWave = waves[nextWaveId];
    int nbEnemies = UnityEngine.Random.Range(currentWave.minCount, currentWave.maxCount);
    int sw = Screen.width / mLayer.scale;
    int sh = Screen.height / mLayer.scale;
    int c = sh / nbEnemies;
    for (int i = 0; i < nbEnemies; i++)
    {
      int index = UnityEngine.Random.Range(0, currentWave.prefabs.Length);
      var enemy = Add(currentWave.prefabs[index]);
      enemy.Spawn(sw, c * i, currentWave);
      currentWave.enemies.Add(enemy);
    }
  }

}
