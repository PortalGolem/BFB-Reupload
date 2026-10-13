using UnityEngine;

public class PotatoScript : FoodScript
{
    [SerializeField] private GameObject FriesParticles;
    bool hasBeenCut = false;
    public override void Cut(float intensity)
    {
        base.Cut(intensity);

        if (intensity > 0 && !hasBeenCut)
        {
            Instantiate(FriesParticles, transform);
            ParticleSystem particles = FriesParticles.GetComponent<ParticleSystem>();
            particles.Play();
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Cut");
            hasBeenCut = true;
        }
    }
}
