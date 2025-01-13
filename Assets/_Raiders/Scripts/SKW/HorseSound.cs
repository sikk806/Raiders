using UnityEngine;
using UnityEngine.Audio;

public class HorseSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip StampClip;
    public AudioClip HoresShakeClip;

    void Start()
    {
        // AudioSource를 자동으로 찾기 (필요시 수동 연결 가능)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    void StampSound()
    {
        audioSource.PlayOneShot(StampClip);
    }

    void HoresShakeSound()
    {
        audioSource.PlayOneShot(HoresShakeClip);
    }
}
