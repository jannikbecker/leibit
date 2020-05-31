# Zugfahrtinformation (ZFI)

Das Fenster *Zugfahrtinformation* kann über den Menüpunkt *Fenster > Zugfahrtinformation (ZFI)* oder durch Klick auf den Button ![ZFI Button](../Leibit.Client.WPF/Resources/Images/zfi.png) geöffnet werden.

![Fenster Zugfahrtinformation](img/zfi.png)

In der Tabelle werden alle Züge angezeigt, die sich im Zulauf einer Betriebsstelle befinden. Die einzelnen Spalten zeigen die folgenden Informationen:

- **Btrst:** Kürzel der Betriebsstelle
- **ZN:** Zugnummer
- **Ankunft:** Planmäßige Ankunftszeit
- **Abfahrt:** Planmäßige Abfahrtszeit
- **+/-:** Aktuelle Verspätung des Zuges.
    - Ist der Zug mehr als 5 Minuten zu früh, wird diese Spalte hellblau hinterlegt.
    - Ist der Zug mehr als 5 Minuten zu spät, wird diese Spalte gelb hinterlegt.
    - Ist der Zug mehr als 10 Minuten zu spät, wird diese Spalte rot hinterlegt.
- **vsl. an:** Voraussichtliche Ankunftszeit
- **vsl. ab:** Voraussichtliche Abfahrtszeit
- **Vsp:** Liegen für den Zug Verspätungsinformationen vor, wird in dieser Spalte ein "J" angezeigt. Ist für den Zug eine Verspätung zu begründen, wird ein "U" angezeigt.
- **öAno:** Wenn es für den Zug örtliche Anordnungen in dem jeweiligen Bahnhof gibt, wird ein "J" angezeigt.
- **Akt. Betriebsstelle:** Name der Betriebsstelle, in der der Zug sich aktuell befindet bzw. durch die er zuletzt durchgefahren ist
- **Zugstatus:** Status des Zuges in Bezug auf die aktuelle Betriebsstelle (Erklärung der einzelnen Status s.u.)
- **Gleis:** Planmäßige Gleisnummer
- **Abw. Gleis:** Fährt ein Zug außerplanmäßig auf einem anderen Gleis, wird hier die tatsächliche Gleisnummer angezeigt.

Die angezeigten Betriebsstellen lassen sich im Fenster [Aufschaltbereich](aufschaltbereich.md) festlegen. Es werden alle Züge angezeigt, die sich aktuell im Stellbereich der eigenen oder einer verbundenen Simulation befinden und die jeweilige Betriebsstelle noch nicht durchfahren haben. Ein Zug verschwindet aus der Übersicht, sobald er die nächste Betriebsstelle erreicht hat oder den Stellbereich verlassen hat.

Durch Doppelklick auf eine Zeile kann das [Zuglauf](zuglauf.md) Fenster geöffnet werden. Wird auf die Spalte *öAno* doppelt geklickt, öffnet sich das Fenster [örtliche Anordnungen](oeano.md). Wenn eine unbegründete Verspätung vorliegt (Spalte *Vsp* = "U") kann durch Doppelklick auf das "U" das Fenster [Verspätungsbegründung](verspaetung.md) geöffnet werden.

## Zugstatus
Die Spalte *Zugstatus* kann insgesamt vier verschiedene Status annehmen:

- **an:** Dieser Status tritt nur in Bahnhöfen oder Haltepunkten auf. Er besagt, dass der Zug sich gerade in dem angegeben Bahnhof befindet und bspw. am Bahnsteig steht. Bei Abzweig- und Überleitstellen kommt der Status *an* nicht vor.
- **fertig:** Wie beim Status *an* befindet sich der Zug in einem Bahnhof. In der Realität kann ein Tf seinen Zug am Startbahnhof fertig melden. In der Simulation kann diese Fertigmeldung automatisch erfolgen. Ab einer bestimmten Anzahl von Minuten vor der planmäßigen Abfahrt wechselt der Zugstatus dann auf *fertig* und die Spalten *ZN*, *Akt. Betriebsstelle* und werden grün hinterlegt. Diese Funktion kann in den [Einstellungen](Einstellungen) aktiviert oder ausgeschaltet werden. Dort kann außerdem die Anzahl von Minuten festgelegt werden, ab der der Zugstatus auf *fertig* wechselt. Der Zugstatus *fertig* tritt nur an Startbahnhöfen auf.
- **ab:** Der Zug hat die angegebene Betriebsstelle verlassen bzw. ist dort durchgefahren und befindet sich nun auf der freien Strecke.
- **beendet:** Der Zug hat die aktuelle Betriebsstelle (Spalte *Btrst*) gerade durchfahren und befindet sich auf der freien Strecke. Sobald der Zug den nächsten Bahnhof erreicht hat, verschwindet die Zeile aus der Übersicht.