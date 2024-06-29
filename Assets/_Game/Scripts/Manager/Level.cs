using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Level : MonoBehaviour
{
    [SerializeField] NavMeshData NavMeshData;
    public int totalNum;
    public Transform TF;

    private int index;
    public int Index { get { return index; } set { index = value; } }

    private void Awake()
    {
        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(NavMeshData);
    }

    public void OnDespawn()
    {
        Destroy(gameObject);
    }
}
