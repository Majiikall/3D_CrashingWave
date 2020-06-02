using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public Particle prefab;
    // Start is called before the first frame update
    void Start()
    {
      print(prefab.position);
      print(prefab.size);
      print(prefab.part);
      Instantiate(prefab.part, prefab.position, Quaternion.identity);
    }

}
