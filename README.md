# 223-ma-app

Willkommen beim Ük 223. Dieses Repository enthält den Code für die Applikation, die wir im Modul 223 erstellen werden, sowie die Lerninhalte. 

Es dient jedoch lediglich als Ablage dieser Informationen. Gewisse Informationen befinden sich auch im Learningview, ebenfalls wird wie gewohnt im Learningview die Dokumentation der Arbeit abgegeben, sowie der Progress dokumentiert.

## Setup

Kopiere `.env.example` in `.env` und passe die Werte an.  
Danach kannst du die Datenbank starten mit `docker compose up -d`.

Beachte auch die READMEs in den einzelnen Projekten beim Setup der Umgebung.

## Dokumentation
Die Herausforderung für mich war es herauszufinden, wie mit den asynchronen Calls umzugehen ist. Beispielsweise habe ich im Repository asynchrone Methoden verwendet und diese auch “awaited”. Dabei vergass ich, im Controller, also direkt an der Quelle, auf die ganze Methode zu warten. Dies konnte ich dadurch lösen, indem ich im Controller die Delete Methode awaited habe.
Weiter habe ich zuerst nicht bemerkt, dass die SaveChangesAsync() Methode die Transaktion nicht beendet. Wenn die Transaktion nicht beendet wird, werden die Änderungen auch nicht umgesetzt. Also habe ich nach der Save Methode auch noch die Transaktion committed und somit das Problem gelöst.
Das Resultat dieser beiden Änderungen war eine funktionierende Delete Methode.
