using UnityEngine;

public class StartReelController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public void Start()
    {
        animator.SetTrigger("LeverPulled");
    }
}
