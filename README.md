[English](README.en.md)

# LeiBIT für ESTWsim
Die Simulationssoftware *ESTWsim* ist ein von Thomas Bauer entwickeltes Programm, das elektronische Stellwerke (ESTW) sehr realitätsnah simuliert ([ESTWsim Homepage](https://www.estwsim.de)). Bei dem hier beschriebenen LeiBIT handelt es sich um ein Zusatzprogramm, das Zuglaufdaten, örtliche Anweisungen und andere betriebliche Informationen in Anlehnung an das "echte" LeiBIT/LeiDis System darstellt. Zur Kommunikation mit ESTWsim dient das von Bernhard Stief entwickelte Tool *ESTWonline*.

![LeiBIT Oberfläche](https://raw.githubusercontent.com/wiki/jannikbecker/leibit/img/overview_windows.png)

## Features
Das LeiBIT für ESTWsim bietet u.a. folgende Features:

- Zugfahrtinformation, eine tabellarische Darstellung aller Züge im Zulauf und deren aktuelle Position
- Bahnhofsfahrordnung für alle Bahnhöfe im Stellbereich
- Darstellung des aktuellen Zuglaufs
- Begründung von Verspätungen
- Anzeige von örtlichen Anordnungen

Folgende Simulationen werden aktuell unterstützt:

- Bereich Norddeutschland
    - Bremen Hbf
    - Rotenburg
    - Buchholz
    - Hamburg-Harburg
    - Hamburg Hbf
- Bereich Nordhessen/Thüringen
    - Heigenbrücken (NEU)
    - Aschaffenburg (NEU)
    - Großkrotzenburg (NEU)
    - Gelnhausen
    - Schlüchtern
    - Elm
    - Fulda
    - Bad Hersfeld
    - Bebra
    - Eisenach
    - Gotha

Karten der Stellbereiche können [hier](maps) eingesehen werden. Eine vollständige Bedienungsanleitung ist [hier](https://github.com/jannikbecker/leibit/wiki) zu finden. Für den ersten Start bietet die [Schnellstartanleitung](https://github.com/jannikbecker/leibit/wiki/Schnellstartanleitung) einen guten Einstiegspunkt.

## Installation
Unter [Releases](https://github.com/jannikbecker/leibit/releases/latest) findet sich die aktuellste Version. Hier kann die passende ZIP-Datei (*x86.zip* für ein 32-Bit System **oder** *x64.zip* für ein 64-Bit System) heruntergeladen werden.

![Installationsdateien](https://raw.githubusercontent.com/wiki/jannikbecker/leibit/img/github_releases.png)

Die ZIP-Datei dann an einem belieben Ort entpacken.

![ZIP-Archiv entpacken](https://raw.githubusercontent.com/wiki/jannikbecker/leibit/img/zip_extract.png)

In der ZIP-Datei ist die Datei *LeiBIT.exe* sowie *ESTWonline* und eine Schnellstartanleitung enthalten. Es ist kein weiteres Setup nötig. Die heruntergeladene Datei enthält bereits alle nötigen Komponenten.

## Beitragen und Feedback
Sämtliche Beiträge und Verbesserungen sind Herzlich Willkommen. Für jede Art von Feedback bitte eine PN oder einen Thread im [ESTWsim Forum](https://estwsim-forum.de/) starten oder ein GitHub Issue erstellen. Wer Code beitragen möchte, erstellt bitte einen Pull Request.

## Lizenz
Lizensiert unter der [Mozilla Public License Version 2.0](LICENSE).