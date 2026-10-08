using UnityEngine;
using UnityEditor;

public class AutoColliderGenerator : EditorWindow
{
    private bool includeInactive = true;

    [MenuItem("Tools/Auto Collider Generator")]
    public static void ShowWindow()
    {
        GetWindow<AutoColliderGenerator>("Auto Collider");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Collider Generator", EditorStyles.boldLabel);

        includeInactive = EditorGUILayout.Toggle(
            "Include Inactive",
            includeInactive
        );

        if (Selection.activeGameObject == null)
        {
            EditorGUILayout.HelpBox(
                "Hierarchy se parent object select karo.",
                MessageType.Info
            );
            return;
        }

        GameObject selected = Selection.activeGameObject;

        GUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Selected:",
            selected.name
        );

        GUILayout.Space(10);

        // Normal option
        if (GUILayout.Button(
            "Add Mesh Colliders",
            GUILayout.Height(35)))
        {
            AddMeshColliders(selected);
        }

        GUILayout.Space(10);

        // Vehicle / object option
        if (GUILayout.Button(
            "Add ONE BoxCollider Per Child",
            GUILayout.Height(45)))
        {
            AddBoxColliderPerChild(selected);
        }

        GUILayout.Space(5);

        // Remove all colliders under selected parent
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);

        if (GUILayout.Button(
            "Remove All Child Colliders",
            GUILayout.Height(35)))
        {
            RemoveChildColliders(selected);
        }

        GUI.backgroundColor = Color.white;
    }

    // ---------------------------------------------------------
    // ADD MESH COLLIDERS
    // ---------------------------------------------------------

    private void AddMeshColliders(GameObject root)
    {
        MeshFilter[] meshes =
            root.GetComponentsInChildren<MeshFilter>(includeInactive);

        int added = 0;

        foreach (MeshFilter meshFilter in meshes)
        {
            GameObject obj = meshFilter.gameObject;

            if (obj.GetComponent<Collider>() != null)
                continue;

            if (meshFilter.sharedMesh == null)
                continue;

            MeshCollider collider =
                Undo.AddComponent<MeshCollider>(obj);

            collider.sharedMesh = meshFilter.sharedMesh;
            collider.convex = false;

            added++;
        }

        Debug.Log(
            $"Added {added} MeshColliders under '{root.name}'."
        );
    }

    // ---------------------------------------------------------
    // ONE BOX COLLIDER PER DIRECT CHILD
    // ---------------------------------------------------------

    private void AddBoxColliderPerChild(GameObject root)
    {
        int added = 0;

        foreach (Transform child in root.transform)
        {
            Renderer[] renderers =
                child.GetComponentsInChildren<Renderer>(
                    includeInactive
                );

            if (renderers.Length == 0)
                continue;

            // If child already has a collider, don't duplicate
            BoxCollider box = child.GetComponent<BoxCollider>();

            if (box == null)
            {
                box = Undo.AddComponent<BoxCollider>(child.gameObject);
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // Convert world bounds to child's local space
            Vector3 localCenter =
                child.InverseTransformPoint(bounds.center);

            Vector3[] corners = GetBoundsCorners(bounds);

            Vector3 localMin =
                child.InverseTransformPoint(corners[0]);

            Vector3 localMax = localMin;

            foreach (Vector3 corner in corners)
            {
                Vector3 localPoint =
                    child.InverseTransformPoint(corner);

                localMin = Vector3.Min(localMin, localPoint);
                localMax = Vector3.Max(localMax, localPoint);
            }

            box.center = (localMin + localMax) * 0.5f;
            box.size = localMax - localMin;

            added++;
        }

        Debug.Log(
            $"Added/updated {added} BoxColliders under '{root.name}'."
        );

        EditorUtility.DisplayDialog(
            "Done!",
            $"Configured {added} BoxColliders.\n\n" +
            "Each direct child now has ONE BoxCollider.",
            "OK"
        );
    }

    // ---------------------------------------------------------
    // REMOVE ALL CHILD COLLIDERS
    // ---------------------------------------------------------

    private void RemoveChildColliders(GameObject root)
    {
        Collider[] colliders =
            root.GetComponentsInChildren<Collider>(
                includeInactive
            );

        int removed = 0;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == root)
                continue;

            Undo.DestroyObjectImmediate(collider);
            removed++;
        }

        Debug.Log(
            $"Removed {removed} child colliders under '{root.name}'."
        );

        EditorUtility.DisplayDialog(
            "Done!",
            $"Removed {removed} child colliders.",
            "OK"
        );
    }

    // ---------------------------------------------------------
    // BOUNDS CORNERS
    // ---------------------------------------------------------

    private Vector3[] GetBoundsCorners(Bounds b)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

        return new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, max.y, max.z),

            new Vector3(max.x, min.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, max.y, max.z)
        };
    }
}