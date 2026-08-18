using System.ComponentModel.DataAnnotations;
using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Web.Models;

/// <summary>
/// Präsentationsmodell für das Erstellen/Bearbeiten einer Aufgabe im UI-Formular.
/// </summary>
public class TaskFormModel
{
    public int? Id { get; set; }

    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Titel ist erforderlich.")]
    [StringLength(200, ErrorMessage = "Titel darf maximal 200 Zeichen lang sein.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Beschreibung darf maximal 2000 Zeichen lang sein.")]
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Open;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public bool IsEditMode => Id.HasValue;
}