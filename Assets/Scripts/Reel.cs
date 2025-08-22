using UnityEngine;

public class Reel : MonoBehaviour
{
    [SerializeField] private Transform reelTransform;
    [SerializeField] private ReelStrip strip;
    [SerializeField] private float frictionIdle = 0.0f;

    public int SymbolCount => strip != null ? strip.symbols.Count : 0;
    public bool IsSpinning { get; private set; }

    private float stepAngle;   // degrees per symbol
    private float angularVel;  // deg/sec
    private float targetAngle; // absolute world-space local angle to land on
    private bool decelerating;
    private float decelTime;
    private float decelTimer;

    private void Awake()
    {
        if (strip == null || reelTransform == null)
        {
            Debug.LogError("Reel not configured.");
            enabled = false;
            return;
        }

        stepAngle = 360f / SymbolCount;
    }

    public void BeginSpin(float maxSpeed, float accelTime)
    {
        IsSpinning = true;
        decelerating = false;
        targetAngle = float.NaN;

        // Smoothly accelerate to max
        StopAllCoroutines();
        StartCoroutine(Accelerate(maxSpeed, accelTime));
    }

    public void StopAtIndex(int targetIndex, int overspins, float decelDuration)
    {
        // Compute current normalized angle
        float current = GetLocalZ();
        float targetStepAngle = Mathf.Repeat(-targetIndex * stepAngle, 360f);

        // Full rotations + nearest exact step
        float goal = current + overspins * 360f;
        // Find the next angle >= goal that aligns with the target step
        float deltaToStep = Mathf.DeltaAngle(Mathf.Repeat(goal, 360f), targetStepAngle);
        if (deltaToStep < 0) deltaToStep += 360f;
        targetAngle = goal + deltaToStep;

        decelerating = true;
        decelTime = decelDuration;
        decelTimer = 0f;
    }

    private System.Collections.IEnumerator Accelerate(float maxSpeed, float accelTime)
    {
        float t = 0f;
        float start = angularVel;
        while (t < accelTime)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / accelTime);
            angularVel = Mathf.Lerp(start, maxSpeed, k);
            yield return null;
        }
        angularVel = maxSpeed;
    }

    private void Update()
    {
        if (!IsSpinning) return;

        if (decelerating && !float.IsNaN(targetAngle))
        {
            decelTimer += Time.deltaTime;
            float k = Mathf.Clamp01(decelTimer / decelTime);
            // Ease out (S-curve)
            float ease = 1f - Mathf.Pow(1f - k, 3f);

            // Compute remaining distance
            float current = GetLocalZ();
            float remaining = Mathf.DeltaAngle(current, targetAngle);

            // Reduce velocity proportionally and cap minimal speed
            float plannedVel = Mathf.Max(60f, angularVel * (1f - ease)); // deg/sec
            float step = Mathf.Sign(remaining) * Mathf.Min(Mathf.Abs(remaining), plannedVel * Time.deltaTime);

            SetLocalZ(current + step);

            // Close enough → snap and finish
            if (Mathf.Abs(Mathf.DeltaAngle(GetLocalZ(), targetAngle)) < 0.2f)
            {
                SetLocalZ(targetAngle);
                angularVel = 0f;
                IsSpinning = false;
                decelerating = false;
            }
        }
        else
        {
            // Free spin at current angularVel
            float z = GetLocalZ() + angularVel * Time.deltaTime;
            SetLocalZ(z);

            // Optional tiny friction
            if (frictionIdle > 0f)
                angularVel = Mathf.Max(0f, angularVel - frictionIdle * Time.deltaTime);
        }
    }

    private float GetLocalZ()
    {
        return reelTransform.localEulerAngles.z;
    }

    private void SetLocalZ(float z)
    {
        var e = reelTransform.localEulerAngles;
        e.z = z;
        reelTransform.localEulerAngles = e;
    }
}
