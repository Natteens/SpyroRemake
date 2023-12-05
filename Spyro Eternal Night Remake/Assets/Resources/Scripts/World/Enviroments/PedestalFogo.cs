using UnityEngine;
using UnityEngine.VFX;

public class PedestalFogo : MonoBehaviour
{
    public VisualEffect fogoVFX;
    public ParticleSystem fogoParticles;
    public bool ativo = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FogoDamage"))
        {
            Debug.Log("Os triggers estão colidindo!");
            Invoke("AtivarEfeitosDeFogo", 0.5f);
        }
    }
    private void AtivarEfeitosDeFogo()
    {
       fogoVFX.Play();
       fogoParticles.Play();
        ativo = true;
    }
}
