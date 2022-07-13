using UnityEngine;

[ExecuteAlways]
public class TimeManager : MonoBehaviour
{
    [SerializeField] private int minutesPerCycle = 15;

    [SerializeField] private ParticleSystem fireflyParticles = default;

    public static TimeManager Instance { get; private set; }
    public int MinutesPerCycle { get => minutesPerCycle; }
    public float TimeOfDay { get => timeOfDay; }

    public bool AtNight => (TimeOfDay >= 19f || TimeOfDay <= 5f);

    [SerializeField, Range(0, 24)] private float timeOfDay = 8f;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            timeOfDay = (timeOfDay + Time.deltaTime / (minutesPerCycle * 60 / 24f)) % 24;

            if (fireflyParticles != null)
            {
                if (!AtNight && fireflyParticles.isPlaying)
                {
                    fireflyParticles.Stop();
                }
                if (AtNight && fireflyParticles.isStopped)
                {
                    fireflyParticles.Play();
                }
            }
        }
    }
}
