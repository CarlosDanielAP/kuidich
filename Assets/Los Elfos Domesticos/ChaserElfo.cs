using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChaserElfoStates;

public class ChaserElfo : Player
{
    public float ThrowStrength;

    // Como hereda de player, ya tiene un FSM y un Steering

    public string TeamTag;
    public string TagEnemigo;
    public Vector3 posicionInicial;

    [HideInInspector]
    public List<Transform> enemigosCercanos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagEnemigo))
            enemigosCercanos.Add(other.transform);

    }

    private void OnTriggerExit(Collider other)
    {
        enemigosCercanos.Remove(other.transform);
    }



    protected override void Start()
    {
        base.Start();
        posicionInicial = transform.position;
        enemigosCercanos = new List<Transform>();
        // Agregar los estados de este agente, chaser
        ChaseBall chase = new ChaseBall(this);
        SearchGoal search = new SearchGoal(this);
        Support supp = new Support(this);
        Defense def = new Defense(this);
        Wait w = new Wait(this);

        fsm.AddState(ChaserStateID.ChaseBall, chase);
        fsm.AddState(ChaserStateID.SearchGoal, search);
        fsm.AddState(ChaserStateID.Support, supp);
        fsm.AddState(ChaserStateID.Defense, def);
        fsm.AddState(ChaserStateID.Wait, w);


        fsm.ChangeState(ChaserStateID.ChaseBall);
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
	}
}
