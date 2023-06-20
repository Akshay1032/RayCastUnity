using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    public GameObject markerPrefab;
    List<GameObject> markers = new List<GameObject>();

    private List<Transform> _selections= new List<Transform>(); // List to hold the transforms
    private List<Transform> _newSelections = new List<Transform>(); // List to hold the transforms
    public int maxMarkers = 10;

    Camera mainCamera;
    LineRenderer lineRenderer;

    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.right, 1000f);

        _newSelections.Clear();


        // Get a list of new selections that were hit
        foreach (RaycastHit hit in hits)
        {
            _newSelections.Add(hit.transform);
        }

        List<Transform> removedSelections = _selections.Except(_newSelections).ToList();


        if (removedSelections != null)
        {
            foreach (Transform _selection in removedSelections)
            {
                // Access each transform
                var selectionRenderer = _selection.GetComponent<Renderer>();
                selectionRenderer.material = defaultMaterial;
                _selections.Remove(_selection);
                //removedSelections.Remove(_selection);
                
            }

        }


        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            var selection = hits[i].transform;

            if (markers.Count + hits.Length > maxMarkers)
            {
                int markersToRemove = markers.Count + hits.Length - maxMarkers;
                RemoveMarkers(markersToRemove);
            }

            if (selection.CompareTag(selectableTag))
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();

                if (rend)
                {
                    rend.material = highlightMaterial;
                }



                 GameObject markerInstance = Instantiate(markerPrefab, hit.point, Quaternion.identity);
                 markers.Add(markerInstance);

                 Vector3 oppositeEdge = hit.point + transform.right * hit.collider.bounds.size.x;
                 GameObject markerInstance1 = Instantiate(markerPrefab, oppositeEdge, Quaternion.identity);
                 markers.Add(markerInstance1);

                _selections.Add(selection);
            }
            
        }

    }
    void RemoveMarkers(int count)
    {
        // Destroy oldest markers if the count exceeds the maximum
        for (int i = 0; i < count; i++)
        {
            Destroy(markers[i]);
        }

        // Remove destroyed markers from the list
        markers.RemoveRange(0, count);
    }
}


