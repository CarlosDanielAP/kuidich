using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FSM
{
    // Referencias para trabajar con cosas de unity en la fsm
    public MonoBehaviour myMono;
    public GameObject gameObject;

    // El estado que esta ejecutando actualmente
    private State currentState;
    // El estado blip
    private State blipPreviousState;

    // Estado global, actúa como un controlador
    private State globalState;

    // Si la fsm se esta ejecutando
    private bool isActive;

    // Una lista de los estados que tiene esta maquina
    private Dictionary<Enum, State> states;

    public FSM(GameObject _object, MonoBehaviour _mono)
    {
        myMono = _mono;
        states = new Dictionary<Enum, State>();
        isActive = false;
        currentState = null;
        gameObject = _object;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetGlobalState(State state)
    {
        globalState = state;
        globalState.SetFSM(this);
    }

    /// <summary>
    /// Este método agrega estados a la fsm
    /// </summary>
    /// <param name="stateID"></param>
    /// <param name="state"></param>
    public void AddState(Enum stateID, State state)
    {
        // Verificar que el estado que quiero agregar no esté ya presente
        if(states.ContainsKey(stateID) || states.ContainsValue(state))
        {
            Debug.LogError("No se puede agregar estado, ya existe");
            return;
        }

        state.SetFSM(this);
        states.Add(stateID, state);
    }

    public void Activate()
    {
        // Primero verifico que haya estado en la fsm
        if(states.Count == 0)
        {
            Debug.LogError("No se puede activar FSM, no tiene estados");
            return;
        }
        if(currentState == null)
        {
            Debug.LogError("No puedo activar FSM, no hay un estado inicial");
            return;
        }

        isActive = true;
    }

    public void DeActivate()
    {
        isActive = false;
    }

    public void UpdateFSM()
    {
        if (isActive)
        {
            if (globalState != null)
            {
                globalState.Act(gameObject);
                globalState.Reason(gameObject);
            }
            currentState.Act(gameObject); // cumple las acciones del estado
            currentState.Reason(gameObject); // verifica si alguna transicion se activa
        }
        else
        {
            Debug.Log("La maquina de estados no esta activa");
        }
    }

    State GetStateFromEnum(Enum stateID)
    {
        if(states[stateID] == null)
        {
            Debug.LogError("No econtró el estado con ese ID");
            return null;
        }
        return states[stateID];
    }

    public void ChangeState(Enum stateID)
    {
        // Ejecutamos acciones de salida del estado
        if (currentState != null)
            currentState.OnExit(gameObject);

        // Obtenemos el nuevo estado al que hay que cambiar
        currentState = GetStateFromEnum(stateID);
        // Como ya cambié de estado, ejecuto las acciones de entrada
        currentState.OnEnter(gameObject);
    }

    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.OnExit(gameObject);

        currentState = newState;
        currentState.OnEnter(gameObject);
    }

    public void InitBlipState(State newState)
    {
        // Guardo el estado actual
        blipPreviousState = currentState;

        ChangeState(newState);
    }

    public void InitBlipState(Enum stateID)
    {
        blipPreviousState = currentState;

        ChangeState(stateID);
    }

    public void RevertBlipState()
    {
        if(blipPreviousState == null)
        {
            Debug.LogError("Intentando revertir un blip sin definir");
            return;
        }

        ChangeState(blipPreviousState); // Regresa por donde vino
        blipPreviousState = null;
    }

    public State GetBlipState()
    {
        return blipPreviousState;
    }

    // Para notificar eventos ( mensajes )
    public void NotifyEvent(Enum eventID, List<object> arguments)
    {
        if(!isActive)
        {
            Debug.LogError("No puedo notificar evento, fms no esta activa");
            return;
        }

        // Antes de avisar que hay un evento, tengo que asegurarme que el estado
        // tenga dicho evento
        if(currentState.HasEvent(eventID))
        {
            // Si lo tiene, entonces llamo un metodo que trate el mensaje
            currentState.OnEvent(eventID, arguments);
        }
    }

    public bool isCurrentState(State state)
    {
        return currentState == state;
    }

    public bool isCurrentState(Enum stateId)
    {
        return currentState == GetStateFromEnum(stateId);
    }
}
