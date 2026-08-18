using System.ComponentModel.DataAnnotations;
using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Web.Models;

/// <summary>
/// Präsentationsmodell für das Erstellen/Bearbeiten eines Projekts im UI-Formular.
/// </summary>
public class ProjectFormModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Name ist erforderlich.")]
    [StringLength(200, ErrorMessage = "Name darf maximal 200 Zeichen lang sein.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kunde ist erforderlich.")]
    [StringLength(200, ErrorMessage = "Kunde darf maximal 200 Zeichen lang sein.")]
    public string Client { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public bool IsEditMode => Id.HasValue;
}