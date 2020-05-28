using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public GameObject part; //Set the game object
    public int size;  //Size of particle
    public float acceleration;  //Accelreration value
    public Vector3 velocity;  //Velocity direction
    public float mass;  //Mass or density
    public Vector3 position;  //Position in space

    void Start()
    {
      print("here");
      print(size);
      print(part);
      print(position);
      Instantiate(part, position, Quaternion.identity);
    }
}
