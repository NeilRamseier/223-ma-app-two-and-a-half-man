# Implementierter Prozess und Überlegungen

### Sven
Die Herausforderung für mich war es herauszufinden, wie mit den asynchronen Calls umzugehen ist. Beispielsweise habe ich im Repository asynchrone Methoden verwendet und diese auch “awaited”. Dabei vergass ich, im Controller, also direkt an der Quelle, auf die ganze Methode zu warten. Dies konnte ich dadurch lösen, indem ich im Controller die Delete Methode awaited habe.
Weiter habe ich zuerst nicht bemerkt, dass die SaveChangesAsync() Methode die Transaktion nicht beendet. Wenn die Transaktion nicht beendet wird, werden die Änderungen auch nicht umgesetzt. Also habe ich nach der Save Methode auch noch die Transaktion committed und somit das Problem gelöst.
Das Resultat dieser beiden Änderungen war eine funktionierende Delete Methode.

### Tobi
Die Herausforderung für mich war es die Book Methode zu implementieren. Dabei gab es einige Probleme mit den Connections und Transactions. Genauer gesagt war das Problem, dass mehrfach Connections geöffnet wurden, obwohl schon eine Connection offen war. Zudem gab es Methoden im LedgerRepository.cs bei denen transactions verwendet wurden und andere ohne und deshalb wusste ich nicht welche die richtige ist. Um dieses Problem zu lösen, musste man den Ablauf verfolgen, der bei der Book Methode durchloffen wird und schauen, dass nicht mehrfach Connections erföffnet werden und die Transactions am richtigen Ort mitgegeben werden. Als Endresultat hatte ich eine funktionierende Book-Methode, welche ich anhand der Frontend-Implementierung testen konnte.

### Neil
...

# Tests und Testergebnisse

### Welche Tests wurden durchgeführt?
...

### Wie wurden die Tests durchgeführt?
...

### Was war das Resultat?
...
