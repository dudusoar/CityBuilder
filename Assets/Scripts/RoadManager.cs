using System.Collections.Generic;
using UnityEngine;


public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;
    
    public List<Vector3Int> temporaryPlacementPosition = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementmode = false;
    
    public RoadFixer roadFixer;

    private void Start()
    {
        roadFixer = GetComponent<RoadFixer>();
    }

    public void PlaceRoad(Vector3Int position)
    {
        if (placementManager.CheckIfPositionInBound(position) == false)
        {
            return;
        }

        if (placementManager.CheckIfPositionIsFree(position) == false)
        {
            return;
        }
        temporaryPlacementPosition.Clear();
        temporaryPlacementPosition.Add(position);
        roadPositionsToRecheck.Clear();
        // 先放直线路段
        placementManager.PlaceTemporaryStructure(position, roadFixer.deadEnd, CellType.Road);
        // 再根据信息修改道路类型
        FixRoadPrefabs();
    }

    private void FixRoadPrefabs()
    {
        foreach (var temporaryPosition in temporaryPlacementPosition)
        {
            roadFixer.FixRoadAtPosition(placementManager, temporaryPosition);
            var neighbours = placementManager.GetNeighboursOfTypeFor(temporaryPosition, CellType.Road);
            foreach (var roadPosition in neighbours)
            {
                roadPositionsToRecheck.Add(roadPosition);
            }
            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, positionToFix);
            }
        }
    }
}
