using System.Collections.Generic;
using SVS;
using UnityEngine;


public class RoadManager : MonoBehaviour
{
    public PlacementManager placementManager;
    
    public List<Vector3Int> temporaryPlacementPosition = new List<Vector3Int>();
    public List<Vector3Int> roadPositionsToRecheck = new List<Vector3Int>();

    private Vector3Int startPosition;
    private bool placementMode = false;
    
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

        if (placementMode == false)
        {
            temporaryPlacementPosition.Clear();
            roadPositionsToRecheck.Clear();
            
            placementMode = true;
            startPosition = position;
            
            temporaryPlacementPosition.Add(position);
            placementManager.PlaceTemporaryStructure(position, roadFixer.deadEnd, CellType.Road);
            
        }
        else
        {
            placementManager.RemoveAllTemporaryStructures();
            temporaryPlacementPosition.Clear();
            
            foreach (var positionToFix in roadPositionsToRecheck)
            {
                roadFixer.FixRoadAtPosition(placementManager, positionToFix);
            }
            roadPositionsToRecheck.Clear();
            
            temporaryPlacementPosition = placementManager.GetPathBetween(startPosition, position);
            
            foreach(var temporaryPosition in temporaryPlacementPosition)
            {
                if (placementManager.CheckIfPositionIsFree(temporaryPosition) == false)
                {
                    return;
                }

                placementManager.PlaceTemporaryStructure(temporaryPosition, roadFixer.deadEnd, CellType.Road);
            }
        }
        
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
                if (roadPositionsToRecheck.Contains(roadPosition) == false)
                {
                    roadPositionsToRecheck.Add(roadPosition);
                }
            }
        }

        foreach (var positionToFix in roadPositionsToRecheck)
        {
            roadFixer.FixRoadAtPosition(placementManager, positionToFix);
        }
    }
    
    public void FinishPlacingRoad()
    {
        placementMode = false;
        placementManager.AddtemporaryStructuresToStructureDictionary();
        if (temporaryPlacementPosition.Count > 0)
        {
            AudioPlayer.instance.PlayPlacementSound();
        }
        temporaryPlacementPosition.Clear();
        startPosition = Vector3Int.zero;
    }
    
    
}
