# Užduotis

### Atlikta pagal žemiau nurodytus punktus:
1.  Formoje vartotojas gali nurodyti kintamą kiekį thread (atšakos) (Nuo 2 iki 15, kiekis negali būti didesnis ar mažesnis).
2.  Išrinkus kiekį ir paspaudus start mygtuką, kiekvienas thread turi atsitiktiniu laiko intervalu (0,5-2 sekundžių) generuoti 5-10 (atsitiktinai) simbolių ilgio eilutę.
3.  Turi būti įsimenamos/rodomos 20 paskutinių sugeneruotų duomenų, kurie išvedami į formos ListView kontrolą. ListView turi tokias kolonas – Thread ID(numeris atšakos, kur numeruojama nuo 1), sugeneruota eilutė.
4.  Visi atšakos generuoti duomenis rašomi į access duomenų failą(mdb) (galite naudoti ir SQL serverį, jie turite tokią galimybę), lentelę - kur laukai yra ID(autonumber), ThreadID, Time(laikas sugeneravimo), Data(eilutė).
5.  Paspaudus Stop mygtuką darbas stabdomas ir thread išjungiamos.
