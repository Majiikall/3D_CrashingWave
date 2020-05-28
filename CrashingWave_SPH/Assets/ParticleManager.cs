using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public int amount;
    public GameObject particlePassIn;

    private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);

    private Particle[] particles;
    // Start is called before the first frame update
    void Start()
    {
      particles = new Particle[amount];

      print(amount);

      print(particles[0].position);

      //Instantiate(particles[0].part, particles[0].position, Quaternion.identity);
    }

}
