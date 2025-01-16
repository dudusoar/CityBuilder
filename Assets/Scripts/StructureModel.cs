using UnityEngine;

public class StructureModel : MonoBehaviour
{
    float yHeight = 0;

    public void CreateModel(GameObject model)
    {
        //transform指这个组件（脚本）挂载对象的transform
        var structure = Instantiate(model, transform);
        //记录创建模型的高度，确保以后切换其他模型时可以复用这个高度值，防止位置发生漂移
        yHeight = structure.transform.position.y;
        
    }
    
    public void SwapModel(GameObject model, Quaternion rotation)
    {
        //清空旧内容再添加更新内容
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var structure = Instantiate(model, transform);
        // 放置模型发生漂移
        structure.transform.localPosition = new Vector3(0, yHeight, 0);
        structure.transform.localRotation = rotation;
    }
}
