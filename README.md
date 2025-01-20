# Implementierter Prozess und Überlegungen

### Sven
Die Herausforderung für mich war es herauszufinden, wie mit den asynchronen Calls umzugehen ist. Beispielsweise habe ich im Repository asynchrone Methoden verwendet und diese auch “awaited”. Dabei vergass ich, im Controller, also direkt an der Quelle, auf die ganze Methode zu warten. Dies konnte ich dadurch lösen, indem ich im Controller die Delete Methode awaited habe.
Weiter habe ich zuerst nicht bemerkt, dass die SaveChangesAsync() Methode die Transaktion nicht beendet. Wenn die Transaktion nicht beendet wird, werden die Änderungen auch nicht umgesetzt. Also habe ich nach der Save Methode auch noch die Transaktion committed und somit das Problem gelöst.
Das Resultat dieser beiden Änderungen war eine funktionierende Delete Methode.

### Tobi
Die Herausforderung für mich war es die Book Methode zu implementieren. Dabei gab es einige Probleme mit den Connections und Transactions. Genauer gesagt war das Problem, dass mehrfach Connections geöffnet wurden, obwohl schon eine Connection offen war. Zudem gab es Methoden im LedgerRepository.cs bei denen transactions verwendet wurden und andere ohne und deshalb wusste ich nicht welche die richtige ist. Um dieses Problem zu lösen, musste man den Ablauf verfolgen, der bei der Book Methode durchloffen wird und schauen, dass nicht mehrfach Connections erföffnet werden und die Transactions am richtigen Ort mitgegeben werden. Als Endresultat hatte ich eine funktionierende Book-Methode, welche ich anhand der Frontend-Implementierung testen konnte.

### Neil
Die grösste Herausforderung für mich war die Implementierung der API beim Lasttest, da ich anfangs nicht genau wusste, wie das funktioniert. Ich habe mich durch mehrere Dokumentationen gearbeitet, und als es schliesslich beim Login funktioniert hat, traten beim Ledger neue Probleme auf. Das machte es für mich zur grössten Herausforderung.
Zusätzlich gab es ein Merge-Problem zwischen dem Projekt, LBank_Lasttest und dem Testprojekt 1. Ich habe längere Zeit versucht, dieses selbstständig durch das Auschecken früherer Commits und erneutes Zusammenführen zu lösen. Allerdings hat das nie geklappt, sodass unser ÜK-Leiter mir schliesslich helfen musste, das Problem zu beheben. Das Endresultat war ein funktionierender Lasttest, durch den wir sicherstellen können, dass die Applikation auch unter hoher Last funktioniert.

# Tests und Testergebnisse

### Welche Tests wurden durchgeführt?
...

### Wie wurden die Tests durchgeführt?
...

### Was war das Resultat?
...
