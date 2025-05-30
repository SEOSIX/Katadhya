using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEFeedback : MonoBehaviour
{
    // CREATE VARIABLES
    private Animator animator;
    [SerializeField] string triggerOnEnable = "Feedback";
    [SerializeField] private ParticleSystem feedbackVFX;



    // DEFINE METHODS
    // Start is called before the first frame update
    void awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator = this.GetComponent<Animator>();
        animator.SetTrigger(triggerOnEnable);
    }

    private void OnSuccessfulClick()
    {

        feedbackVFX.Play();
    }

}
