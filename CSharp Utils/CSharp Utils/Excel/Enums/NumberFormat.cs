namespace CSharp_Utils.Excel.Enums;

/// <summary>
/// Enumération représentant les formats numériques des cellules.
/// </summary>
public enum NumberFormat
{
    /// <summary>
    /// Format général (0).
    /// Exemple : 1234.5678
    /// </summary>
    General = 0,

    /// <summary>
    /// Format décimal (1).
    /// Exemple : 1234.57
    /// </summary>
    Decimal = 1,

    /// <summary>
    /// Format décimal avec 2 chiffres après la virgule (2).
    /// Exemple : 1234.57
    /// </summary>
    Decimal2 = 2,

    /// <summary>
    /// Format en milliers (3).
    /// Exemple : 1,234
    /// </summary>
    Thousands = 3,

    /// <summary>
    /// Format en milliers avec 2 chiffres après la virgule (4).
    /// Exemple : 1,234.57
    /// </summary>
    Thousands2 = 4,

    /// <summary>
    /// Format pourcentage (9).
    /// Exemple : 123457%
    /// </summary>
    Percentage = 9,

    /// <summary>
    /// Format pourcentage avec 2 chiffres après la virgule (10).
    /// Exemple : 123456.78%
    /// </summary>
    Percentage2 = 10,

    /// <summary>
    /// Format scientifique (11).
    /// Exemple : 1.23E+03
    /// </summary>
    Scientific = 11,

    /// <summary>
    /// Format fraction (12).
    /// Exemple : 1234 1/2
    /// </summary>
    Fraction1 = 12,

    /// <summary>
    /// Format fraction (13).
    /// Exemple : 1234 2/3
    /// </summary>
    Fraction2 = 13,

    /// <summary>
    /// Format date courte (14).
    /// Exemple : 14-Mar
    /// </summary>
    DateShort = 14,

    /// <summary>
    /// Format date longue (15).
    /// Exemple : 14-Mars
    /// </summary>
    DateLong = 15,

    /// <summary>
    /// Format date et heure longue (16).
    /// Exemple : 14-Mar-01 01:30 PM
    /// </summary>
    DateTimeLong = 16,

    /// <summary>
    /// Format heure 12 heures (17).
    /// Exemple : 1:30 PM
    /// </summary>
    Time12Hour = 17,

    /// <summary>
    /// Format heure 12 heures avec secondes (18).
    /// Exemple : 1:30:55 PM
    /// </summary>
    Time12HourSeconds = 18,

    /// <summary>
    /// Format heure 24 heures (19).
    /// Exemple : 13:30
    /// </summary>
    Time24Hour = 19,

    /// <summary>
    /// Format heure 24 heures avec secondes (20).
    /// Exemple : 13:30:55
    /// </summary>
    Time24HourSeconds = 20,

    /// <summary>
    /// Format date et heure (21).
    /// Exemple : 14-Mar-01 13:30
    /// </summary>
    DateTime = 21,

    /// <summary>
    /// Format date et heure avec secondes (22).
    /// Exemple : 14-Mar-01 13:30:55
    /// </summary>
    DateTimeSeconds = 22,

    /// <summary>
    /// Format milliers (37).
    /// Exemple : (1,234)
    /// </summary>
    Thousands3 = 37,

    /// <summary>
    /// Format milliers avec négatif (38).
    /// Exemple : (1,234)
    /// </summary>
    Thousands3Negative = 38,

    /// <summary>
    /// Format milliers avec négatif en rouge (39).
    /// Exemple : (1,234)
    /// </summary>
    Thousands3RedNegative = 39,

    /// <summary>
    /// Format milliers avec négatif en rouge entre parenthèses (40).
    /// Exemple : (1,234)
    /// </summary>
    Thousands3RedNegativeParentheses = 40,

    /// <summary>
    /// Format comptabilité (44).
    /// Exemple : (1,234)
    /// </summary>
    Accounting = 44,

    /// <summary>
    /// Format minutes et secondes (45).
    /// Exemple : 13:30
    /// </summary>
    TimeMinutesSeconds = 45,

    /// <summary>
    /// Format minutes et secondes avec dixièmes de seconde (46).
    /// Exemple : 13:30.5
    /// </summary>
    TimeMinutesSecondsTenths = 46,

    /// <summary>
    /// Format minutes et secondes avec centièmes de seconde (47).
    /// Exemple : 13:30.55
    /// </summary>
    TimeMinutesSecondsHundredths = 47,

    /// <summary>
    /// Format scientifique (48).
    /// Exemple : 1.23E+03
    /// </summary>
    Scientific2 = 48,

    /// <summary>
    /// Format texte (49).
    /// Exemple : 1234
    /// </summary>
    Text = 49
}
