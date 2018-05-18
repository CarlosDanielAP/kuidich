using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringElfos : MonoBehaviour
{
    public float maxSpeed;
    public float maxForce;
    private Rigidbody rb;

    public Transform Target;

    Vector3 Velocity;
    Vector3 SteeringForce;

    [Header("Seek Settings")]
    public bool seek;
    public float seekWeight;
    [Header("Flee Settings")]
    public bool flee;
    public float fleeWeight;
    [Header("Arrive Settings")]
    public bool arrive;
    public float arriveWeight;
    [Header("Pursuit Settings")]
    public bool pursuit;
    public float pursuitWeight;
    [Header("Obstacle avoidance Settings")]
    public bool obstacleAvoidance;
    public float obstacelAvoidanceWeight;
    private BoxCollider obstacleDetector;
    public float obstacleDetectorMinLength;
    public List<GameObject> Obstacles;
    [Header("Wander Settings")]
    public bool wander;
    public float wanderWeight;
    public float wanderJitter;
    public float wanderRadius;
    public float wanderDistance;
    [Header("Wall Avoidance Settings")]
    public bool wallAvoidance;
    public float wallAvoidanceWeight = 1f;
    public float wallAvoidanceSensorLenght;

    [Header("Interpose Settings")]
    public bool interpose;
    public float interposeWeight = 1f;
    public GameObject agent1;
    public GameObject agent2;

    [Header("Cohesion Settings")]
    public bool cohesion;
    public bool teamCohesion;
    public float cohesionWeight;
    [Header("Separation Settings")]
    public bool separation;
    public bool teamSeparation;
    public float separationWeight;
    [Header("Alignment Settings")]
    public bool alignment;
    public bool teamAlignment;
    public float alignmentWeight;
    //...etc

    public void ShutDownAll()
    {
        seek = flee = arrive = pursuit = obstacleAvoidance = cohesion = wander =interpose= false;

    }

    public enum combiningMethod
    {
        Weighted_Truncated_Sum,
        Prioritazed,
        Dithered
    };
    public combiningMethod myCombiningMethod;

    // Delegado para combinar metodos
    delegate void SteeringDelegate();
    SteeringDelegate mySteeringDelegate;


    // Para arrive
    public enum Deceleration
    {
        fast = 1,
        normal = 2,
        slow = 3
    };
    public Deceleration deceleration;


    // Variables de grupo
    public List<GameObject> NearPlayers;
    public List<GameObject> NearTeammates;
    public List<GameObject> NearRivals;
    public float nearPlayersSensorRadius;
    public SphereCollider NearPlayersSensor;


    /// <summary>
    /// Seleccionar el comportamiento de Steering y guardar SteeringForce
    /// </summary>
    void Calculate()
    {
        // Resetear la fuerza
        SteeringForce = Vector3.zero;

        // usar el metodo de combinación elegido
        switch (myCombiningMethod)
        {
            case combiningMethod.Weighted_Truncated_Sum:
                WeightedTruncatedSum();
                break;
            case combiningMethod.Prioritazed:

                break;
            case combiningMethod.Dithered:

                break;
        }
    }

    ////////////////////////////////////////////////////Funciones de combinacion
    void WeightedTruncatedSum()
    {
        // limpiar el delegado
        mySteeringDelegate = null;

        // Verificar que funciones de steering queremos agregar al delegado

        if (seek)
            mySteeringDelegate += Seek; // si esta encedido seek, entonces al delegado
                                        // le pego la funcion Seek()
        if (flee)
            mySteeringDelegate += Flee;
        if (arrive)
            mySteeringDelegate += Arrive;
        if (pursuit)
            mySteeringDelegate += Pursuit;
        if (obstacleAvoidance)
            mySteeringDelegate += ObstacleAvoidance;
        if (wander)
            mySteeringDelegate += Wander;
        if (wallAvoidance)
            mySteeringDelegate += WallAvoidance;
        if (interpose)
            mySteeringDelegate += Interpose;

        if (cohesion)
            mySteeringDelegate += Cohesion;
        if (separation)
            mySteeringDelegate += Separation;
        if (alignment)
            mySteeringDelegate += Alignment;

        if (teamCohesion)
            mySteeringDelegate += TeamCohesion;
        if (teamSeparation)
            mySteeringDelegate += TeamSeparation;
        if (teamAlignment)
            mySteeringDelegate += TeamAlignment;

        // Ya que sabemos que funciones hay que ejecutar, llamamos al delegado
        if (mySteeringDelegate != null)
            mySteeringDelegate();

        // Probablemente la acumulacion de los metodos haga que el vector de steering
        // crezca demasiado, entonces hay que cortarlo
        SteeringForce = Vector3.ClampMagnitude(SteeringForce, maxForce);
    }



    //////////////////////////////////////////////////////////////////////////////
    void Seek()
    {
        Vector3 DesiredVelocity =
            (Target.position - transform.position).normalized * maxSpeed;

        SteeringForce += (DesiredVelocity - Velocity) * seekWeight;
    }

    void Flee()
    {
        // Tarea: Agregarle distancia de panico
        Vector3 DesiredVelocity =
            (transform.position - Target.position).normalized * maxSpeed;

        SteeringForce += (DesiredVelocity - Velocity) * fleeWeight;
    }

    void Arrive()
    {
        Vector3 ToTarget = Target.position - transform.position;

        // Calcula la distancia al objetivo
        float distance = ToTarget.magnitude;

        if (distance > 0f)
        {
            float decelTweaker = 1.5f;

            // Calculamos la velocidad requerida para llegar al objetivo
            // dada una desaceleración
            float speed = distance / ((float)deceleration * decelTweaker);

            // Verificar que la velocidad obtenida no supere la maxima del agente
            speed = Mathf.Min(speed, maxSpeed);

            // Hacemos como seek pero sin normalizar puesto que ya calculamos la 
            // distancia
            Vector3 DesiredVelocity = ToTarget * speed / distance;

            SteeringForce += (DesiredVelocity - Velocity) * arriveWeight;
            return;
        }
        // Si ya estoy en el objetivo, ya no aplico fuerza
        SteeringForce += Vector3.zero;
    }

    void Pursuit()
    {
        // Si el target esta encarando al agente, podemos solo llamar a seek
        Vector3 ToEvader = Target.position - transform.position;

        float relativeHeading = Vector3.Dot(transform.forward, Target.forward);

        if (Vector3.Dot(ToEvader, transform.forward) > 0f &&
            relativeHeading < -0.95f)
        {
            Seek();
            return;
        }

        // Si llega acá entonces tratamos de predecir en donde estará el evasor
        float lookAheadTime = ToEvader.magnitude /
            (maxSpeed + Target.GetComponent<Rigidbody>().velocity.magnitude);

        // Ahora hacemos Seek a la posicion futura del evasor
        Vector3 futurePosition =
            Target.position + Target.GetComponent<Rigidbody>().velocity * lookAheadTime;

        // Código de seek
        Vector3 DesiredVelocity =
            (futurePosition - transform.position).normalized * maxSpeed;

        SteeringForce += (DesiredVelocity - Velocity) * pursuitWeight;
    }

    void ObstacleAvoidance()
    {
        // Ajustar el tamaño del detector de obstaculos de acuerdo a la velocidad del agente
        Vector3 newSize = obstacleDetector.size;

        newSize.z = obstacleDetectorMinLength +
            (rb.velocity.magnitude / maxSpeed) * obstacleDetectorMinLength;

        obstacleDetector.size = newSize;

        // Mover el colisionador de acuerdo a su nuevo tamaño
        Vector3 newPos = obstacleDetector.center;
        newPos.z = newSize.z / 2f;
        obstacleDetector.center = newPos;

        // Verificar si choco con obstaculos y cual de ellos es el mas cercano
        if (Obstacles.Count > 0)
        {
            // Buscar cual es el obstaculo mas cercano
            int closestObstacle = -1;
            float distanceToObstacle = float.MaxValue;

            for (int obs = 0; obs < Obstacles.Count; obs++)
            {
                // mido la distancia entre el agente y este obstaculo
                float dist =
                    Vector3.Distance(transform.position, Obstacles[obs].transform.position);

                if (dist < distanceToObstacle)
                {
                    // Encontramos un obstaculo mas cercano
                    closestObstacle = obs;
                    distanceToObstacle = dist;
                }
            } // llave del for

            // Ya que sabemos cual es el obstaculo mas cercano, hay que evitarlo.
            Vector3 steeringTemp = Vector3.zero;

            // Mientras mas cercano este el obstaculo, mayor debe ser la fuerza que nos aleje de el
            // Transformamos la posicion del obstaculo a espacio local en base al agente.
            Vector3 localObs =
                transform.InverseTransformPoint(Obstacles[closestObstacle].transform.position);
            float multiplier = 1f + (obstacleDetector.size.z - localObs.z) /
                                obstacleDetector.size.z;

            // Calculamos la fuerza lateral en x
            float radioObs = Obstacles[closestObstacle].GetComponent<SphereCollider>().radius;
            steeringTemp.x = (radioObs - localObs.x) * multiplier;
            // Tambien se puede calcular una fuerza en y
            //float radioObs = Obstacles[closestObstacle].GetComponent<SphereCollider>().radius;
            //steeringTemp.y = (radioObs - localObs.y) * multiplier;

            // Aplicamos una fuerza de freno, proporcional a la distancia del agente al obstaculo
            float brakingWeight = 0.2f;

            steeringTemp.z = (radioObs - localObs.z) * brakingWeight;

            SteeringForce += steeringTemp * obstacelAvoidanceWeight;
        }
    }

    private Vector3 wanderTarget = Vector3.zero;
    void Wander()
    {

        Random.InitState(System.DateTime.Now.Millisecond);
        //this behavior is dependent on the update rate, so this line must
        //be included when using time independent framerate.
        //float JitterThisTimeSlice = wanderJitter * Time.deltaTime;

        //first, add a small random vector to the target's position
        wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter,
                                    Random.Range(-1f, 1f) * wanderJitter,
                                    Random.Range(-1f, 1f) * wanderJitter);
        //wanderTarget += Random.onUnitSphere;
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        //Vector3 newTarget = wanderTarget + new Vector3(0f, 0f, wanderDistance);

        wanderTarget *= wanderDistance;

        //newTarget = transform.TransformPoint(wanderTarget);
        // newTarget -= transform.position;

        SteeringForce += wanderTarget * wanderWeight;
    }

    void WallAvoidance()
    {
        
        RaycastHit[] hit = new RaycastHit[5];
        bool[] didHit = new bool[5];
        //feeler pointing straight in front
        didHit[0] = Physics.Raycast(transform.position, transform.forward, out hit[0], wallAvoidanceSensorLenght);
        Debug.DrawRay(transform.position, transform.forward* wallAvoidanceSensorLenght, Color.blue);

        //feeler to left
        Vector3 temp = transform.forward;
        temp = Quaternion.AngleAxis(45, transform.up) * temp;
        didHit[1] = Physics.Raycast(transform.position, temp, out hit[1], wallAvoidanceSensorLenght);
        Debug.DrawRay(transform.position, temp * wallAvoidanceSensorLenght, Color.blue);

        //feeler to right
        Vector3 temp2 = transform.forward;
        temp2 = Quaternion.AngleAxis(-45, transform.up) * temp2;
        didHit[2] = Physics.Raycast(transform.position, temp2, out hit[2], wallAvoidanceSensorLenght);
        Debug.DrawRay(transform.position, temp2 * wallAvoidanceSensorLenght, Color.blue);

        //feeler to up
        Vector3 temp3 = transform.forward;
        temp3 = Quaternion.AngleAxis(45, transform.right) * temp3;
        didHit[3] = Physics.Raycast(transform.position, temp3, out hit[3], wallAvoidanceSensorLenght);
        Debug.DrawRay(transform.position, temp3 * wallAvoidanceSensorLenght, Color.blue);

        //feeler to down
        Vector3 temp4 = transform.forward;
        temp4 = Quaternion.AngleAxis(-45, transform.right) * temp4;
        didHit[4] = Physics.Raycast(transform.position, temp4, out hit[4], wallAvoidanceSensorLenght);
        Debug.DrawRay(transform.position, temp4 * wallAvoidanceSensorLenght, Color.blue);



        float DistToThisIP = 0f;
        float DistToClosestIP = float.MaxValue;

        Vector3 SteeringForceW = Vector3.zero,
                ClosestPoint; //holds the closest intersection point

        //examine each feeler in turn
        for (int h = 0; h < hit.Length; h++)
        {
            //run through each sensor checking for any intersection points
            if (didHit[h] && hit[h].collider.tag.Equals("Boundary"))
            {
                ////("IMPACT ON WALL, sensor " + h);
                if (DistToThisIP < DistToClosestIP)
                {
                    // See if this is the closest wall
                    DistToClosestIP = DistToThisIP;
                    ClosestPoint = hit[h].point;

                    //if an intersection point has been detected, calculate a force
                    //that will direct the agent away             

                    //calculate by what distance the projected position of the agent
                    //will overshoot the wall
                    Vector3 OverShoot = transform.position - ClosestPoint;
                    //create a force in the direction of the wall normal, with a
                    //magnitude of the overshoot
                    SteeringForceW = hit[h].normal * OverShoot.magnitude;
                }
            }
        }

        SteeringForce += SteeringForceW * wallAvoidanceWeight;
    }

    //est es para interpose
   

    void Interpose()
    {
        //primero necesitamos averiguar en donde estaran los dos agentes en el futuro
        Vector3 MidPoint = (agent1.transform.position + agent2.transform.position) / 2;
        MidPoint.Normalize();
        float TimeToReachMidPoint = Vector3.Distance(transform.position, MidPoint) / maxSpeed;
        //obtener las futuras posiciones
        Vector3 Apos = (agent1.transform.position + Velocity) * TimeToReachMidPoint;
        Vector3 Bpos = (agent2.transform.position + Velocity) * TimeToReachMidPoint;
        Apos.Normalize();
        Bpos.Normalize();
        //calcular el punto medio de estas posiciones
        MidPoint = (Apos + Bpos) / 2;

        //luego se ocupa arrive para llegar a esta posicion
        Vector3 ToTarget = MidPoint - transform.position;

        //Calcular la distancia al objetivo
        float distance = ToTarget.magnitude;

        //Aun no llega al objetivo, entonces tiene que acercarse
        if (distance > 0)
        {
            float DecelTweaker = 0.3f;

            //Calculamos la velocidad para llegar al objetivo, tomando en cuenta la desaceleracion deseada
            float speed = distance / ((float)deceleration * DecelTweaker);

            //Checamos que la velocidad no supere el maximo
            speed = Mathf.Min(speed, maxSpeed);

            //De aqui es como Seek, pero sin normalizatr ya que calculamos la velocidad deseada
            Vector3 DesiredVelocity = ToTarget * speed / distance;

            SteeringForce = (DesiredVelocity - GetComponent<Rigidbody>().velocity)*interposeWeight;
            return;
        }

        //Si la distanica es cero, no me muevo porque ya llegue al objetivo
        SteeringForce = Vector3.zero*interposeWeight;

    }

    // -------------------------------------------------------------------------------
    // --------------- COMPORTAMIENTOS DE GRUPO
    // -------------------------------------------------------------------------------

    void Cohesion()
    {
        // Encontrar el centro de masa del grupo de vecinos
        Vector3 CenterOfMass = Vector3.zero;
        Vector3 DesiredVelocity = Vector3.zero;

        if (NearPlayers.Count > 0)
        {
            foreach (GameObject vecino in NearPlayers)
            {
                CenterOfMass += vecino.transform.position;
            }
            CenterOfMass /= NearPlayers.Count;

            // Aplicar una fuerza hacia el centro de masa encontrado
            DesiredVelocity = (CenterOfMass - transform.position).normalized
                                * maxSpeed;

            DesiredVelocity = DesiredVelocity - Velocity;
        }

        SteeringForce += DesiredVelocity * cohesionWeight;
    }

    void Separation()
    {
        Vector3 Steering = Vector3.zero;

        foreach (GameObject persecutor in NearPlayers)
        {
            // make sure it doesn't include the evade target 
            //if ((neighbors[a] != m_pTargetAgent1))
            {
                Vector3 ToAgent = transform.position - persecutor.transform.position;

                //scale the force inversely proportional to the agents distance  
                //from its neighbor.
                Steering += ToAgent.normalized / ToAgent.magnitude;
            }
        }

        SteeringForce += Steering * separationWeight;
    }

    void Alignment()
    {

    }

    void TeamCohesion()
    {
        // Encontrar el centro de masa del grupo de vecinos
        Vector3 CenterOfMass = Vector3.zero;
        Vector3 DesiredVelocity = Vector3.zero;

        if (NearTeammates.Count > 0)
        {
            foreach (GameObject vecino in NearPlayers)
            {
                CenterOfMass += vecino.transform.position;
            }
            CenterOfMass /= NearTeammates.Count;

            // Aplicar una fuerza hacia el centro de masa encontrado
            DesiredVelocity = (CenterOfMass - transform.position).normalized
                                * maxSpeed;

            DesiredVelocity = DesiredVelocity - Velocity;
        }

        SteeringForce += DesiredVelocity * cohesionWeight;
    }

    void TeamSeparation()
    {
        Vector3 Steering = Vector3.zero;

        foreach (GameObject persecutor in NearTeammates)
        {
            // make sure it doesn't include the evade target 
            //if ((neighbors[a] != m_pTargetAgent1))
            {
                Vector3 ToAgent = transform.position - persecutor.transform.position;

                //scale the force inversely proportional to the agents distance  
                //from its neighbor.
                Steering += ToAgent.normalized / ToAgent.magnitude;
            }
        }

        SteeringForce += Steering * separationWeight;
    }

    void TeamAlignment()
    {
        //used to record the average heading of the neighbors
        Vector3 AverageHeading = Vector3.zero;

        if (NearTeammates.Count > 0)
        {
            //iterate through all the tagged vehicles and sum their heading vectors  
            foreach (GameObject neighbor in NearTeammates)
            {
                //make sure it doesn't include any evade target ***
                //if ((neighbors[a] != m_pTargetAgent1))
                {
                    AverageHeading += neighbor.transform.forward;
                }
            }

            //if the neighborhood contained one or more vehicles, average their
            //heading vectors.


            AverageHeading /= NearTeammates.Count;

            AverageHeading -= transform.forward;

        }
        SteeringForce += AverageHeading * alignmentWeight;
    }

    private Player player;
    void Start()
    {
        // Soy pelota o jugador
        player = GetComponent<Player>();

        rb = GetComponent<Rigidbody>();

        NearPlayers = new List<GameObject>();
        NearTeammates = new List<GameObject>();
        NearRivals = new List<GameObject>();
        NearPlayersSensor = transform.Find("Player Sensor").GetComponent<SphereCollider>();

        obstacleDetector = transform.Find("Obstacle Detector").GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ajustar el tamaño del sensor de vecinos
        NearPlayersSensor.radius = nearPlayersSensorRadius;


        // Calculamos la fuerza de control "steering"
        Calculate();

        // Aceleracion = Fuerza / masa
        Vector3 acceleration = SteeringForce / rb.mass;

        // Actualizamos la velocidad
        Velocity += acceleration * Time.deltaTime;

        // Que el agente no supere la velocidad maxima
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        // Actualizo la posicion
        //Vector3 newVelocity = new Vector3(Velocity.x, Velocity.y, 0f);
        rb.velocity = Velocity;

        // Girar al agente si lleva una velocidad significativa
        if (rb.velocity.magnitude > 0.01f)
        {
            transform.LookAt(transform.position + Velocity);
        }

    }

    void OnTriggerEnter(Collider colisionador)
    {
        // Si es un jugador lo agrego a mi lista 
        if (GameManager.instancia.team1Players.Contains(colisionador.transform) ||
            GameManager.instancia.team2Players.Contains(colisionador.transform))
        {
            if (!NearPlayers.Contains(colisionador.gameObject))
            {
                NearPlayers.Add(colisionador.gameObject);
            }
            // soy una pelota o un jugador
            if (player != null)
            {// Si es un compañero
                if (player.myTeam.isTeammate(colisionador.gameObject) && !NearTeammates.Contains(colisionador.gameObject))
                    NearTeammates.Add(colisionador.gameObject);

                // Si es un rival
                else if (!NearRivals.Contains(colisionador.gameObject))
                    NearRivals.Add(colisionador.gameObject);
            }
        }

    }

    void OnTriggerExit(Collider col)
    {
        // Deja de estar en mi rango de sensor de vecinos
        if (NearPlayers.Contains(col.gameObject))
            NearPlayers.Remove(col.gameObject);

        if (player != null)
        {
            if (NearTeammates.Contains(col.gameObject))
                NearTeammates.Remove(col.gameObject);

            if (NearRivals.Contains(col.gameObject))
                NearRivals.Remove(col.gameObject);
        }
    }
}
