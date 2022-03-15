import java.util.Scanner;

public class Agenda {

    public static void main(String[] args) {
        //Referències
        Scanner scn = new Scanner(System.in);
        Agenda pgm = new Agenda();
        //Llista de contactes
        Persona[] llista = new Persona[10];
        //Aquesta variable seguirá el nom de persones registrades a la llista, per recòrrer els arrays sense perill de trobar un valor nul.
        int numPersones = 0;

        //While que sosté el menú del programa. Abans del bucle, es demanará la primera opció de menú escollida per iniciar el programa.
        int opcioMenu = pgm.Menu(scn);
        while (opcioMenu != 5) {
            switch (opcioMenu) {
                //Introducció seqüencial de contactes.
                case 1:
                        numPersones = pgm.AfegirContacte(scn, llista, numPersones);
                    
                    break;
                case 2:
                    System.out.print("Introduïr el cognom: ");
                    String cognom = scn.next();
                    pgm.BuscarContacte(llista, cognom, numPersones);
                    break;
                case 3:
                    //S'introdueix el nom de la persona a esborrar.
                    System.out.println("Introdueix el nom de la persona a esborrar: ");
                    String nom = scn.next();
                    numPersones = pgm.EsborrarContacte(nom, llista, numPersones);
                    break;
                case 4:
                    pgm.ImprimirLlista(llista, numPersones);
                    break;
            }
            //Tornem a preguntar al final del bucle quina opció es vol escollir per si es surt del bucle o si es segueix, executar la funció escollida.
            opcioMenu = pgm.Menu(scn);
        }
    }

    private int Menu(Scanner scn) {
        //Guardarem la opció introduïda en una variable d'enter que retornarem al programa al final del mètode.
        int opcioEscollida;
        //Totes les opcions.
        System.out.println("\nMenú Agenda");
        System.out.println("1. Introduïr contactes.");
        System.out.println("2. Buscar contacte per cognom.");
        System.out.println("3. Esborrar contacte.");
        System.out.println("4. Mostrar tots els contactes");
        System.out.println("5. Sortir.");
        System.out.print("Introdueix una opcio: ");
        opcioEscollida = scn.nextInt();
        //Espai en blanc per separar.
        System.out.println("");
        //Es retorna l'opció escollida.
        return opcioEscollida;
    }

    private int AfegirContacte(Scanner scn, Persona[] llista, int numPersones) {
        System.out.println("\n\nIntroduïnt contacte " + numPersones);
        //Introduccio de nom.
        System.out.print("\nNom: ");
        String nom = scn.next();
        //Introduccio de cognom.
        System.out.print("\nCognom: ");
        String cognom = scn.next();
        //Introduccio de numero de telefon.
        System.out.print("\nTeléfon: ");
        int telefon = scn.nextInt();

        //Es crea una nova persona amb les dades introduïdes i s'afegeix a l'array en la posició del paràmetre.
        Persona personaNova = new Persona(nom, cognom, telefon);
        llista[numPersones] = personaNova;
        numPersones++;
        return numPersones;
    }

    private int EsborrarContacte(String nom, Persona[] llista, int numPersones) {
        //Busquem la persona a esborrar.
        boolean trobada = false;
        int i = 0;
        //Bucle que o bé acaba quan es recorre tota la llista o bé acaba quan es troba la persona.
        while (i < numPersones - 1 && !trobada) {
            if (llista[i].getNom().equalsIgnoreCase(nom)) {
                trobada = true;
            } else {
                //El comptador nomès augmentarà si no s'ha trobat la persona.
                i++;
            }
        }

        //Un cop trobada, l'esborrem i pujem els elements restants una posició per evitar valors nuls.
        //Comprobem que s'ha trobat correctament
        if (trobada) {
            for (int j = i; j < numPersones-1; j++) {
                    llista[j] = llista[j + 1];
            }
            numPersones--;
        } else {
            System.out.println("ERROR: El contacte a esborrar no s'ha trobat.");
        }
        return numPersones;
    }

    private void BuscarContacte(Persona[] llista, String cognom, int numPersones) {
        boolean trobada = false;
        int i = 0;
        //Bucle que o bé acaba quan es recorre tota la llista o bé acaba quan es troba la persona.
        while (i < numPersones && !trobada) {
            if (llista[i].getCognom().equalsIgnoreCase(cognom)) {
                trobada = true;
            } else {
                //El comptador nomès augmentarà si no s'ha trobat la persona.
                i++;
            }
        }

        //Si s'ha trobat la persona, s'imprimeix el resultat, si no, s'imprimeix un error.
        if (trobada) {
            System.out.println("Nom: " + llista[i].getNom());
            System.out.println("Telèfon: " + llista[i].getTelefon() + "\n");
        } else {
            System.out.println("ERROR: Persona no trobada.");
        }
    }

    private void ImprimirLlista(Persona[] llista, int numPersones) {
        System.out.println("-Llista-");
        //Es recorre l'array i es mostra nom, cognom i telèfon de cada element.
        for (int i = 0; i < numPersones; i++) {
            System.out.println(i + " Nom: " + llista[i].getNom());
            System.out.println("Cognom: " + llista[i].getCognom());
            System.out.println("Telèfon: " + llista[i].getTelefon());
        }
    }
}