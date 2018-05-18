using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManagerElfo : MonoBehaviour
{
    //lugar para guardar todos los eventos disponibles
    private Dictionary<string, UnityEvent> events;


	private static EventManagerElfo eventManager;

	public static EventManagerElfo EventManagerInstance
    {
        get
        {
            if (!eventManager)
            {
				eventManager = FindObjectOfType(typeof(EventManagerElfo)) as EventManagerElfo;
                //Si no se encuentra quiere decir que no está en el gameManager
                if (!eventManager)
                {
                    Debug.Log("No hay en la escena un objeto con el EventManager");
                }
                else
                {
                    //iniciamos los eventos
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }


    void Init()
    {
        if (events == null)
        {
            events = new Dictionary<string, UnityEvent>();
        }
    }

    //Le permita a los agentes escuchar un evento
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        //Checamos que el evento exista
        if (EventManagerInstance.events.TryGetValue(eventName, out thisEvent))
        {
            //Al agente le decimos que esté pendiendte de este evento
            thisEvent.AddListener(listener);
        }
        else
        {
            //Si no existe el evento, lo podemos crear
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            EventManagerInstance.events.Add(eventName, thisEvent);
        }


    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null)
            return;

        UnityEvent thisEvent = null;
        if (EventManagerInstance.events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;

        //busco si existe el evento para invocarlo
        if (EventManagerInstance.events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}

public abstract class Events
{
    public static string Quaffle = "Quaffle";
    public static string dinnerReady = "dinnerReady";
}

