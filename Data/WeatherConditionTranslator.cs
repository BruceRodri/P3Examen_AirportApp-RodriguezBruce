using Npgsql;
using P3Examen_AirportApp.Models;

namespace P3Examen_AirportApp.Data;

public sealed class WeatherConditionTranslator : INpgsqlNameTranslator
{
    public string TranslateTypeName(string clrName) => clrName;

    public string TranslateMemberName(string clrName) => clrName switch
    {
        nameof(WeatherCondition.NebelSchneefall) => "Nebel-Schneefall",
        nameof(WeatherCondition.Schneefall) => "Schneefall",
        nameof(WeatherCondition.Regen) => "Regen",
        nameof(WeatherCondition.RegenSchneefall) => "Regen-Schneefall",
        nameof(WeatherCondition.NebelRegen) => "Nebel-Regen",
        nameof(WeatherCondition.NebelRegenGewitter) => "Nebel-Regen-Gewitter",
        nameof(WeatherCondition.Gewitter) => "Gewitter",
        nameof(WeatherCondition.Nebel) => "Nebel",
        nameof(WeatherCondition.RegenGewitter) => "Regen-Gewitter",
        _ => clrName
    };
}
