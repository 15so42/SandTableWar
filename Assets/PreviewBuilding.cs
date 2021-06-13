using Photon.Pun;
using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    private CollisionDetection collisionDetection;
    private IsInBuildingArea isInBuildingArea;
    public SpawnBattleUnitConfigInfo buildingInfo;
    private Material previewMat;
    private Collider collider;


    private bool playerControl;
    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
        isInBuildingArea = GetComponent<IsInBuildingArea>();
        previewMat = GetComponent<MeshRenderer>().material;
    }

    public void ToggleCollider(bool status)
    {
        collisionDetection.ToggleCollider(status);
    }

    public void Init(bool playerControl)
    {
        this.playerControl = playerControl;
    }

    private void Update()
    {
        
        bool canPlace = collisionDetection.CanPlace();
        
        bool isInBuildArea = isInBuildingArea.CanPlace(playerControl);
        previewMat.SetColor("_Color", canPlace && isInBuildArea?new Color(0,1,0,0.5f): new Color(1,0,0,0.5f));
        //previewMat.SetColor("_EmissionColor",canPlace?Color.green:Color.red);
    }

    public void OnBuildingPreviewEnd(Vector3 pos,int factionId)
    {
        
        if (collisionDetection.CanPlace()&& isInBuildingArea.CanPlace(playerControl))
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(buildingInfo,pos,factionId);
        }
        
        Destroy(gameObject);
        
    }
}