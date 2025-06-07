using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class DungeonGeneratorOld : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int dungeonWidth = 100;
    public int dungeonHeight = 100;
    public int minRoomSize = 10;
    public int maxRoomSize = 30;
    public int splitIterations = 5;
    public float wallHeight = 3f;
    public float wallThickness = 0.2f;
    [Tooltip("Hallway width in tiles (must be odd for symmetry)")]
    public int corridorWidth = 3;

    private List<Rect> rooms = new List<Rect>();
    private List<Vector2Int[]> corridors = new List<Vector2Int[]>();

    void Start() => GenerateDungeon();

    void GenerateDungeon()
    {
        rooms.Clear(); corridors.Clear();
        var leaves = new List<Rect> { new Rect(0, 0, dungeonWidth, dungeonHeight) };
        BSPSplit(leaves, splitIterations);
        foreach (var leaf in leaves)
        {
            int w = Random.Range(minRoomSize, (int)leaf.width - 2);
            int h = Random.Range(minRoomSize, (int)leaf.height - 2);
            int x = Random.Range((int)leaf.x + 1, (int)leaf.x + (int)leaf.width - w - 1);
            int y = Random.Range((int)leaf.y + 1, (int)leaf.y + (int)leaf.height - h - 1);
            rooms.Add(new Rect(x, y, w, h));
        }
        ConnectRooms();
        BuildDungeonMesh();
    }

    void BSPSplit(List<Rect> list, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            var leaf = list[Random.Range(0, list.Count)];
            if (leaf.width <= minRoomSize * 2 || leaf.height <= minRoomSize * 2) continue;
            if (leaf.width > leaf.height)
            {
                float splitX = Random.Range(leaf.x + minRoomSize, leaf.x + leaf.width - minRoomSize);
                list.Remove(leaf);
                list.Add(new Rect(leaf.x, leaf.y, splitX - leaf.x, leaf.height));
                list.Add(new Rect(splitX, leaf.y, leaf.x + leaf.width - splitX, leaf.height));
            }
            else
            {
                float splitY = Random.Range(leaf.y + minRoomSize, leaf.y + leaf.height - minRoomSize);
                list.Remove(leaf);
                list.Add(new Rect(leaf.x, leaf.y, leaf.width, splitY - leaf.y));
                list.Add(new Rect(leaf.x, splitY, leaf.width, leaf.y + leaf.height - splitY));
            }
        }
    }

    void ConnectRooms()
    {
        for (int i = 1; i < rooms.Count; i++)
        {
            var a = Vector2Int.RoundToInt(rooms[i - 1].center);
            var b = Vector2Int.RoundToInt(rooms[i].center);
            if (Random.value < 0.5f)
            {
                corridors.Add(new[] { a, new Vector2Int(b.x, a.y) });
                corridors.Add(new[] { new Vector2Int(b.x, a.y), b });
            }
            else
            {
                corridors.Add(new[] { a, new Vector2Int(a.x, b.y) });
                corridors.Add(new[] { new Vector2Int(a.x, b.y), b });
            }
        }
    }

    void BuildDungeonMesh()
    {
        var mf = GetComponent<MeshFilter>();
        var mc = GetComponent<MeshCollider>();
        var mesh = new Mesh { name = "Dungeon" };

        bool[,] occupied = new bool[dungeonWidth, dungeonHeight];
        foreach (var r in rooms)
            for (int x = (int)r.x; x < r.x + r.width; x++)
                for (int y = (int)r.y; y < r.y + r.height; y++)
                    occupied[x, y] = true;
        foreach (var c in corridors)
        {
            var rect = CreateCorridorRect(c[0], c[1]);
            for (int x = (int)Mathf.Max(0, rect.x); x < Mathf.Min(dungeonWidth, rect.x + rect.width); x++)
                for (int y = (int)Mathf.Max(0, rect.y); y < Mathf.Min(dungeonHeight, rect.y + rect.height); y++)
                    occupied[x, y] = true;
        }

        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        // Floors
        for (int x = 0; x < dungeonWidth; x++)
            for (int y = 0; y < dungeonHeight; y++)
                if (occupied[x, y])
                    AddQuad(new Vector3(x, 0, y), Vector3.right, Vector3.forward, ref verts, ref tris, ref uvs);

        // Walls
        for (int x = 0; x < dungeonWidth; x++)
        for (int y = 0; y < dungeonHeight; y++)
        {
            if (!occupied[x, y]) continue;
            Vector3 p = new Vector3(x, 0, y);
            // East
            if (x + 1 >= dungeonWidth || !occupied[x + 1, y])
                AddWallVolume(p + Vector3.right, p + Vector3.right + Vector3.forward, wallHeight, wallThickness, ref verts, ref tris, ref uvs);
            // West
            if (x - 1 < 0 || !occupied[x - 1, y])
                AddWallVolume(p + Vector3.forward, p, wallHeight, wallThickness, ref verts, ref tris, ref uvs);
            // North
            if (y + 1 >= dungeonHeight || !occupied[x, y + 1])
                AddWallVolume(p + Vector3.right + Vector3.forward, p + Vector3.forward, wallHeight, wallThickness, ref verts, ref tris, ref uvs);
            // South
            if (y - 1 < 0 || !occupied[x, y - 1])
                AddWallVolume(p, p + Vector3.right, wallHeight, wallThickness, ref verts, ref tris, ref uvs);
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    Rect CreateCorridorRect(Vector2Int a, Vector2Int b)
    {
        int x0 = Mathf.Min(a.x, b.x);
        int y0 = Mathf.Min(a.y, b.y);
        int w = Mathf.Abs(a.x - b.x) + 1;
        int h = Mathf.Abs(a.y - b.y) + 1;
        int pad = corridorWidth / 2;
        x0 -= pad; y0 -= pad;
        w += pad * 2; h += pad * 2;
        return new Rect(x0, y0, w, h);
    }

    void AddQuad(Vector3 origin, Vector3 dir1, Vector3 dir2,
        ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs)
    {
        int i = verts.Count;
        verts.Add(origin);
        verts.Add(origin + dir1);
        verts.Add(origin + dir1 + dir2);
        verts.Add(origin + dir2);
        uvs.AddRange(new[]{Vector2.zero, Vector2.right, Vector2.one, Vector2.up});
        tris.AddRange(new[]{i, i+2, i+1, i, i+3, i+2});
    }

    void AddWallVolume(Vector3 bl, Vector3 br, float height, float thickness,
        ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs)
    {
        Vector3 edge = (br - bl).normalized;
        Vector3 normal = Vector3.Cross(edge, Vector3.up);
        Vector3 bl_in = bl + normal * (thickness * 0.5f);
        Vector3 br_in = br + normal * (thickness * 0.5f);
        Vector3 bl_out = bl - normal * (thickness * 0.5f);
        Vector3 br_out = br - normal * (thickness * 0.5f);

        int i = verts.Count;
        verts.Add(bl_out); verts.Add(br_out); verts.Add(br_in); verts.Add(bl_in);
        uvs.AddRange(new[]{Vector2.zero, Vector2.right, Vector2.one, Vector2.up});
        tris.AddRange(new[]{i, i+2, i+1, i, i+3, i+2});

        int j = verts.Count;
        verts.Add(bl_out + Vector3.up * height);
        verts.Add(br_out + Vector3.up * height);
        verts.Add(br_in + Vector3.up * height);
        verts.Add(bl_in + Vector3.up * height);
        uvs.AddRange(new[]{Vector2.zero, Vector2.right, Vector2.one, Vector2.up});
        tris.AddRange(new[]{j, j+1, j+2, j, j+2, j+3});

        BuildSide(bl_out, bl_in, bl_in + Vector3.up * height, bl_out + Vector3.up * height, ref verts, ref tris, ref uvs);
        BuildSide(br_out, br_in, br_in + Vector3.up * height, br_out + Vector3.up * height, ref verts, ref tris, ref uvs);
        BuildSide(bl_out + Vector3.up * height, br_out + Vector3.up * height, br_out, bl_out, ref verts, ref tris, ref uvs);
        BuildSide(bl_in + Vector3.up * height, br_in + Vector3.up * height, br_in, bl_in, ref verts, ref tris, ref uvs);
    }

    void BuildSide(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
        ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs)
    {
        int k = verts.Count;
        verts.Add(v0); verts.Add(v1); verts.Add(v2); verts.Add(v3);
        uvs.AddRange(new[]{Vector2.zero, Vector2.right, Vector2.one, Vector2.up});
        tris.AddRange(new[]{k, k+2, k+1, k, k+3, k+2});
    }
}
