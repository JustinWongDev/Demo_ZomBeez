using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droppable : MonoBehaviour
{
    public float dropHeight = 20.0f;
    private Vector3 mousePos;
    private Vector3 lastPos;
    private bool isDropped = false;
    private bool isGrounded = false;
    public bool IsGround => isGrounded;
    
    private HumanController _controller;
    private Animator _animator;
    private Camera mainCam => Camera.main;
    

    private void Update()
    {
        //Hold and drop 
        HoldAndDrop();

        //Start moving, after dropping to floor
        CheckToStartMoving();
    }

    public void Initialise(HumanController controller, Animator anim)
    {
        _controller = controller;
        _animator = anim;
    }
    
    private int findClosestWaypoint()
    {
        //Convert mouse coordinates to world position
        if (!(mainCam is null))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                mousePos = hit.point;
            }
        }
        
        Debug.DrawLine(mainCam.transform.position, mousePos, Color.red);
        
        float distance = Mathf.Infinity;
        int closestWaypoint = 0;
        
        //Find closest node to mouse position
        for (int i = 0; i < _controller.graphNodes.graphNodes.Length; i++)
        {
            if (Vector3.Distance(mousePos, _controller.graphNodes.graphNodes[i].transform.position) <= distance)
            {
                distance = Vector3.Distance(mousePos, _controller.graphNodes.graphNodes[i].transform.position);
                closestWaypoint = i;
            }
        }
        
        return closestWaypoint;
    }
    
    private void HoldAndDrop()
    {
        if (!isDropped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDropped = true;
                _controller.currentPath.Clear();
                _controller.currentNodeIndex = findClosestWaypoint();
                _controller.currentPath.Add(_controller.currentNodeIndex);
            }

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            //If ray hits...
            var transformPosition = transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //print(hit.collider.name);

                //Hover over ray hit
                Vector3 pos = hit.point;
                pos.y += dropHeight;
                transform.position = pos;
                lastPos = pos;

                //Draw line indicating drop location
                Debug.DrawLine(transformPosition, hit.point, Color.green);
            }
            else
            {
                //Hover over last ray hit
                transform.position = lastPos;

                Vector3 pos = lastPos;
                pos.y -= dropHeight;

                //Draw line indicating drop location
                Debug.DrawLine(transformPosition, pos, Color.green);
            }
        }
        else
        {
            isDropped = true;
            _animator.SetTrigger("isDropped");
        }
    }
    
     void CheckToStartMoving()
     {
         if (_animator.GetCurrentAnimatorStateInfo(0).IsName("2D Blend Tree"))
         {
             _controller.currentPathing = Pathfinding.Astar;
         }
     }
    
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            _animator.SetTrigger("isGrounded");
        }
    }
}
