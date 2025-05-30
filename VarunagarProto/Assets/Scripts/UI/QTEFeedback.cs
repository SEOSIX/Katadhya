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
    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
    }

    private void OnEnable()
    {
        animator.SetTrigger(triggerOnEnable);
    }

    private void OnSuccessfulClick()
    {

        feedbackVFX.Play();
    }

}
