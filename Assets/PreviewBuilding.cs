using Photon.Pun;
using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    private CollisionDetection collisionDetection;
    private IsInBuildingArea isInBuildingArea;
    public SpawnBattleUnitConfigInfo buildingInfo;
    private Material previewMat;

    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
        isInBuildingArea = GetComponent<IsInBuildingArea>();
        previewMat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        bool canPlace = collisionDetection.CanPlace();
        bool isInBuildArea = isInBuildingArea.CanPlace();
        previewMat.SetColor("_Color", canPlace && isInBuildArea?new Color(0,1,0,0.5f): new Color(1,0,0,0.5f));
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