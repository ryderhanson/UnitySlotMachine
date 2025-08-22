using UnityEngine;
using System.Collections;
using System.Linq;

public class SlotMachineController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Reels")]
    [SerializeField] private Reel[] reels; // 0..N-1 left to right
    [SerializeField] private float reelMaxSpeed = 1080f;     // deg/sec
    [SerializeField] private float reelAccelTime = 0.35f;    // sec to ramp up
    [SerializeField] private float reelDecelTime = 0.6f;     // sec to ramp down
    [SerializeField] private float reelStopStagger = 0.25f;  // delay between reel stops

    [Header("Events/SFX")]
    [SerializeField] private AudioSource sfxStart;
    [SerializeField] private AudioSource sfxStop;
    [SerializeField] private AudioSource sfxWin;

    private bool spinning;

    // Hooked by Animation Event on Lever_Pull
    public void OnLeverPulled()
    {
        if (spinning) return;
        StartCoroutine(SpinRoutine());
    }

    public void PressSpinButton() // optional UI/button path
    {
        if (spinning) return;
        animator.SetTrigger("Spin"); // plays Lever_Pull on Base layer
    }

    private IEnumerator SpinRoutine()
    {
        spinning = true;
        animator.SetBool("IsSpinning", true);
        if (sfxStart) sfxStart.Play();

        // Decide outcome (replace with RNG or server-authoritative result)
        int[] result = DecideOutcome();

        // Kickoff acceleration for all reels
        foreach (var reel in reels)
            reel.BeginSpin(reelMaxSpeed, reelAccelTime);

        // Staggered stopping
        for (int i = 0; i < reels.Length; i++)
        {
            // overshoot (full rotations) before landing
            int overspins = Random.Range(3, 6);
            reels[i].StopAtIndex(result[i], overspins, reelDecelTime);
            if (i < reels.Length - 1)
                yield return new WaitForSeconds(reelStopStagger);
        }

        // Wait until the last reel fully stopped
        while (reels.Any(r => r.IsSpinning))
            yield return null;

        if (sfxStop) sfxStop.Play();

        bool isWin = EvaluateWin(result);
        if (isWin)
        {
            animator.SetTrigger("WinFlash");
            if (sfxWin) sfxWin.Play();
            // Optionally: animator.SetTrigger("Payout");
        }

        animator.SetBool("IsSpinning", false);
        spinning = false;
    }

    private int[] DecideOutcome()
    {
        // Example: random; swap with paytable logic or server result.
        var res = new int[reels.Length];
        for (int i = 0; i < reels.Length; i++)
            res[i] = Random.Range(0, reels[i].SymbolCount);
        return res;
    }

    private bool EvaluateWin(int[] result)
    {
        // Example: all same → win
        return result.All(x => x == result[0]);
    }
}
