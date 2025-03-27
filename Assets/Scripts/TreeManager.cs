using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [Header("Trees")]
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private int treeCount = 10;

    private TreeEntity[] _treeEntity;
    
    private void Start()
    {
        _treeEntity = new TreeEntity[treeCount];
        for (int i = 0; i < treeCount; i++)
        {
            _treeEntity[i] = Instantiate(treePrefab, transform).GetComponent<TreeEntity>();
        }
    }

    public void PlantTree(Vector3 position)
    {
        // Todo: Implement tree planting
    }
}
