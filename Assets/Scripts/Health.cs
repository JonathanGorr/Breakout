using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHealth
{
    [Header("Cube")]

    public AudioClip deathSound;
    public virtual int PointValue => StartHealth;

    public int StartHealth;
    [SerializeField] private int health;

    public MeshRenderer renderer;
    public AudioSource asrc;
    public AudioClip hitSound;

    protected MaterialPropertyBlock propertyBlock;
    public float flashDuration = 0.1f;

    public UnityEvent OnHit = new UnityEvent();
    public UnityEvent OnDeath = new UnityEvent();
    private bool Invincible => StartHealth < 0;

    public enum EHealthState
    {
        Alive,
        Dead
    }

    private EHealthState state = EHealthState.Dead;
    public EHealthState State => state;

    protected virtual void Awake()
    {
        health = StartHealth;
        propertyBlock = new MaterialPropertyBlock();
    }

    public void DealDamage(int amount)
    {
        if (state == EHealthState.Dead)
            return;

        if (!Invincible)
        {
            health -= amount;
        }
        if (health <= 0)
        {
            if (!Invincible)
            {
                Death();
                //Destroy(this.gameObject, 1f);
            }
        }
        else
        {
            StartCoroutine(Flash());
            asrc.PlayOneShot(hitSound);
        }
    }

    internal void Kill(bool attributePoints)
    {
        Death();
    }

    public void Resurrect()
    {
        renderer.enabled = true;
        propertyBlock.SetFloat("_Flash", 0f);
        propertyBlock.SetFloat("_Opacity", 1.5f);
        renderer.SetPropertyBlock(propertyBlock);
        health = StartHealth;
        state = EHealthState.Alive;
        StartCoroutine(FadeIn());
    }

    protected virtual void Death()
    {
        state = EHealthState.Dead;
        StartCoroutine(FadeAway());
        asrc.PlayOneShot(deathSound);
        OnDeath.Invoke();
    }

    protected IEnumerator Flash()
    {
        if (flashDuration < 0)
            yield break;

        propertyBlock.SetFloat("_Flash", 1f);
        renderer.SetPropertyBlock(propertyBlock);
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / (flashDuration / 2);
            propertyBlock.SetFloat("_Flash", Mathf.SmoothStep(1, 0, t));
            renderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
        propertyBlock.SetFloat("_Flash", 0f);
        renderer.SetPropertyBlock(propertyBlock);
    }

    protected IEnumerator FadeIn()
    {
        renderer.enabled = true;
        propertyBlock.SetFloat("_Flash", 0f);
        propertyBlock.SetFloat("_Opacity", 0f);
        renderer.SetPropertyBlock(propertyBlock);
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float opacity = Mathf.SmoothStep(1.5f, 0f, t);
            propertyBlock.SetFloat("_Opacity", opacity);
            renderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
        propertyBlock.SetFloat("_Opacity", 1.5f);
        renderer.SetPropertyBlock(propertyBlock);
    }

    protected IEnumerator FadeAway()
    {
        propertyBlock.SetFloat("_Flash", 1f);
        propertyBlock.SetFloat("_Opacity", 1.5f);
        renderer.SetPropertyBlock(propertyBlock);
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float opacity = Mathf.SmoothStep(0f, 1.5f, t);
            propertyBlock.SetFloat("_Opacity", opacity);
            renderer.SetPropertyBlock(propertyBlock);
            yield return null;
        }
        propertyBlock.SetFloat("_Opacity", 0f);
        renderer.SetPropertyBlock(propertyBlock);
        renderer.enabled = false;
    }
}
