todo:
* tilføj claims
* tilføj rate limiting middleware
* tilføj session invalidated middleware
* tilføj claims middleware

* Ændrer det til:
  * Create guest access with claims.

* Sessions lagret var en dårlig idé iht. sikkerhed. Fordi så har man adgang til alle sessions via databasen.
  * Alterntivt så hashede jeg sessions og matchede op derefter.

BASE TASK:

* ~~Set up swagger~~
* Endpoints:
* Fix readme.md

EXTRA TASK:
* Preventing brute force of passwords.
  * Rate limit login attempts (token/session creation) 
  * Ideally I would a loadbalancer add extra information such as IP address to the request and then block bad actors.
    * But this should maybe also be the responsibility of the loadbalancer and not the API itself.
    * If a user has too many attempts on creation of Guest-Session. Then lock the user for guest session creation until the bad actor has been identified and blocked.
      Yes this will lead to someone being a bit unhappy - but again the blocking of the IP-Address should be something handled by the Load balancer.
* Add a background job to set passwords/sessions as expired
* Setting up rate limits. Store it in redis.

EXTRA TASK:
* How to handle test, dev, prod?
  * Use AppSettings => Check environment variable and generate IConfiguration from some repository function 
  * (Production could retrive some key from azure key vault and then retrive the configuration from a Repository).
  

EXTRA MILE:
* Create a gateway API (making rate limiting better/easier if more APIs are )
* Create a HAProxy/nginx/traefik loadbalancer to make a PoC of blocking bad actors.



Opgave

Design et simpelt system til engangskoder for adgang

Du arbejder på et produkt, hvor man ønsker at give midlertidig adgang til en bestemt ressource via en 6-cifret engangskode. 
Det skal fungere nogenlunde som en gæsteadgang – f.eks. til at hente en fil, tilgå en support-session eller lignende. 
Hvad selve koden beskytter er ikke relevant – men arbejdet med koderne er.

Du bedes overveje, design og implementere en løsning (eller dele af en løsning) som kan løse denne opgave.

Forslag til funktionalitet:

Generering af adgangskode

    En bruger skal kunne anmode om en 6-cifret kode, som er unik og gyldig i f.eks. 10 minutter.
    Koden skal være nem at indtaste manuelt.

Validering af kode

Når nogen forsøger at bruge en kode, skal systemet kunne afgøre, om den er:

    gyldig (ikke udløbet),
    knyttet til en gyldig session/bruger/ressource,
    og ikke allerede brugt (engangskoder).

Skalerbarhed og sikkerhed

    Overvej hvordan systemet håndterer høj belastning (f.eks. 10.000 samtidige brugere).
    Undgå brute-force angreb på koderne.
    Det må gerne være muligt at sætte "rate limits".

Audit og udløb

    Systemet skal kunne logge anvendelser og rydde op i udløbne koder.

Andet

    Hvordan ville du gøre det muligt at gemme audit logs i længere tid (f.eks. i Azure)?
    Hvordan kunne man udvide det til også at virke offline i en begrænset periode?
    Hvordan håndterer du forskellige miljøer (dev/test/prod) og hemmeligheder?

Før samtalen bedes du sende et link til et repository hvor opgaven kan hentes ned sammen med evt. instruktioner til at køre løsningen.