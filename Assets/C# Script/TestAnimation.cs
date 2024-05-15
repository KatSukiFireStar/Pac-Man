using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    public Animator animator;
    private bool[] direction = { true, false, false, false };

    void Start()
    {
        StartCoroutine("changeAnimation");
    }

    private IEnumerator changeAnimation()
    {
        if (direction[0])
        {
            animator.SetBool("down", false);
            animator.SetBool("left", true);
            direction[0] = false;
            direction[1] = true;
        }else if (direction[1])
        {
            animator.SetBool("left", false);
            animator.SetBool("up",true);
            direction[1] = false;
            direction[2] = true;
        }else if (direction[2])
        {
            animator.SetBool("up", false);
            animator.SetBool("right", true);
            direction[2] = false;
            direction[3] = true;
        }else if (direction[3])
        {
            animator.SetBool("right", false);
            animator.SetBool("down", true);
            direction[3] = false;
            direction[0] = true;
        }

        yield return new WaitForSeconds(2);
        StartCoroutine("changeAnimation");
    }
}
