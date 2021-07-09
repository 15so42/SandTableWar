using Photon.Pun;
using UnityEngine;
using UnityTimer;

public class PreviewBuilding : MonoBehaviour
{
    private CollisionDetection collisionDetection;
    private IsInBuildingArea isInBuildingArea;
    public SpawnBattleUnitConfigInfo buildingInfo;
    private MeshRenderer meshRenderer;
    private Material previewMat;
    private Collider collider;


    private bool playerControl;
    private void Awake()
    {
        collisionDetection = GetComponent<CollisionDetection>();
        isInBuildingArea = GetComponent<IsInBuildingArea>();
        meshRenderer = GetComponent<MeshRenderer>();
        previewMat = meshRenderer.material;
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

    public void SetVisibility(bool status)
    {
        meshRenderer.enabled = status;
    }

    public BaseBattleBuilding OnBuildingPreviewEnd(Vector3 pos,int factionId)
    {
        BaseBattleBuilding result = null;
        if (collisionDetection.CanPlace()&& isInBuildingArea.CanPlace(playerControl))
        {
            result= BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(buildingInfo,pos,pos,factionId) as BaseBattleBuilding;
        }
        
        gameObject.transform.rotation = result == null ? Quaternion.identity : result.transform.rotation;
        Destroy(gameObject,3);
        return result;
    }
}