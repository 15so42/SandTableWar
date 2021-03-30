using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineMachine : BaseBattleBuilding
{
    public Transform[] mineLeave;
    public LineRenderer[] lineRenderers;
    public Transform[] shootPoints;
    private Vector3[] lastMinePoint;

    private bool setUpFinished;
    public void StartMine()
    {
        setUpFinished = true;
        PlayBuildCompleteFx();
        lastMinePoint = new[]
        {
            transform.position + Random.insideUnitSphere, transform.position + Random.insideUnitSphere,
            transform.position + Random.insideUnitSphere, transform.position + Random.insideUnitSphere
        };
        GameManager.Instance.GetFightingManager().battleResMgr.AddIncreaseRate(BattleResType.mineral,1);
    }

    //private float timer;

    //public float interval;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (setUpFinished)
        {
            transform.Rotate(Vector3.up * (30 * Time.deltaTime));
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                float rand = Random.Range(0f, 1f);
                if (rand < 0.05f)
                {
                    lastMinePoint[i] = transform.position + Random.insideUnitSphere;

                }
                lineRenderers[i].SetPositions(new[]{shootPoints[i].position,lastMinePoint[i]});
            }
        }
    }

    public override void Die()
    {
        Destroy(hpUi.gameObject);
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].enabled = false;
            lineRenderers[i].transform.parent.SetParent(null);
            lineRenderers[i].transform.gameObject.AddComponent<MeshCollider>().convex=true;
            lineRenderers[i].gameObject.AddComponent<Rigidbody>();
        }
        GameManager.Instance.GetFightingManager().battleResMgr.ReduceIncreaseRate(BattleResType.mineral,1);
        base.Die();
    }
}
