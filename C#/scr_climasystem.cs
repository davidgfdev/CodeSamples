using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Es un sistema de clima para un videojuego basado en Unity en el cual puede ser de dia, de noche, puede haber lluvia o bien tormenta. 
//Va cambiando según parametros de tiempo extendidos al editor.

public class scr_climasystem : MonoBehaviour
{
    //Enum para diferentes tipos de horario.
    public enum daytimes
    {
        day, 
        night //Durante la noche, la iluminacion de la escena bajará y será dificil ver.
    }

    //Enum para diferentes climas.
    public enum clima
    {
        clear, //Soleado, sin ningún efecto en el jugador.
        rain, //La lluvia provoca que el jugador se resbale.
        storm   //La tormenta genera rayos que si impactan al jugador, le harán daño.
    }

    //Las varialbes con la etiqueta SerializeField se extienden al editor de Unity. Por defecto, las variables son privadas.
    [Header("Clima  y horario")]
    [SerializeField] float dayTimeChange; //Cada cuanto tiempo cambia el momento del dia.
    [SerializeField] float climaTimeChange; //Cada cuanto tiempo cambia el clima.
    [SerializeField] bool cycleActive;  //Si el ciclo está activo.

    [Header("Tormenta")]
    [SerializeField] float lightningRate;   //Cada cuanto tiempo cae un rayo.
    [SerializeField] float lightningRange;  //El rango horizontal por donde es posible que caiga un rayo respecto la posicion del jugador.
    [SerializeField] GameObject lightningStrike;    //GameObject del rayo.

    [Header("Gráficos")]
    [SerializeField] Color nightColor;  //Color que adoptará el cielo cuando es de noche.
    [SerializeField] Color dayColor;    //Color que adoptará el cielo cuando es de dia.
    [SerializeField] ParticleSystem rain;   //Referencia al sistema de particulas de la lluvia.

    daytimes currentDayTime = daytimes.day; //Control del momento del dia.
    clima currentClima = clima.clear;   //Control del clima.
    bool storm; //Control de la tormenta, para saber si está activada o no.

    //Getters
    public clima getCurrentClima()
    {
        return currentClima;
    }

    public daytimes getCurrentDayTime()
    {
        return currentDayTime;
    }

    //Método de inicio. Unity lo ejecuta cuando el GameObject al que está adjunto este script aparece en la escena.
    private void Start()
    {
        //Se inician las corutinas que cambian los momentos del dia y el clima.
        StartCoroutine(DayCycle());
        StartCoroutine(ClimaCycle());
        //UpdateClima es llamado para asegurarnos de que el clima se asigna al principio, ya que muchas veces cargaremos la partida al principio.
        UpdateClima();
    }

    //Dependiendo del Enum seleccionado, provoca los efectos de dia y noche.
    public void UpdateDay()
    {
        switch (currentDayTime)
        {
            case daytimes.day:
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = dayColor;
                break;
            case daytimes.night:
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = nightColor;
                break;
        }
    }

    //Dependiendo del clima, provoca los efectos del que toque.
    //Si es lluvia o tormenta, activará las particulas de lluvia.
    //En el caso de tormenta, activará la corutina de tormenta y la variable será true.
    public void UpdateClima()
    {
        switch (currentClima)
        {
            case clima.clear:
                rain.Stop();
                storm = false;
                break;
            case clima.rain:
                rain.Play();
                storm = false;
                break;
            case clima.storm:
                Debug.Log("Storm");
                rain.Play();
                storm = true;
                StartCoroutine(StormThunders());
                break;
        }
    }

    //Este metodo sirve para cambiar el momento del dia desde otros objetos.
    public void ChangeDayTime(daytimes newDaytime)
    {
        currentDayTime = newDaytime;
        UpdateDay();
    }
    
    //Cambiar el clima desde otros objetos.
    public void ChangeClima(clima newClima)
    {
        currentClima = newClima;
        UpdateClima();
    }

    //Espera el tiempo del horario, y despues cambia el momento del dia.
    //Si el ciclo continua, se llama a si misma.
    IEnumerator DayCycle()
    {
        if (currentDayTime == daytimes.day)
        {
            ChangeDayTime(daytimes.night);
        }
        else
        {
            ChangeDayTime(daytimes.day);
        }
        yield return new WaitForSeconds(dayTimeChange);

        if (cycleActive)
        {
            StartCoroutine(DayCycle());
        }
    }

    //Establece el clima de manera aleatoria.
    //Si el ciclo esta activo, se llama a si misma.
    IEnumerator ClimaCycle()
    {
        int dice = Random.Range(0, 11);
        if (dice >= 0 && dice < 8)
        {
            ChangeClima(clima.clear);
        }
        if (dice >= 8 && dice < 10)
        {
            ChangeClima(clima.rain);
        }
        if (dice == 10)
        {
            ChangeClima(clima.storm);
        }
        yield return new WaitForSeconds(climaTimeChange);
        if (cycleActive)
        {
            StartCoroutine(ClimaCycle());
        }
    }

    //Instancia rayos mientras la tormenta esté activa de manera aleatoria por una zona.
    IEnumerator StormThunders()
    {
        while (storm)
        {
            //Mientras la tormenta esté activa, calcula la zona por la que puede caer un rayo.
            //La zona es inversamente proporcional a la velocidad del jugador. Si el jugador va muy rapido, mas pequeña sera la zona por la que cae el rayo.
            float calculatedRange = lightningRange / GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().velocity.sqrMagnitude;
            //Se calcula que no pase de ciertos limites.
            calculatedRange = Mathf.Clamp(calculatedRange, 7, lightningRange);

            //Se calcula la posición donde caerá el rayo y se guarda en un Vector3.
            Vector3 lightningPos = calcStrikePosition(calculatedRange);
            //Instanciamos el rayo.
            Instantiate(lightningStrike, lightningPos, lightningStrike.transform.rotation);
            //Espera el tiempo entre rayos.
            yield return new WaitForSeconds(lightningRate);
        }
    }

    //Este metodo, dado un rango, calcula la posicion aleatoria por donde puede caer un rayo respecto al jugador.
    Vector2 calcStrikePosition(float range)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform; //Posicion del jugador.
        float minX = player.position.x - range;
        float maxX = player.position.x + range;

        //Se calcula una posicion horizontal aleatoria.
        float XPos = Random.Range(minX, maxX);
        //Se comprueba si hay SUELO con un rayo que va desde la camara hacia abajo. Si el rayo impacta con un COLLIDER de Unity, devuelve un hit.
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(XPos, player.position.y + lightningRange), Vector2.down);

        //Establecemos la posicion del rayo como las coordenadas del hit.
        Vector2 finalPos = new Vector2(hit.point.x, hit.point.y);

        return finalPos;
    }

    //Activa los 2 ciclos.
    public void ActiveCycle()
    {
        cycleActive = true;
        StartCoroutine(DayCycle());
        StartCoroutine(ClimaCycle());
    }

    //DEBUG. Metodo de Unity que resuelve los gizmos.
    private void OnDrawGizmos()
    {   //Dibuja una esfera en el rango de rayos.
        Gizmos.DrawWireSphere(GameObject.FindGameObjectWithTag("Player").transform.position, lightningRange);
    }
}
