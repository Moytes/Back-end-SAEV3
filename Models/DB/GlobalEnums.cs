using System.ComponentModel.DataAnnotations;

namespace Models.DB;

public enum BoolStatus
{
    False = 0,
    True = 1
}

public enum Grades
{
    [Display(Name = "Primero")]
    first = 1,
    [Display(Name = "Segundo")]
    second = 2,
    [Display(Name = "Tercero")]
    third = 3,
    [Display(Name = "Cuarto")]
    fourth = 4,
    [Display(Name = "Quinto")]
    fifth = 5,
    [Display(Name = "Sexto")]
    sixth = 6
}

public enum attentionAreasCatalog
{
    [Display(Name = "Aprendizaje")]
    APRENDIZAJE = 1,
    [Display(Name = "Psicología")]
    PSICOLOGIA = 2,
    [Display(Name = "Comunicación")]
    COMUNICACION = 3,
    [Display(Name = "Trabajo Social")]
    TRABAJO_SOCIAL = 4
}