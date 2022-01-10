# [if21b163] Monster Trading Card Game

## Design
Ich habe mich dafür entschieden die gesamte Applikation von Anfang an synchron umzusetzen und danach erst mittels Threading nebenläufig zu machen. Dies hat den initialen Programmieraufwand relativ gering gehalten und ich bin zufrieden mit dem Ansatz.

Für die Datenbank Connection (siehe Lessons Learned) habe ich einen Singleton verwendet, damit von überall in der Applikation auf die DB zugegriffen werden könnte. Da das allerdings ziemlich unleserlich ist, finden alle Datenbankzugriffe im jeweiligen Repository statt - ich habe nämlich das Repository Pattern implementiert. So könnte die Datenbank gegebenenfalls recht einfach ausgetauscht werden und die Businesslogik ist davon nicht betroffen.

Das Battle ist ebenfalls mit einem Singleton umgesetzt, da dieses über mehrere Threads verfügbar sein muss.
Der erste Spieler joined dem Battle und "polled" so lange, bis das Battle beendet wurde und die Ergebnisse verfügbar sind.
Der zweite Spieler hingegen startet nach dem Joinen das Battle und kriegt die Ergebnisse direkt als `return` Value.
Nachdem der erste Spieler das Ergebnis des Battles gepolled hat, setzt er dieses zurück damit ein neues Battle stattfinden kann. Dieser Approach ist für einen großen Server nicht geeignet, da immer nur ein Battle gleichzeitig stattfinden kann - für die Anforderungen dieses Projekts fand ich die Herangehensweise aber ganz passend.

Bezüglich OOP, der Benutzung von Interfaces, etc. bin ich aus zeitlichen Gründen leider nicht mehr dazu gekommen das Projekt zu refactoren. Die Codebase ist daher etwas messy. Das würde ich auf jeden Fall noch beheben um eine bessere Wartbarkeit / Lesbarkeit zu erzeugen.

## Lessons Learned
Ich habe bei der Implementation gelernt, dass eine Postgres `NpgsqlConnection` nur ein lesendes Kommando auf einmal ausführen kann. Wenn man also den Reader nicht mittels `Reader.Close()` schließt, kann keine zweite lesende Operation ausgeführt werden. Dies hat sich als mühsam herausgestellt, da ich die Datenbank Connection als Singelton verwende. Sobald mir das Problem bewusst war, konnte ich dieses ohne große Mühe umgehen.

Außerdem habe ich mir im Zuge dieses Projektes die "Postman" Applikation näher angesehen und bin ein großer Fan geworden. Ich habe Postman für die Integration Tests benutzt und eigene Testcases für jeden Request gechrieben. Dies hat das Testen schnell und einfach gemacht.


## Unit Testing
Bei den Unit Tests mit NUnit habe ich darauf geachtet alle Sonderfälle der Gamelogik abzudecken. Der für das Gameplay am wichtigste Wert ist die Damage einer Karte im Bezug auf ihr Gegenüber. Deswegen fokusieren sich die Unit Tests vor allem auf die besagte Berechnung. Mit dem `[TestCase(...params)]` Attribut konnte ich schnell und einfach alle möglichen Varianten abdecken ohne manuell sehr viele Tests schreiben zu müssen.

## Unique Feature
Mein uniques Feature besteht darin, dass Karten mit einer geringen Wahrscheinlichkeit "shiny" sein können. Dies wird beim erstmaligen Anlegen einer Karte festgelegt. Momentan liegt die Wahrscheinlichkeit, dass die Karte shiny ist bei 10%, dieser Wert würde in einem echten Spiel allerdings niedriger angesetzt werden. Wenn eine shiny Karte im Spiel ausgespielt wird, so ist ein extra Eintrag im Log zu finden und der "Damage" Wert der Karte wird mit 1.2 multipliziert. Dies bietet einen Vorteil im Spiel und erhöht die Hoffnung darauf, eine shiny Karte zu erhalten.

## Time Tracking
Ich habe bei diesem Projekt ziemlich präzises Timetracking mit "Toggl Track" betrieben und bin auf folgenden Aufwand gekommen:

> 17 Stunden 56 Minuten

## Code Repository
Das Code Repository meiner Abgabe finden Sie [hier](https://github.com/maxwoedl/sem3-mtcg).
