using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStartAnim : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(WaitRandom());
    }

    public IEnumerator WaitRandom()
    {
        yield return new WaitForSeconds(Random.Range(0, 6));
        animator.SetBool("randomTime", true);
    }
}
