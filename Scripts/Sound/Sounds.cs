using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    float volume = 0.5f;

	public AudioClip[] sounds;

    public AudioSource audioSource => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
		audioSource.pitch = Random.Range(p1, p1);
		audioSource.PlayOneShot(clip, volume);
	}
	public void StopSound()
	{
		audioSource.Stop();
	}
}
