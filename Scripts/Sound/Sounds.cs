using System.Collections;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    float volume = 0.5f;

	public AudioClip[] clips;
	public bool isPlaying;

	public AudioSource audioSource => GetComponent<AudioSource>();

	public void PlayClip(AudioClip clip, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
		isPlaying = true;

		audioSource.pitch = Random.Range(p1, p1);
		audioSource.PlayOneShot(clip, volume);

		StartCoroutine(backFalsePlaying(clip.length));
	}
	public void StopSound()
	{
		audioSource.Stop();
		isPlaying = false;
	}

	private IEnumerator backFalsePlaying(float duration)
	{
		// ∆дем чтобы проигралс€ трек
		yield return new WaitForSeconds(duration);

		isPlaying = false;
	}
}
