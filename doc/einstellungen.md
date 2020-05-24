# Einstellungen

Das Fenster *Einstellungen* kann über den Menüpunkt *Datei > Einstellungen* oder über den Button ![Einstellungen Button](../Leibit.Client.WPF/Resources/Images/settings.png) geöffnet werden.

![Fenster Einstellungen](img/settings.png)

## Pfade
In dieser Rubrik werden die Pfade zu den Installationsverzeichnissen der jeweiligen ESTWsim angegeben, d.h. der Ordner, in dem die estw_sim.exe liegt. Die Pfade können entweder manuell eingegeben werden oder durch Klick auf den ... Button in einem Dialog ausgewählt werden. Zur besseren Übersichtlichkeit sind die Stellwerke nach Bereichen gruppiert, die sich auf- und zuklappen lassen.

## Programmverhalten
Durch die Einstellungen in dieser Rubrik lässt sich auf das Verhalten des LeiBIT in bestimmten Situationen Einfluss nehmen.

- **Automatische Fertigmeldung:** Im realen Leben kann der Tf seinen Zug am Startbahnhof fertig melden. Der Zug wird in der Zugfahrtinformation dann braun hinterlegt. Da es in ESTWsim keine realen Lokführer gibt, kann die Fertigmeldung automatisch erfolgen. Wenn aktiv, wird die Zugnummer ab einer konfigurierbaren Anzahl von Minuten vor der planmäßigen Abfahrt im Startbahnhof (abweichend vom Original) grün hinterlegt und der Status *fertig* angezeigt. Standard: aktiv ab 2 Minuten vor Abfahrt.
- **Verspätungsbegründung:** Zur besseren Nachvollziehbarkeit von Verspätungen müssen diese in der Realität begründet werden. Diese Funktion lässt sich im LeiBIT für ESTWsim an- oder ausschalten. Zusätzlich kann eine Plausibilitätsprüfung durchgeführt werden, die prüft, ob ein für die Verspätung als ursächlich angegebener Zug auch tatsächlich als Verursacher infrage kommt (Details siehe [Verspätungsbegründung](delay.md)). Optional besteht die Möglichkeit, die Verspätungsursache an alle per ESTWonline verbundenen Stellwerke zu übertragen, sodass die Nachbar-Fdl den Grund für die Verspätung ebenfalls sehen können. Dafür muss das LeiBIT auf demselben Rechner laufen wie ESTWsim selbst. Standard: aktiv ab 3 Verspätungsminuten mit Plausibilitätsprüfung ohne Übertragung an verbundene ESTW.
- **Kompletten Zuglauf anzeigen:** Wenn aktiv, werden im Fenster [Zuglauf](zuglauf.md) die Betriebsstellen aller verbundenen Stellwerke angezeigt. Ist diese Option deaktiv, werden nur die Betriebsstellen im eigenen Stellbezirk (gemäß [Aufschaltbereich](aufschaltbereich.md)) angezeigt. Standard: aktiv
- **Inaktive ESTW laden:** Stellwerke, von denen schon länger keine Daten mehr empfangen wurden, werden als inaktiv angesehen. Beim Laden von gespeicherten Spielständen können diese Stellwerke ausgelassen werden. Ist diese Option deaktiv, kann es zu Datenverlust kommen, da Züge, die sich zum Zeitpunkt des Speicherns in einem inaktiven Stellwerk befanden, nicht mehr geladen werden und die Zuglaufdaten somit verloren gehen. Standard: aktiv
- **ESTW inaktiv nach:** Gibt an, nach wie vielen Sekunden ein Stellwerk als inaktiv angesehen wird, wenn keine Daten mehr von diesem Stellwerk empfangen werden. Standard: 30 Sekunden

## Sonstiges
- **Pfad zu ESTWonline:** Hier ist der Ordner einzutragen, in dem die ESTWonline.exe für die Datenübertragung zum LeiBIT liegt. Dies ist **nicht** der Ordner *Kommunikation* der jeweiligen Simulation, sondern ein separater Ordner, in dem die ESTWonline.exe des Typ *Leibit* liegt. In diesem Ordner liegt i.d.R. auch eine Datei *ESTWsim_Liste.ini*.
- **Farbe der Fenster:** Hier kann die Farbe für den Rahmen der Fenster festgelegt werden. Eine Änderung dieser Einstellung wird nur für neue Fenster wirksam, d.h. bereits geöffnete Fenster behalten zunächst ihre Farbe, bis sie geschlossen und neu geöffnet werden.