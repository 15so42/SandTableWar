using Photon.Pun;
using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    private CollisionDetection collisionDetection;
    public SpawnBattleUnitConfigInfo buildingInfo;
    private Material previewMat;

    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
        previewMat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        bool canPlace = collisionDetection.CanPlace();
        previewMat.SetColor("_Color", canPlace?new Color(0,1,0,0.5f): new Color(1,0,0,0.5f));
        //previewMat.SetColor("_EmissionColor",canPlace?Color.green:Color.red);
    }

    public void OnBuildingPreviewEnd(Vector3 pos)
    {
        if (collisionDetection.CanPlace())
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(buildingInfo,pos,GameManager.Instance.GetFightingManager().campId);
        }
        
        Destroy(gameObject);
        
    }
}