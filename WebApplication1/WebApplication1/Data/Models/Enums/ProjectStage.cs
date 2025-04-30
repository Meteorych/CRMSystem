namespace WebApplication1.Data.Models.Enums;

public enum ProjectStage
{
    [Display(Name = "Не указано")]
    NotSpecified = 0,

    [Display(Name = "Техническое задание")]
    TechTask = 1,

    [Display(Name = "Сценарий")]
    Script = 2,

    [Display(Name = "Съёмка")]
    Filming = 3,

    [Display(Name = "Монтаж видео")]
    VideoEditing = 4,

    [Display(Name = "Финальный отчёт")]
    Complete = 5
}