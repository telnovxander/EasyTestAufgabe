using System.ComponentModel.DataAnnotations;

namespace EasyTestAufgabe.Web.Models;

/// <summary>
/// Präsentationsmodell für das Erfassen eines Zeiteintrags im UI-Formular.
/// </summary>
public class TimeEntryFormModel
{
    public int TaskItemId { get; set; }

    [Required(ErrorMessage = "Datum ist erforderlich.")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Range(1, 1440, ErrorMessage = "Dauer muss zwischen 1 und 1440 Minuten liegen.")]
    public int DurationMinutes { get; set; } = 30;

    [StringLength(500, ErrorMessage = "Notiz darf maximal 500 Zeichen lang sein.")]
    public string? Note { get; set; }
}