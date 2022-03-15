using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_windZone : MonoBehaviour
{
    //Variables
    //Tiempo entre aplicaci�n de la fuerza.
    [SerializeField] float m_fuerzaTick;
    //Magnitud de la fuerza que se aplica.
    [SerializeField] float m_fuerzaMagnitud;
    //Cuanto dura el viento activo, en segundos.
    [SerializeField] int m_segundosActivo;
    //Cuanto dura el viento desactivado, en segundos.
    [SerializeField] int m_segundosInactivo;

    //Guardamos el siguiente tick en esta variable.
    float m_nextTick;
    bool m_activado;
    SpriteRenderer m_spriteRenderer;
    GameObject box;

    //Al instanciarse, recoge el SpriteRenderer y lo guarda en la variable.
    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Al empezar, activar� la corutina donde activa-desactiva el viento.
    private void Start()
    {
        StartCoroutine(ActivarDesactivarViento());
        m_activado = false; 
    }

    //Empujamos la caja, si est� dentro de la zona.
    private void Update()
    {
        if (box != null && m_activado)
        {
            Debug.Log("Se est� detectando la caja.");
            Vector3 nuevaPosicion = new Vector3(transform.position.x, box.gameObject.transform.position.y, box.gameObject.transform.position.z);
            box.transform.position = nuevaPosicion;
            Empujar(box);
        }
    }

    //Detectamos que ha entrado la caja y la guardamos.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            box = collision.gameObject;
        }
    }

    //Detectamos que la caja ha salido y la eliminamos.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            box = null;
        }
    }

    //TriggerStay para aplicar la fuerza todo el tiempo que el objeto est� dentro de la zona.
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Todo el codigo se ejecutar� solamente si el viento est� activado.
        if (m_activado)
        {
            Debug.Log("ACTIVADO");
            //Comprobamos que el objeto de dentro de la zona sea el jugador o una caja.
            if (collision.CompareTag("Player"))
            {
                Empujar(collision.gameObject);
            }
        }
    }

    private void Empujar(GameObject collision)
    {
        //Le aplicamos una fuerza (fuerzaMagnitud) cada vez que pase fuerzaTick segundos al objeto que entre dentro del Trigger.
        if (Time.time > m_nextTick)
        {
            float distanceToTop = (GetComponent<BoxCollider2D>().bounds.max.y + 1) - collision.transform.position.y;
            m_nextTick = Time.time + m_fuerzaTick;
            Vector2 l_fuerza = (transform.up * m_fuerzaMagnitud) * distanceToTop;
            collision.GetComponent<Rigidbody2D>().AddForce(l_fuerza);
        }
    }

    //Activa o desactiva el viento, esperando los segundos indicados y ajustar� el color para que se vea si esta activado o no.
    IEnumerator ActivarDesactivarViento()
    {
        yield return new WaitForSeconds(m_segundosInactivo);
        Debug.Log("Activando");
        m_activado = true;
        m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, 1);
        yield return new WaitForSeconds(m_segundosActivo);
        m_activado = false;
        Debug.Log("Desactivando");
        m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, 0.1f);
        StartCoroutine(ActivarDesactivarViento());
    }

    private void OnDrawGizmos()
    {
        //Gizmo para indicar hacia donde ir� el viento.
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 5);
    }
}
