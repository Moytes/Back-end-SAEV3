using System.Data;
using Dapper;

namespace Utilities.Data;

/// <summary>
/// Dapper no reconoce DateOnly/TimeOnly (tipos de .NET 6+) de forma nativa: sin esto,
/// cualquier query que devuelva una de estas columnas truena con "Object must implement
/// IConvertible" (StudentReportPdfService, etc.). Mismo manejador que en backend-reportes
/// (backend-reportes/Data/DapperDateTypeHandlers.cs) — se registra por separado aquí porque
/// cada servicio corre en su propio proceso/AppDomain.
/// </summary>
public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value) => value switch
    {
        DateOnly d => d,
        DateTime dt => DateOnly.FromDateTime(dt),
        _ => DateOnly.Parse(value.ToString()!)
    };
}

public sealed class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly? value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value is null ? DBNull.Value : value.Value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly? Parse(object value) => value switch
    {
        null or DBNull => null,
        DateOnly d => d,
        DateTime dt => DateOnly.FromDateTime(dt),
        _ => DateOnly.Parse(value.ToString()!)
    };
}

public sealed class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value.ToTimeSpan();
    }

    public override TimeOnly Parse(object value) => value switch
    {
        TimeOnly t => t,
        TimeSpan ts => TimeOnly.FromTimeSpan(ts),
        _ => TimeOnly.Parse(value.ToString()!)
    };
}

public sealed class NullableTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly?>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly? value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value is null ? DBNull.Value : (object)value.Value.ToTimeSpan();
    }

    public override TimeOnly? Parse(object value) => value switch
    {
        null or DBNull => null,
        TimeOnly t => t,
        TimeSpan ts => TimeOnly.FromTimeSpan(ts),
        _ => TimeOnly.Parse(value.ToString()!)
    };
}

public static class DapperDateTypeHandlers
{
    public static void Register()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new NullableDateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new NullableTimeOnlyTypeHandler());
    }
}
