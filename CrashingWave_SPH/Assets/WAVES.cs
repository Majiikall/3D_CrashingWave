using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAVES : MonoBehaviour
{
  public int dimension = 10;
  //public Octave[] octaves;

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
      Mesh.RecalculateBounds();

      //Add the mesh filter game object
      MeshFilter = gameObject.AddComponent<MeshFilter>();

      //Add the mesh to the filter
      MeshFilter.mesh = Mesh;
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

            }
          }
          verts[index(x, z)] = new Vector3(x, y, z);
        }
      }
      Mesh.vertices = verts;
    }

    //This creates a "object" that unity can call on and instantiate during later times of the program
    [Serializable]
    public struct Octave
    {
      public Vector2 speed;
      public Vector2 scale;
      public float height;
      public bool alternate;
    }
}
