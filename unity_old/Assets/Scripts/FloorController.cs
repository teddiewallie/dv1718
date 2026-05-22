using UnityEngine;

public class FloorController : MonoBehaviour
{
    private GameObject floor;

    private float _minX;
    private float _maxX;
    private float _minZ;
    private float _maxZ;

    private void Start()
    {
        floor = GameObject.Find("Floor");
        BoxCollider col = floor.GetComponent<BoxCollider>();

        Vector3 size = col.bounds.size;
        Vector3 center = col.bounds.center;
        Vector3 half = size / 2f;

        this._minX = center.x - half.x;
        this._maxX = center.x + half.x;
        this._minZ = center.z - half.z;
        this._maxZ = center.z + half.z; 
    }

    public Vector3 Clamp(Vector3 pos, float height)
    {
        Vector3 newPos = pos;

        newPos.x = Mathf.Clamp(newPos.x, this._minX, this._maxX);
        newPos.y = height;
        newPos.z = Mathf.Clamp(newPos.z, this._minZ, this._maxZ);

        return newPos;
    }

    public Vector3 RandomPos()
    {
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: return new Vector3(this._minX, 1.5f, Random.Range(this._minZ, this._maxZ));
            case 1: return new Vector3(this._maxX, 1.5f, Random.Range(this._minZ, this._maxZ));
            case 2: return new Vector3(Random.Range(this._minX, this._maxX), 1.5f, this._minZ);
            case 3: return new Vector3(Random.Range(this._minX, this._maxX), 1.5f, this._maxZ);
            default: return new Vector3(Random.Range(this._minX, this._maxX), 1.5f, this._maxZ);
        }
    }
}

