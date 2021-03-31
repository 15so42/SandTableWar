using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;

public class EnemySpawner : MonoBehaviour
{
    [Header("要生成的敌人种类")]
    public BattleUnitId[] battleUnitIds;

    private SpawnBattleUnitConfigInfo[] spawnBattleUnitConfigInfos;
    public bool byPhoton;

    public int maxSpawnCount;
    public float spawnRadius=3;

    public float spawnInterval=1;

    [Header("默认为1，-1表示无限次循环")]
    public int loopCount = 1;

    public float delay=0;

    private int spawnCount;
    // Start is called before the first frame update
    void Start()
    {
        spawnCount = maxSpawnCount;
        spawnBattleUnitConfigInfos=new SpawnBattleUnitConfigInfo[battleUnitIds.Length];
        for (int i = 0; i < battleUnitIds.Length; i++)
        {
            spawnBattleUnitConfigInfos[i] =
                ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(battleUnitIds[i]);
        }

        Timer.Register(delay, () =>
        {
            StartCoroutine(Spawn());
        });

    }

    // Update is called once per frame
    IEnumerator Spawn()
    {
        while (loopCount >= 1 || loopCount == -1)
        {
            if (spawnCount > 0)
            {
                int selfCampId = GameManager.Instance.GetSelfId();
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * spawnRadius, out navMeshHit,spawnRadius,-1))
                {
                    BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(
                        spawnBattleUnitConfigInfos[Random.Range(0, battleUnitIds.Length)],
                        navMeshHit.position, selfCampId + 1);
                }
               
                spawnCount--;
                if (spawnCount <= 0)
                {
                    if (loopCount >= 1)
                    {
                        loopCount--;
                        spawnCount = maxSpawnCount;
                    }
                    else
                    {
                        if (loopCount == -1)
                        {
                            spawnCount = maxSpawnCount;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }

    }
}
