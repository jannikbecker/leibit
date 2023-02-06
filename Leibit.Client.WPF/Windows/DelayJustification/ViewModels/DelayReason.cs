﻿using System.Collections.Generic;

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

        internal static List<DelayReason> GetDBDelayReasons()
        {
            return new List<DelayReason>
            {
                new DelayReason(10, "Fahrplanerstellung"),
                new DelayReason(12, "Fehldisposition"),
                new DelayReason(13, "Vorbereitung (Betrieb)"),
                new DelayReason(14, "Anfangsverspätung bei Zügen des Netzes"),
                new DelayReason(18, "Betriebliches Personal DB"),
                new DelayReason(19, "Sonstiges Betriebsdurchführung DB Netz"),
                new DelayReason(20, "Stromversorgungsanlagen (Fahrstrom)"),
                new DelayReason(21, "Telekommunikationsanlagen"),
                new DelayReason(22, "Bauwerke"),
                new DelayReason(23, "Fahrbahn"),
                new DelayReason(24, "Bahnübergangssicherungsanlagen"),
                new DelayReason(25, "Anlagen Leit- und Sicherungstechnik"),
                new DelayReason(26, "Weichen"),
                new DelayReason(27, "Netzfahrzeuge"),
                new DelayReason(28, "Technisches Personal Netz"),
                new DelayReason(29, "Sonstiges Technik Netz"),
                new DelayReason(30, "Mängellangsamfahrstellen"),
                new DelayReason(31, "Bauarbeiten/Arbeiten"),
                new DelayReason(32, "Unregelmäßigkeiten bei Bauarbeiten/Arbeiten"),
                new DelayReason(40, "Nächster Eisenbahn-Infrastruktur-Unternehmer (EIU)"),
                new DelayReason(41, "Vorheriger Eisenbahn-Infrastruktur-Unternehmer (EIU)"),
                new DelayReason(46, "Anlagen DB Energie"),
                new DelayReason(47, "Anlagen Station&Service (S&S)"),
                new DelayReason(48, "Personal Station&Service und DB Energie"),
                new DelayReason(49, "Sonstiges Station&Service und DB Energie"),
                new DelayReason(50, "Haltezeitüberschreitung"),
                new DelayReason(51, "Antrag EVU"),
                new DelayReason(52, "Ladearbeiten"),
                new DelayReason(53, "Unregelmäßigkeiten an der Ladung"),
                new DelayReason(54, "Verkehrliche Zugvorbereitung"),
                new DelayReason(57, "Keine Meldung durch EVU"),
                new DelayReason(58, "Verkehrliches Personal EVU"),
                new DelayReason(59, "Sonstige verkehrliche Gründe EVU"),
                new DelayReason(60, "Umlauf-, Einsatzplanung"),
                new DelayReason(61, "Zugbildung durch EVU"),
                new DelayReason(62, "Reisezugwagen"),
                new DelayReason(63, "Güterwagen"),
                new DelayReason(64, "Triebfahrzeuge"),
                new DelayReason(68, "Technisches Personal EVU"),
                new DelayReason(69, "Sonstiges Fahrzeuge EVU"),
                new DelayReason(70, "Nächstes EVU"),
                new DelayReason(71, "Vorheriges EVU"),
                new DelayReason(80, "Externe Einflüsse bei externen EIU"),
                new DelayReason(81, "Anordnung NLZ - Streik"),
                new DelayReason(82, "Anordnung NLZ - Witterung"),
                new DelayReason(83, "Schmierfilm"),
                new DelayReason(84, "Behörden"),
                new DelayReason(85, "Fremdeinwirkung"),
                new DelayReason(90, "Gefährliche Ereignisse"),
                new DelayReason(91, "Zugfolge - wegen Vorrang anderer Züge"),
                new DelayReason(92, "Zugfolge - betroffener Zug war verspätet"),
                new DelayReason(93, "Wende"),
                new DelayReason(94, "Anschluss"),
                new DelayReason(95, "Flügeln"),
                new DelayReason(96, "Anordnung NLZ - Weitere Untersuchungen erforderlich"),
            };
        }

        internal static List<DelayReason> GetOEBBDelayReasons()
        {
            return new List<DelayReason>
            {
                new DelayReason(100, "Fahrplanerstellung"),
                new DelayReason(115, "Zugbildung für die Infrastruktur"),
                new DelayReason(125, "Betriebliche Abweichungen"),
                new DelayReason(180, "Personal INFRA Betrieb"),
                new DelayReason(191, "Sicherheitskontrolle durch BL - QSU"),
                new DelayReason(195, "Sonstige Betriebsführung"),
                new DelayReason(200, "Störungen an sicherungstechnischen Außenanlagen"),
                new DelayReason(201, "Störungen an Weichen"),
                new DelayReason(202, "Störungen an Zuglaufcheckpoints"),
                new DelayReason(205, "Störungen an NICHT aus BFZ fernbedienten sicherungstechnischen Innenanlagen"),
                new DelayReason(206, "Störungen an aus BFZ fernbedienten (sicherungstechnischen) Innenanlagen"),
                new DelayReason(210, "Störungen an EK-Sicherungsanlagen"),
                new DelayReason(220, "Störungen an Fernmeldeanlagen (Kommunikation)"),
                new DelayReason(225, "Störungen an IT- und Kundeninformationsanlagen"),
                new DelayReason(230, "Störungen an Oberleitungsanlagen"),
                new DelayReason(231, "Störungen an Traktionsstromanlagen"),
                new DelayReason(240, "Fahrbahnstörungen"),
                new DelayReason(250, "Mängel an Bauwerken"),
                new DelayReason(290, "Störungen sonstige Infrastrukturanlagen"),
                new DelayReason(295, "Präventive Instandsetzung"),
                new DelayReason(300, "Bauarbeiten Infrastruktur AG"),
                new DelayReason(301, "Inspektion / Wartung im Wartungsfenster laut Jahresfahrplan"),
                new DelayReason(302, "Inspektion / Wartung"),
                new DelayReason(303, "Bauarbeiten Investitionen Dritte"),
                new DelayReason(311, "Verzögerungen bei Bauarbeiten"),
                new DelayReason(312, "Störungen bei Bauarbeiten"),
                new DelayReason(320, "Dauer - Langsamfahrstelle"),
                new DelayReason(390, "Sonstiges baulicher Grund"),
                new DelayReason(420, "Verspätungen durch nachfolgenden IB DB"),
                new DelayReason(421, "Verspätungen durch nachfolgenden IB SZCZ"),
                new DelayReason(422, "Verspätungen durch nachfolgenden IB ZSR"),
                new DelayReason(423, "Verspätungen durch nachfolgenden IB MAV"),
                new DelayReason(424, "Verspätungen durch nachfolgenden IB GySEV / ROeEE"),
                new DelayReason(425, "Verspätungen durch nachfolgenden IB SZ"),
                new DelayReason(426, "Verspätungen durch nachfolgenden IB RFI"),
                new DelayReason(427, "Verspätungen durch nachfolgenden IB SBB"),
                new DelayReason(428, "Verspätungen durch nachfolgenden IB Österr. Privatbahnen"),
                new DelayReason(500, "Haltezeitüberschreitung"),
                new DelayReason(510, "Abweichungsbestellung EVU"),
                new DelayReason(520, "Ladearbeiten"),
                new DelayReason(530, "Unregelmäßigkeiten an der Ladung"),
                new DelayReason(540, "Kommerzielle Behandlung"),
                new DelayReason(585, "EVU Personal - ortsgebunden"),
                new DelayReason(590, "Sonstiger kommerzieller Grund"),
                new DelayReason(600, "Umlauf / Einsatz"),
                new DelayReason(610, "Zugbildung / Zugvorbereitung"),
                new DelayReason(680, "Personal - Triebfahrzeugführer"),
                new DelayReason(685, "Personal - fahrendes Personal"),
                new DelayReason(630, "Störungen an Güterwagen"),
                new DelayReason(640, "Störungen an Triebfahrzeugen, Triebwagen, Wendezügen und Reisezugwagen"),
                new DelayReason(720, "Verspätungen DB"),
                new DelayReason(721, "Verspätungen SZCZ"),
                new DelayReason(722, "Verspätungen ZSR"),
                new DelayReason(723, "Verspätungen MAV"),
                new DelayReason(724, "Verspätungen GySEV / ROeEE"),
                new DelayReason(725, "Verspätungen SZ"),
                new DelayReason(726, "Verspätungen RFI"),
                new DelayReason(727, "Verspätungen SBB"),
                new DelayReason(728, "Verspätungen österr. Privatbahnen"),
                new DelayReason(800, "Streik"),
                new DelayReason(810, "Behördliche Grenzbehandlung"),
                new DelayReason(820, "Fremdeinwirkung"),
                new DelayReason(821, "Personen im Gleis"),
                new DelayReason(822, "Anfahren an Brücken"),
                new DelayReason(825, "Kupferkabeldiebstahl"),
                new DelayReason(830, "Witterungseinflüsse, Naturereignisse"),
                new DelayReason(890, "Sonstige externe Gründe"),
                new DelayReason(900, "Gefährdungen, Unfälle"),
                new DelayReason(915, "Folgeverspätung aus Anschluss"),
                new DelayReason(925, "Folgeverspätung aus Gleisbelegung"),
                new DelayReason(926, "Folgeverspätungen aus Gleisbelegung (Anordnung Fdl-VL)"),
                new DelayReason(927, "Folgeverspätungen aus Gleisbelegung (Anordnung Fdl-ZL)"),
                new DelayReason(935, "Folgeverspätung aus Umlauf"),
                new DelayReason(945, "Folgeverspätungen aus EVU-Störfällen"),
                new DelayReason(950, "Weitere Untersuchung erforderlich"),
                new DelayReason(990, "Fehlberechnungen"),
                new DelayReason(991, "Zugnummernwechsel"),
                new DelayReason(992, "Fehlende Trassenanpassung"),
                new DelayReason(999, "Übungen"),
            };
        }
    }
}
