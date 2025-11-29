using UnityEngine;
public class AnimationAudioPlayer : StateMachineBehaviour
{
    [SerializeField] private float playSpeed;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource auido = GameObject.Find("MainAudio").transform.GetChild(4).GetComponent<AudioSource>();
        auido.pitch = playSpeed;
        auido.Play();
        //            mainMusic.GetChild(3).GetComponent<AudioSource>().Play();
    }
}
