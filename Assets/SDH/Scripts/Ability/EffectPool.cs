using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GenerateEffectPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject lightningLine;
    public GameObject lightningEffect;
    Queue<GameObject> lightningLinePool = new();
    Queue<GameObject> lightningEffectPool = new();

    public GameObject iceEffect;
    Queue<GameObject> iceEffectPool = new();

    public GameObject poisonEffect;
    Queue<GameObject> poisonEffectPool = new();


    void GenerateEffectPool()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject lineObj = Instantiate(lightningLine);
            lineObj.SetActive(false);
            lightningLinePool.Enqueue(lineObj);
            GameObject effectObj = Instantiate(lightningEffect);
            effectObj.SetActive(false);
            lightningEffectPool.Enqueue(effectObj);
            GameObject iceObj = Instantiate(iceEffect);
            iceObj.SetActive(false);
            iceEffectPool.Enqueue(iceObj);
            GameObject poisonObj = Instantiate(poisonEffect);
            poisonObj.SetActive(false);
            poisonEffectPool.Enqueue(poisonObj);
        }
    }

    public LineRenderer GetLightningLine()
    {
        if (lightningLinePool.Count > 0)
        {
            GameObject obj = lightningLinePool.Dequeue();
            obj.SetActive(true);
            return obj.GetComponent<LineRenderer>();
        }
        else
        {
            GameObject obj = Instantiate(lightningLine);
            obj.SetActive(true);
            return obj.GetComponent<LineRenderer>();
        }
    }

    public ParticleSystem GetLightningEffect()
    {
        if (lightningEffectPool.Count > 0)
        {
            GameObject obj = lightningEffectPool.Dequeue();
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
        else
        {
            GameObject obj = Instantiate(lightningEffect);
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
    }

    public ParticleSystem GetIceEffect()
    {
        if (iceEffectPool.Count > 0)
        {
            GameObject obj = iceEffectPool.Dequeue();
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
        else
        {
            GameObject obj = Instantiate(iceEffect);
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
    }

    public ParticleSystem GetPoisonEffect()
    {
        if (poisonEffectPool.Count > 0)
        {
            GameObject obj = poisonEffectPool.Dequeue();
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
        else
        {
            GameObject obj = Instantiate(poisonEffect);
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }
    }

    public void ReturnLightningLine(LineRenderer line)
    {
        if (line == null) return;
        line.gameObject.SetActive(false);
        lightningLinePool.Enqueue(line.gameObject);
    }
    public void ReturnLightningEffect(ParticleSystem effect)
    {
        if (effect == null) return;
        effect.gameObject.SetActive(false);
        lightningEffectPool.Enqueue(effect.gameObject);
    }

    public void ReturnIceEffect(ParticleSystem effect)
    {
        if (effect == null) return;
        effect.gameObject.SetActive(false);
        iceEffectPool.Enqueue(effect.gameObject);
    }

    public void ReturnPoisonEffect(ParticleSystem effect)
    {
        if (effect == null) return;
        effect.gameObject.SetActive(false);
        poisonEffectPool.Enqueue(effect.gameObject);
    }
}
