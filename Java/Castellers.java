import java.util.Scanner;

public class Castellers {

    public static void main(String[] args) {
        //Escàner i programa.
        Scanner teclat = new Scanner(System.in);
        Castellers pgm = new Castellers();
        
        //Dades
        //Tipus de Castells
        Castell[] arrayCastell = {new Castell(1, "Quatre de set", 800, 980),
            new Castell(27, "Tres de deu amb folre i manilles", 12500, 13200),
            new Castell(3, "Quatre de set amb l'agulla", 1220, 1440),
            new Castell(22, "Quatre de nou amb folre i l'agulla", 9200, 9800),
            new Castell(2, "Tres de set", 1000, 1200),
            new Castell(21, "Cinc de nou amb folre", 8600, 9180),
            new Castell(20, "Pilar de vuit amb folre i manilles", 8020, 8580)};
        
        //Codis de castells
        int[] codis = {21, 3, 20};
        int[] codis2 = {2, 1, 3};
        int[] codis3 = {22, 20, 21};
        int[] codis4 = {27, 22, 20};
        
        //Colles castelleres
        Colla[] arrayColla = {new Colla("Colla Vella dels Xiquets de Valls", "Valls", codis),
            new Colla("Xiquets del Serrallo", "Tarragona", codis2),
            new Colla("Minyons de Terrassa", "Terrassa", codis3),
            new Colla("Xiquets de Vilafranca", "Vilafranca", codis4)};
        
        //Mètodes
        //Colla castellera i els seus castells.
        //El catch sortirá amb un error si es produeix una excepció.
        try{
            System.out.print("Introdueix el nom de la colla: ");
            pgm.dadesColla(teclat.nextLine(), arrayColla, arrayCastell);
        }catch(Exception e){
            System.out.println("ERROR: El nom de la colla no existeix a les dades. (JAVA: " + e.getMessage() + ")");
        }
        //Castell i colles castelleres que el fan.
        try{
            System.out.println("");
            System.out.print("Introdueix el nom del castell: ");
            pgm.dadesCastell(teclat.nextLine(), arrayCastell, arrayColla);
        }catch (Exception e){
            System.out.println("ERROR: El nom del castell no existeix a les dades. (JAVA: " + e.getMessage() + ")");
        }
        
    }
    
    private void dadesColla(String nomColla, Colla[] arrayColla, Castell[] arrayCastell){
        //Buscar la colla que coincideixi amb el nom passat per paràmetre i imprimir-la.
        Colla colla = arrayColla[buscarColla(nomColla, arrayColla)];
        System.out.println("Colla: " + colla.getNom());
        
        //Recòrrer l'array de codis de la colla.
        for (int i = 0; i < colla.getCodisCastellsPrincipals().length; i++){
            //Per cada codi de la colla, imprimir el castell que tingui aquest codi.
            Castell castell = arrayCastell[buscarCastellPerCodi(colla.getCodisCastellsPrincipals()[i], arrayCastell)];
            System.out.println("Castell: " + castell.getDescripció());
        }
    }
    
    private void dadesCastell(String nomCastell, Castell[] arrayCastell, Colla[] arrayColla){
        //Buscar el castell que coincideixi amb el nom passat per paràmetre i imprimir-lo.
        Castell castell = arrayCastell[buscarCastellPerNom(nomCastell, arrayCastell)];
        System.out.println("Castell: " + castell.getDescripció());
        
        //Recòrrer l'array de colles i imprimir les que tinguin el codi del castell.
        for (int i = 0; i < arrayColla.length; i++){
            Colla colla = arrayColla[i];
            //Per cada colla, recòrrer els seus castells principals.
            for (int j = 0; j < colla.getCodisCastellsPrincipals().length; j++){
                //Per cadascun dels castells principals, imprimir el nom de la colla si aquesta té el castell seleccionat.
                int[] codisCastellsPrincipals = colla.getCodisCastellsPrincipals();
                if (codisCastellsPrincipals[j] == castell.getCodi()){
                    System.out.println("Colla: " + colla.getNom());
                }
            }
        }
    }
    
    //Busca la colla amb el nom.
    private int buscarColla(String nomColla, Colla[] arrayColla){
        int i = 0;
        boolean trobat = false;
        while (i < arrayColla.length && !trobat){
            if (arrayColla[i].getNom().equalsIgnoreCase(nomColla)){
                trobat = true;
            }else{
                i++;
            }
        }
        return i;
    } 
    
    //Busca el castell per el codi.
    private int buscarCastellPerCodi(int codiCastell, Castell[] arrayCastell){
        int i = 0;
        boolean trobat = false;
        while (i < arrayCastell.length && !trobat){
            if (arrayCastell[i].getCodi() == codiCastell){
                trobat = true;
            }else{
                i++;
            }
        }
        return i;
    }
    
    //Busca el castell per el seu nom.
    private int buscarCastellPerNom(String nomCastell, Castell[] arrayCastell){
        int i = 0;
        boolean trobat = false;
        while (i < arrayCastell.length && !trobat){
            if (arrayCastell[i].getDescripció().equalsIgnoreCase(nomCastell)){
                trobat = true;
            }else{
                i++;
            }
        }
        return i;
    }
}
