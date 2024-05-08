using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPositionManager : MonoBehaviour
{
    [SerializeField] float cornerOffset;
    [SerializeField] Vector2 size;
    [SerializeField] bool showGizmos;
    [SerializeField] Transform[] walls;

    public static HidingPositionManager Instance { get; private set; }
    public List<Vector3> HidingSpots { get; private set; } = new List<Vector3>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GetHidingSpots();
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        GetHidingSpots();

        // show position of hiding spots
        Gizmos.color = Color.green;
        foreach (Vector3 spot in HidingSpots)
        {
            Gizmos.DrawSphere(spot, 0.5f);
        }
    }

    void GetHidingSpots()
    {
        // reset hiding spots list
        HidingSpots.Clear();
        // instantiate array to store corners, and vectors for calculations
        Vector3[] corners = new Vector3[4];
        Vector3 tempSpot1, tempSpot2;

        foreach (Transform wall in walls)
        {
            // corner 1
            corners[0] = wall.position;
            corners[0].x += size.x - cornerOffset;
            corners[0].z +=  size.y - cornerOffset;
            // calculate hiding spots
            tempSpot1 = corners[0];
            tempSpot2 = corners[0];
            tempSpot1.x += cornerOffset * 2;
            tempSpot2.z += cornerOffset * 2;
            HidingSpots.Add(tempSpot1);
            HidingSpots.Add(tempSpot2);

            // corner 2
            corners[1] = wall.position;
            corners[1].x += size.x - cornerOffset;
            corners[1].z -=  size.y - cornerOffset;
            // calculate hiding spots
            tempSpot1 = corners[1];
            tempSpot2 = corners[1];
            tempSpot1.x += cornerOffset * 2;
            tempSpot2.z -= cornerOffset * 2;
            HidingSpots.Add(tempSpot1);
            HidingSpots.Add(tempSpot2);

            // corner 3
            corners[2] = wall.position;
            corners[2].x -= size.x - cornerOffset;
            corners[2].z +=  size.y - cornerOffset;
            // calculate hiding spots
            tempSpot1 = corners[2];
            tempSpot2 = corners[2];
            tempSpot1.x -= cornerOffset * 2;
            tempSpot2.z += cornerOffset * 2;
            HidingSpots.Add(tempSpot1);
            HidingSpots.Add(tempSpot2);

            // corner 4
            corners[3] = wall.position;
            corners[3].x -= size.x - cornerOffset;
            corners[3].z -=  size.y - cornerOffset;
            // calculate hiding spots
            tempSpot1 = corners[3];
            tempSpot2 = corners[3];
            tempSpot1.x -= cornerOffset * 2;
            tempSpot2.z -= cornerOffset * 2;
            HidingSpots.Add(tempSpot1);
            HidingSpots.Add(tempSpot2);
        }
    }
}
