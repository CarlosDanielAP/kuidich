using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class State 
{
    // Referencia a la maquina de estados
    public FSM fsm;

    // Cada estado tendra el nombre del trigger que lo activa
    public string triggerName;

    // Lista de mensajes o eventos del estado
    private List<Enum> events;

    protected State()
    {
        events = new List<Enum>();
    }

    // Asigna la maquina a la que pertenece el estado
    public void SetFSM(FSM _fsm)
    {
        fsm = _fsm;
    }

    public void ChangeState(Enum id)
    {
        fsm.ChangeState(id);
    }

    public void ChangeState(State newState)
    {
        fsm.ChangeState(newState);
    }

    public void InitBlipState(Enum StateID)
    {
        fsm.InitBlipState(StateID);
    }

    public void InitBlipState(State newState)
    {
        fsm.InitBlipState(newState);
    }

    public void RevertBlipState()
    {
        fsm.RevertBlipState();
    }

    public bool isCurrentState(Enum stateId)
    {
        return fsm.isCurrentState(stateId);
    }

    public virtual void Act(GameObject _object){}
    public virtual void Reason(GameObject _object){}

    public virtual void OnEnter(GameObject _object) { }
    public virtual void OnExit(GameObject _object) { }

    // cosas de animaciones
    private Animator animator;
       
    public void SetAnimationTrigger(string triggerName)
    {
        animator = fsm.myMono.GetComponent<Animator>();

        animator.SetTrigger(triggerName);
    }

    // Cosas de mensajes o eventos
    public void AddEvent(Enum eventID)
    {
        // Antes de agregar evento, checar que no este ya guardado
        if(events.Contains(eventID))
        {
            Debug.Log("El estado ya tiene el evento");
            return;
        }
        events.Add(eventID);
    }

    public bool HasEvent(Enum eventID)
    {
        return events.Contains(eventID);
    }

    public virtual void OnEvent(Enum eventID, List<object> args) { }
}
