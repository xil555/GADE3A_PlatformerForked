using UnityEngine;

public interface IState 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Enter();


    // Update is called once per frame
    void Update();

    void Exit();
    
}
