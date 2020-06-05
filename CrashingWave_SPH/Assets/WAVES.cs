using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAVES : MonoBehaviour
{
  public int dimension = 10;
  public Octave[] Octaves;
  public float UVscale;

  protected MeshFilter MeshFilter;
  protected Mesh Mesh;

    // Start is called before the first frame update
    void Start()
    {
      //Create new mesh
      Mesh = new Mesh();

      //Mesh can be game object
      Mesh.name = gameObject.name;

      //Generate vertices, triangles and then find bounds using methods below
      Mesh.vertices = GenerateVerts();
      Mesh.triangles = GenerateTries();
      Mesh.uv = GenerateUVs();
      Mesh.RecalculateBounds();
      Mesh.RecalculateNormals();

      //Add the mesh filter game object
      MeshFilter = gameObject.AddComponent<MeshFilter>();

      //Add the mesh to the filter
      MeshFilter.mesh = Mesh;
    }

    public float GetHeight(Vector3 position)
    {
        //scale factor and position in local space
        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale((position - transform.position), scale);

        // get edge points
        var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        // clamp if the position is outside the plane
        p1.x = Mathf.Clamp(p1.x, 0, dimension);
        p1.z = Mathf.Clamp(p1.z, 0, dimension);
        p2.x = Mathf.Clamp(p2.x, 0, dimension);
        p2.z = Mathf.Clamp(p2.z, 0, dimension);
        p3.x = Mathf.Clamp(p3.x, 0, dimension);
        p3.z = Mathf.Clamp(p3.z, 0, dimension);
        p4.x = Mathf.Clamp(p4.x, 0, dimension);
        p4.z = Mathf.Clamp(p4.z, 0, dimension);

        // get the max distance to one of the edges and take that to compute max - distance
        var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        var dist = (max - Vector3.Distance(p1, localPos))
                 + (max - Vector3.Distance(p2, localPos))
                 + (max - Vector3.Distance(p3, localPos))
                 + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        // weighted sum
        var height = Mesh.vertices[(int)index2(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                   + Mesh.vertices[(int)index2(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                   + Mesh.vertices[(int)index2(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                   + Mesh.vertices[(int)index2(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));
        //Scale
        return height * transform.lossyScale.y / dist;
    }

    private Vector3[] GenerateVerts()
    {
      //Make a new array of vector 3 to work as vertices
      var verts = new Vector3[(dimension + 1) * (dimension + 1)];

      //Iterate through each vertex in the array
      for(int x = 0; x <= dimension; ++x)
      {
        for(int y = 0; y <= dimension; ++y)
        {
          //Store the new vector3 index into the array of vertices
          //Index is done by using x to multiply into the tens digit and then adding y
          verts[index(x, y)] = new Vector3(x, 0, y);
        }
      }

      //Return all the vertices
      return verts;
    }

    private int index(int x, int y)
    {
      return ((x * (dimension + 1)) + y);
    }

    private float index2(float x, float y)
    {
      return ((x * (dimension + 1)) + y);
    }

    private int[] GenerateTries()
    {
      //Split squares into 2 triangles
      var tries = new int[Mesh.vertices.Length * 6];

      for(int x = 0; x < dimension; ++x)
      {
        for(int y = 0; y < dimension; ++y)
        {
          //Set the tries for the indices by increments of 6
          tries[index(x, y) * 6 + 0] = index(x, y);
          tries[index(x, y) * 6 + 1] = index(x+1, y+1);
          tries[index(x, y) * 6 + 2] = index(x+1, y);
          tries[index(x, y) * 6 + 3] = index(x, y);
          tries[index(x, y) * 6 + 4] = index(x, y+1);
          tries[index(x, y) * 6 + 5] = index(x+1, y+1);
        }
      }

      return tries;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[Mesh.vertices.Length];

        // always set one uv over n tiles then flip the uv and set it again
        for (int x = 0; x <= dimension; x++)
        {
            for (int z = 0; z <= dimension; z++)
            {
                var vec = new Vector2((x / UVscale) % 2, (z / UVscale) % 2);
                uvs[index(x,z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }
        return uvs;
    }

    // Update is called once per frame
    void Update()
    {
      var verts = Mesh.vertices;

      for(int x = 0; x < dimension; ++x)
      {
        for(int z = 0; z < dimension; ++z)
        {
          var y = 0f;
          for(int o = 0; o < Octaves.Length; ++o)
          {
            if(Octaves[o].alternate)
            {
                var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x) / dimension, (z * Octaves[o].scale.y) / dimension) * Mathf.PI * 2f;
                y += (Mathf.Cos(perl + Octaves[o].speed.magnitude * Time.time) * Octaves[o].height);
            }
            else {
                var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x + Time.time * Octaves[o].speed.x) / dimension, (z * Octaves[o].scale.y + Time.time * Octaves[o].speed.y) / dimension) - .5f;
                y += perl * Octaves[o].height;
            }
          }
          verts[index(x, z)] = new Vector3(x, y, z);
        }
      }
      Mesh.vertices = verts;
      Mesh.RecalculateNormals();
    }

    //This creates a "object" that unity can call on and instantiate during later times of the program
    [System.Serializable]
    public struct Octave
    {
      public Vector2 speed;
      public Vector2 scale;
      public float height;
      public bool alternate;
    }
}
