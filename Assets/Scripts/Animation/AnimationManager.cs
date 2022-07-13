using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    public Animator Animator => animator;
    public int CurrentAnimation => currentAnimation;
    public bool IsActive => isActive;
    public bool IsPlaying => animator.GetCurrentAnimatorStateInfo(0).length >
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    private Animator animator;
    private int currentAnimation;
    private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isActive = true;
    }

    public void ChangeAnimation(int newAnimation)
    {
        if (isActive && animator != null)
        {
            if (newAnimation == currentAnimation)
            {
                return;
            }
            animator.Play(newAnimation, -1);
            ResumeAnimation();

            currentAnimation = newAnimation;
        }
    }

    public void ChangeAnimation(int newAnimation, float speed)
    {
        if (isActive && animator != null)
        {
            if (newAnimation == currentAnimation)
            {
                return;
            }
            animator.SetFloat("speedMult", speed);
            animator.Play(newAnimation, -1);
            ResumeAnimation();

            currentAnimation = newAnimation;
        }
    }

    public void OverrideAnimation(int newAnimation)
    {
        if (isActive && animator != null)
        {
            animator.Play(newAnimation, -1);
            ResumeAnimation();

            currentAnimation = newAnimation;
        }
    }

    public void ControlAnimation(int animation, float normalizedTime)
    {
        if (isActive && animator != null)
        {
            PauseAnimation();
            animator.Play(animation, -1, normalizedTime);

            currentAnimation = animation;
        }
    }

    public void PauseAnimation()
    {
        if (isActive && animator != null)
        {
            animator.speed = 0f;
        }
    }
    
    public void ResumeAnimation()
    {
        if (isActive && animator != null)
        {
            animator.speed = 1f;
        }
    }

    public void ClearLayerWeights()
    {
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0f);
        }
    }
}
