using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    private Animator animator;
    public float shadowTime;
    public bool doneAnimating { get; set; } = false;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenShadow()
    {
        doneAnimating = false;
        animator.SetBool("shadowClosed", false);
        StartCoroutine(Wait());
    }

    public void CloseShadow()
    {
        doneAnimating = false;
        animator.SetBool("shadowClosed", true);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(shadowTime);
        doneAnimating = true;
    }
}
