using UnityEngine;
using UnityEngine.VFX;

public class PedestalFogo : MonoBehaviour
{
    public VisualEffect fogoVFX;
    public ParticleSystem fogoParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FogoDamage"))
        {
            Debug.Log("Os triggers estão colidindo!");
            Invoke("AtivarEfeitosDeFogo", 1.0f);
        }
    }
    private void AtivarEfeitosDeFogo()
    {
       fogoVFX.Play();
       fogoParticles.Play();     
    }
}
