using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Codigo de un GANCHO para un videojuego 2D creado en Unity. El jugador pulsa una tecla mirando en una dirección y se dispara un gancho.
// Al impactar, el gancho transporta al jugador hacia donde ha colisionado.
public class scr_hook : MonoBehaviour
{
    //SerializeField extiende las variables al editor. Por defecto son privadas.
    [Header("Prefab, spawners y velocidad del gancho")]
    [SerializeField] GameObject hookObject; //GameObject del proyectil del gancho.
    [SerializeField] Transform HookPositionTop, HookPositionForward;    //Puntos del jugador desde los cuales puede salir el gancho.
    [SerializeField] float hookSpeed; //Velocidad del gancho.

    [Header("Velocidad del jugador y rango")]
    [SerializeField] float movementSpeed; //Velocidad del jugador cuando lo mueve el gancho.
    [SerializeField] float hookRange;   //Ranga hasta donde llega el gancho.

    [Header("Camera")]
    [SerializeField] float freezeTime; //Tiempo del efecto de la camara.
    [SerializeField] float shakeMagnitude;  //Magnitud del efecto shake de la camara.

    bool canHook, moveToHook, moveBoxToHook; //Variables que controlan si puede disparar el gancho, si se esta moviendo hacia el gancho, o si una caja esta siendo movida hacia el jugador.
    GameObject hookInstance; //Instancia del gancho.
    scr_camera cameraScript; //Referencia al script de la camara para usar efectos.
    Vector3 hookDestinationTarget;  //Posicion en la que impacta el gancho.
    Rigidbody2D rb; //Componenete RigidBody del jugador, para fisicas.
    float gameGravityScale; //Gravedad del jugador.
    GameObject boxToMove; //Referencia de la caja que ha impactado el gancho.
    Animator anim;  //Referencia del componente Animator, para animaciones.

    //Unity ejecuta Awake cuando el objeto se instancia. Es buen momento para coger referencias.
    private void Awake()
    {
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<scr_camera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    //Start se ejecuta tan pronto se inicia la escena, aqui ponemos variables por defecto.
    void Start()
    {
        gameGravityScale = rb.gravityScale;
        canHook = true;
    }

    //Unity ejecuta Update una vez cada frame. Sirve para movimiento independiente de la velocidad de frames.
    void Update()
    {
        //Se comprueba si en el inventario está el objeto que permite usar el gancho.
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_inventorysystem>().GetItem("ii_hook") != null)
        {
            //Si es posible usar el gancho, y el jugador aprieta C, se dispara el gancho.
            if (canHook && Input.GetKeyDown(KeyCode.C))
            {
                ShootHook();
            }

            //Si el gancho esta en la escena, y se sale del rango, se para de disparar.
            if (hookInstance != null)
            {
                if (Vector2.Distance(transform.position, hookInstance.transform.position) > hookRange)
                {
                    StopHook();
                }
            }
        }
    }

    //Unity ejecuta esto al final de cada frame, teniendo en cuenta el tiempo entre el ultimo frame (DeltaTime).
    //Aquí es buen lugar para colocar la parte física.
    private void FixedUpdate()
    {
        //Si se mueve hacia el gancho, se interpola la posicion del jugador hacia la del objetivo, con un efecto de suavizado.
        if (moveToHook)
        {
            //MoveTowards interpola 2 vectores con suavidaad.
            transform.position = Vector3.MoveTowards(transform.position, hookDestinationTarget, movementSpeed / 100);

            //Cuando se llegue al destino, se detiene el gancho.
            if (Vector3.Distance(transform.position, hookDestinationTarget) < 0.1f)
            {
                StopHook();
            }
        }

        //Si la caja se mueve hacia el jugador, ocurre lo mismo con moveToHook pero a la inversa.
        if (moveBoxToHook)
        {
            boxToMove.transform.position = Vector3.MoveTowards(boxToMove.transform.position, transform.position, movementSpeed / 100);

            if (Vector3.Distance(boxToMove.transform.position, transform.position) < 0.1f)
            {
                StopHook();
            }
        }
    }

    //Dispara el gancho.
    private void ShootHook()
    {
        //Convierte al jugador en estatico y le quita la posibildad de lanzar el gancho.
        rb.bodyType = RigidbodyType2D.Static;
        canHook = false;
        rb.gravityScale = 0;

        //Guardaremos aqui hacia donde lanza el gancho.
        Transform targetPosition = HookPositionForward;

        //Dependiendo de los ejes del teclado o mando, (arriba o abajo) guardamos hacia donde irá el gancho.
        //Tambien disparamos los triggers del animador.
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            targetPosition = HookPositionTop;
            anim.SetTrigger("Hook_Up");
        }
        else
        {
            anim.SetTrigger("Hook_Forward");
        }

        //Instanciamos el gancho, lo guardamos y le damos velocidad.
        hookInstance = Instantiate(hookObject, targetPosition.position, targetPosition.rotation);
        hookInstance.GetComponent<scr_hookcollider>().speed = hookSpeed;
    }

    //Detiene el gancho.
    void StopHook()
    {
        //Establece tods las variables a su estado de antes de lanzar el gancho.
        rb.bodyType = RigidbodyType2D.Dynamic;
        moveToHook = false;
        moveBoxToHook = false;
        canHook = true;
        rb.gravityScale = gameGravityScale;
        //Eliminamos la instancia del gancho.
        Destroy(hookInstance);
    }

    //Esta funci�n es llamada desde el HookCollider (proyectil del gancho) para pasarle el resultado del gancho y el objeto que haya tocado.
    public void OnHookCollide(Vector2 hookDestination, string result, Collision2D collision)
    {
        switch (result)
        {
            case "HookableWall":
                StartCoroutine("OnHookSuccess", hookDestination);
                break;

            case "Box":
                boxToMove = collision.gameObject;
                cameraScript.CameraShake(freezeTime, shakeMagnitude);
                moveBoxToHook = true;
                break;

            case "Failed":
                StopHook();
                break;
        }
    }

    //Si el gancho impacta, se genera el efecto de la camara y se inicia el movimiento.
    IEnumerator OnHookSuccess(Vector2 hookDestination)
    {
        cameraScript.CameraShake(freezeTime, shakeMagnitude);
        yield return new WaitForSeconds(freezeTime);
        hookDestinationTarget = new Vector3(hookDestination.x, hookDestination.y, transform.position.z);
        moveToHook = true;
    }

    //Debug. Unity dibuja gizmos.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        //Rango del gancho.
        Gizmos.DrawWireSphere(transform.position, hookRange);
    }
}
