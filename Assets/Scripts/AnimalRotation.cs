using UnityEngine;

public class AnimalRotation : MonoBehaviour
{
    private TerrainData terrainData;
    void Start()
    {
        // Get the terrain data
        Terrain terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;

        // Find the position on the terrain where your object is located
    }

    private void Update()
    {
        //Vector3 position = transform.position;
        //Vector2 normalizedPos = new Vector2( Mathf.InverseLerp(terrainData.bounds.min.x, terrainData.bounds.max.x, position.x), Mathf.InverseLerp(terrainData.bounds.min.z, terrainData.bounds.max.z, position.z));

        //// Get the normal vector of the terrain at that position
        //Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);

        //// Rotate your object to match the terrain normal
        //transform.up = terrainNormal;


        RaycastHit hit;
        var rei = new Ray(transform.position, Vector3.down);


        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (!Physics.Raycast(rei, out hit, 1000f, layerMask))
        {
            Debug.Log(System.DateTime.Now.ToString() + "" + "Not hitting anything!");
            return;
        }

        if (hit.collider == null)
            return;

        // Assuming you have already done a raycast and have hitInfo with the hitNormal
        Vector3 hitNormal = hit.normal;

        // Get the rotation that aligns the forward axis of your object with the hit normal
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

        // Apply the rotation to your object's transform
        transform.rotation = rotation;


        //Vector3 objectPosition = transform.position;
        ////float terrainHeight = terrainData.GetHeight((int)(objectPosition.x / terrainData.size.x), (int)(objectPosition.z / terrainData.size.z)) * terrainData.size.y;
        //Vector3 terrainNormal = terrainData.GetInterpolatedNormal(objectPosition.x / terrainData.size.x, objectPosition.z / terrainData.size.z);

        //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
        //transform.rotation = rotation;

    }
}
