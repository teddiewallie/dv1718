using UnityEngine;

public class FloorController : MonoBehaviour
{
    private GameObject floor;

    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    private void Start()
    {
        floor = GameObject.Find("Floor");
        BoxCollider col = floor.GetComponent<BoxCollider>();

        Vector3 size = col.bounds.size;
        Vector3 center = col.bounds.center;
        Vector3 half = size / 2f;

        minX = center.x - half.x;
        maxX = center.x + half.x;
        minZ = center.z - half.z;
        maxZ = center.z + half.z; 
    }

    public Vector3 Clamp(Vector3 pos)
    {
        Vector3 newPos = pos;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        return newPos;
    }

    public Vector3 RandomPos()
    {
        return new Vector3(
            Random.Range(minX, maxX),
            2f,
            Random.Range(minZ, maxZ)
        );
    }
}
