using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    //把PlacementManager当成一个附着很多功能的格子地图
    public int width, height;
    Grid placementGrid; //可放置的区域
    
    //单独一个数据结构存放道路信息
    private Dictionary<Vector3Int, StructureModel> temporaryRoadObjects = new Dictionary<Vector3Int, StructureModel>(); // <position, structureModel>

    private void Start()
    {
        placementGrid = new Grid(width, height); //初始化可放置区域
    }

    #region  Get Neighbours' information
    
    //得到当前位置的邻近的所有cell
    internal CellType[] GetNeighbourTypesFor(Vector3Int position)
    {
        return placementGrid.GetAllAdjacentCellTypes(position.x, position.z);
    }

    //得到当前位置的邻近的所有符合特定类型的type
    internal List<Vector3Int> GetNeighboursOfTypeFor(Vector3Int position, CellType type)
    {
        var neighbourVertices = placementGrid.GetAdjacentCellsOfType(position.x, position.z, type);
        List<Vector3Int> neighbours = new List<Vector3Int>();
        foreach (var point in neighbourVertices)
        {
            //point的定义是二维的，这里把二维坐标转换为三维的
            neighbours.Add(new Vector3Int(point.X, 0, point.Y));
        }
        return neighbours;
    }
    #endregion

    #region Position Check Methods
    //区域检查，包括范围检查、单元格可用性检查、
    //定义为internal方法，表示整个项目可用
    internal bool CheckIfPositionInBound(Vector3Int position)
    {
        if (position.x >= 0 && position.x < width && position.z >= 0 && position.z < height)
        {
            return true;
        }
        return false;;
    }
    
    internal bool CheckIfPositionIsFree(Vector3Int position)
    {
        return CheckIfPositionIsOfType(position, CellType.Empty);
    }

    internal bool CheckIfPositionIsOfType(Vector3Int position, CellType type)
    {
        return placementGrid[position.x, position.z] == type;
    }
    
    #endregion

    #region Place Method

    internal void PlaceTemporaryStructure(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        placementGrid[position.x, position.z] = type; //指定cell的类型
        StructureModel structure = CreateANewStructureModel(position, structurePrefab, type); //放置建筑
        temporaryRoadObjects.Add(position, structure); //储存临时道路信息，因为道路可能被修改或销毁
    }
    
    //在指定位置创建特定种类的structure
    private StructureModel CreateANewStructureModel(Vector3Int position, GameObject structurePrefab, CellType type)
    {
        GameObject structure = new GameObject(type.ToString());
        //这里将model设置为placementManager的子GO，position就变成了placementManager的相对坐标
        //也就是placementGrid的相对坐标，我们就可以自由在这个grid上放置物体了
        structure.transform.SetParent(transform);
        structure.transform.localPosition = position;
        //-----------
        
        //添加对应类型的组件
        var structureModel = structure.AddComponent<StructureModel>();
        //在场景中创建这个建筑体
        structureModel.CreateModel(structurePrefab);
        return structureModel;
    }

    //替换模型
    public void ModifyStructureModel(Vector3Int position, GameObject newModel, Quaternion rotation)
    {
        //如果指定位置已经有模型了，那么替换模型
        if (temporaryRoadObjects.ContainsKey(position))
        {
            temporaryRoadObjects[position].SwapModel(newModel, rotation);
        }
    }

    #endregion
    

}
