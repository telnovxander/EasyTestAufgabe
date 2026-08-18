namespace EasyTestAufgabe.Web.Helpers;

/// <summary>
/// Kleine Formatierungs-Hilfsfunktionen für die Anzeige im UI.
/// </summary>
public static class FormatHelper
{
    public static string FormatMinutes(int totalMinutes)
    {
        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;
        return hours > 0 ? $"{hours} Std. {minutes} Min." : $"{minutes} Min.";
    }
}