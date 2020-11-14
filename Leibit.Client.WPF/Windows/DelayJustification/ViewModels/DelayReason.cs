using System.Collections.Generic;

namespace Leibit.Client.WPF.Windows.DelayJustification.ViewModels
{
    public class DelayReason
    {
        private DelayReason(int no, string text)
        {
            No = no;
            Text = text;
        }

        public int No { get; set; }
        public string Text { get; set; }

        internal static List<DelayReason> GetAllDelayReasons()
        {
            return new List<DelayReason>
            {
                new DelayReason(10, "Fahrplanerstellung (Vertrieb)"),
                new DelayReason(12, "Fehldisposition"),
                new DelayReason(13, "Vorbereitung (Betrieb)"),
                new DelayReason(14, "Anfangsverspätung bei Zügen des Netzes"),
                new DelayReason(18, "Betriebliches Personal Netz"),
                new DelayReason(19, "Sonstiges Betrieb Netz"),
                new DelayReason(20, "Oberleitungsanlagen"),
                new DelayReason(21, "Telekommunikationsanlagen"),
                new DelayReason(22, "Bauwerke"),
                new DelayReason(23, "Fahrbahn"),
                new DelayReason(24, "Bahnübergangssicherungsanlagen"),
                new DelayReason(25, "Anlagen Leit- und Sicherungstechnik"),
                new DelayReason(26, "Weichen"),
                new DelayReason(27, "Netzfahrzeuge"),
                new DelayReason(28, "Technisches Personal Netz"),
                new DelayReason(29, "Sonstiges Technik Netz"),
                new DelayReason(30, "Mängellangsamfahrstelle"),
                new DelayReason(31, "Bauarbeiten"),
                new DelayReason(32, "Unregelmäßigkeiten bei Bauarbeiten"),
                new DelayReason(40, "Nächster EIU"),
                new DelayReason(41, "Vorheriger EIU"),
                new DelayReason(46, "Anlage DB Energie"),
                new DelayReason(47, "Anlage S&S"),
                new DelayReason(48, "Personal S&S und DB Energie"),
                new DelayReason(49, "Sonstiges S&S und DB Energie"),
                new DelayReason(50, "Haltezeitüberschreitung"),
                new DelayReason(51, "Antrag EVU"),
                new DelayReason(52, "Ladearbeiten"),
                new DelayReason(53, "Unregelmäßigkeiten an der Ladung"),
                new DelayReason(54, "Verkehrliche Zugvorbereitung"),
                new DelayReason(57, "Keine Meldung durch EVU"),
                new DelayReason(58, "Verkehrliches Personal EVU"),
                new DelayReason(59, "Sonstige verkehrliche Gründe EVU"),
                new DelayReason(60, "Umlauf-Einsatzplanung"),
                new DelayReason(61, "Zugbildung durch EVU"),
                new DelayReason(62, "Reisezugwagen"),
                new DelayReason(63, "Güterwagen"),
                new DelayReason(64, "Triebfahrzeuge"),
                new DelayReason(68, "Technisches Personal EVU"),
                new DelayReason(69, "Sonstige Fahrzeuge EVU"),
                new DelayReason(70, "Nächstes EVU"),
                new DelayReason(71, "Vorheriges EVU"),
                new DelayReason(80, "Externe Einflüsse nächstes EVU"),
                new DelayReason(81, "Anordnung NLZ - Streik"),
                new DelayReason(82, "Witterung"),
                new DelayReason(83, "Schmierfilm"),
                new DelayReason(84, "Behörden"),
                new DelayReason(85, "Fremdeinwirkung"),
                new DelayReason(90, "Gefährliche Ereignisse"),
                new DelayReason(91, "Zugfolge (betroffener Zug war plan)"),
                new DelayReason(92, "Zugfolge (betroffener Zug war verspätet)"),
                new DelayReason(93, "Wende"),
                new DelayReason(94, "Anschluss"),
                new DelayReason(95, "Flügeln"),
                new DelayReason(96, "Anordnung NLZ - weitere Untersuchung erforderlich"),
            };
        }
    }
}
