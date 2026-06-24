using System.ComponentModel.DataAnnotations;

namespace Models.DB;

public enum BoolStatus
{
    False = 0,
    True = 1
}

public enum Sexo
{
    [Display(Name = "Mujer")]
    M = 1,
    [Display(Name = "Hombre")]
    H = 2
}

public enum Turns
{
    [Display(Name = "Matutino")]
    MATUTINO = 1,
    [Display(Name = "Vespertino")]
    VESPERTINO = 2,
    [Display(Name = "Completo")]
    COMPLETO = 3
}

public enum FinalSituation
{
    [Display(Name = "Alta")]
    ALTA = 1,
    [Display(Name = "Baja")]
    BAJA = 2,
    [Display(Name = "Seguimiento")]
    SEGUIMIENTO = 3
}

public enum Phases
{
    [Display(Name = "Inicial")]
    INICIAL = 1,
    [Display(Name = "Final")]
    FINAL = 2
}

public enum AttentionTypes
{
    [Display(Name = "Plan Individual")]
    PLAN_INDIVIDUAL = 1,
    [Display(Name = "Plan Escuela")]
    PLAN_ESCUELA = 2
}

public enum DisabilityCategory
{
    [Display(Name = "Discapacidad")]
    DISCAPACIDAD = 1,
    [Display(Name = "BAP")]
    BAP = 2,
    [Display(Name = "Aptitudes Sobresalientes")]
    AS = 3
}

public enum NotificationType
{
    [Display(Name = "Información")]
    INFO = 0,
    [Display(Name = "Advertencia")]
    WARNING = 1,
    [Display(Name = "Error")]
    ERROR = 2,
    [Display(Name = "Éxito")]
    SUCCESS = 3
}

public enum SubscriptionStatus
{
    [Display(Name = "Activa")]
    ACTIVA = 1,
    [Display(Name = "Suspendida")]
    SUSPENDIDA = 2,
    [Display(Name = "Cancelada")]
    CANCELADA = 3,
    [Display(Name = "Trial")]
    TRIAL = 4
}

public enum SubscriptionPeriod
{
    [Display(Name = "Mensual")]
    MENSUAL = 1,
    [Display(Name = "Anual")]
    ANUAL = 2
}

public enum PaymentMethodType
{
    TARJETA = 1,
    TRANSFERENCIA = 2,
    OXXO = 3,
    PAYPAL = 4,
    OTRO = 5
}

public enum PaymentProvider
{
    STRIPE = 1,
    CONEKTA = 2,
    MERCADOPAGO = 3
}

public enum TransactionType
{
    COBRO = 1,
    REEMBOLSO = 2,
    AJUSTE = 3
}

public enum TransactionStatus
{
    PENDIENTE = 1,
    PROCESANDO = 2,
    COMPLETADA = 3,
    FALLIDA = 4,
    REEMBOLSADA = 5
}

public enum InvoiceStatus
{
    EMITIDA = 1,
    CANCELADA = 2,
    PENDIENTE = 3
}
