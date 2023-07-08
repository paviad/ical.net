using Ical.Net.DataTypes;

namespace Ical.Net.NaturalLanguage;

internal class RecurVisitor : RecurBaseVisitor<RecurrencePattern> {
    private static readonly string[] DayNames = {
        "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday",
    };

    private static readonly string[] DayAbbrevs = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

    private static readonly string[] MonthNames = {
        "january",
        "february",
        "march",
        "april",
        "may",
        "june",
        "july",
        "august",
        "september",
        "october",
        "november",
        "december",
    };

    private static readonly string[] MonthAbbrevs = {
        "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec",
    };

    public override RecurrencePattern VisitCount(RecurParser.CountContext context) {
        var count = int.Parse(context.NUMBER().GetText());
        var rp = new RecurrencePattern {
            Count = count,
        };
        return rp;
    }

    public override RecurrencePattern VisitDaylist(RecurParser.DaylistContext context) {
        var rp = new RecurrencePattern();
        foreach (var t in context.DAYOFWEEK()) {
            var dayInp = t.GetText();
            var day = Array.IndexOf(dayInp.Length == 3 ? DayAbbrevs : DayNames, dayInp);
            var dow = (DayOfWeek)day;
            rp.ByDay.Add(new(dow));
        }

        return rp;
    }

    public override RecurrencePattern VisitDayofmonthlist(RecurParser.DayofmonthlistContext context) {
        var rp = new RecurrencePattern();

        var ordinals = context.dayofmonthspec().Select(x => Visit(x).ByMonthDay[0]).ToList();

        rp.ByMonthDay = ordinals;

        return rp;
    }

    public override RecurrencePattern VisitDayofmonthspec(RecurParser.DayofmonthspecContext context) {
        var ordinal = ParseOrdinal(context.ORDINAL().GetText());
        if (context.LAST() is not null) {
            ordinal = -ordinal;
        }

        var rp = new RecurrencePattern {
            ByMonthDay = { ordinal },
        };

        return rp;
    }

    public override RecurrencePattern VisitDayofweeklist(RecurParser.DayofweeklistContext context) {
        var c = context.dayofweekspec();
        var rp = new RecurrencePattern();
        int ordinal = int.MinValue;
        foreach (var ce in c) {
            var rp1 = Visit(ce);
            var wd = rp1.ByDay[0];
            var nwd = new WeekDay(wd.DayOfWeek, wd.Offset == int.MinValue ? ordinal : wd.Offset);
            rp.ByDay.Add(nwd);
            if (wd.Offset != int.MinValue) {
                ordinal = wd.Offset;
            }
        }

        return rp;
    }

    public override RecurrencePattern VisitDayofweekspec_last_ordinal(
        RecurParser.Dayofweekspec_last_ordinalContext context) {
        var rp = new RecurrencePattern();

        var terminalNode = context.ORDINAL();
        var ordinal = -1;
        if (terminalNode is not null) {
            ordinal = -ParseOrdinal(terminalNode.GetText());
        }

        var dow = context.DAYOFWEEK().GetText() switch {
            "sunday" => DayOfWeek.Sunday,
            "monday" => DayOfWeek.Monday,
            "tuesday" => DayOfWeek.Tuesday,
            "wednesday" => DayOfWeek.Wednesday,
            "thursday" => DayOfWeek.Thursday,
            "friday" => DayOfWeek.Friday,
            "saturday" => DayOfWeek.Saturday,
            _ => throw new ArgumentOutOfRangeException(),
        };

        var w = new WeekDay(dow, ordinal);

        rp.ByDay = new List<WeekDay> { w };

        return rp;
    }

    public override RecurrencePattern VisitDayofweekspec_ordinal(RecurParser.Dayofweekspec_ordinalContext context) {
        var rp = new RecurrencePattern();

        var ordinal = ParseOrdinal(context.ORDINAL().GetText());

        var dayInp = context.DAYOFWEEK().GetText();
        var day = Array.IndexOf(dayInp.Length == 3 ? DayAbbrevs : DayNames, dayInp);
        var dow = (DayOfWeek)day;
        
        var w = new WeekDay(dow, ordinal);

        rp.ByDay = new List<WeekDay> { w };

        return rp;
    }

    public override RecurrencePattern VisitDayofweekspec_plain(RecurParser.Dayofweekspec_plainContext context) {
        var rp = new RecurrencePattern();

        var dayInp = context.GetText();
        var day = Array.IndexOf(dayInp.Length == 3 ? DayAbbrevs : DayNames, dayInp);
        var dow = (DayOfWeek)day;

        var w = new WeekDay(dow);

        rp.ByDay = new List<WeekDay> { w };

        return rp;
    }

    public override RecurrencePattern VisitFile(RecurParser.FileContext context) {
        var rp = Visit(context.recur());

        var countContext = context.count();
        if (countContext is not null) {
            var rp1 = Visit(countContext);
            rp.Count = rp1.Count;
        }

        var specatContext = context.specat();
        if (specatContext is not null) {
            var rp1 = Visit(specatContext);
            rp.ByHour = rp1.ByHour;
        }

        return rp;
    }

    public override RecurrencePattern VisitInterval_ordinal_dayofweek(
        RecurParser.Interval_ordinal_dayofweekContext context) {
        var ordinalContext = context.ORDINAL();
        var ordinal = 1;
        if (ordinalContext is not null) {
            ordinal = ParseOrdinal(ordinalContext.GetText());
        }

        var dayInp = context.DAYOFWEEK().GetText();
        var day = Array.IndexOf(dayInp.Length == 3 ? DayAbbrevs : DayNames, dayInp);

        var dow = (DayOfWeek)day;

        var rp = new RecurrencePattern {
            Frequency = FrequencyType.Weekly,
            Interval = ordinal,
            ByDay = new List<WeekDay> { new(dow) },
        };

        return rp;
    }

    public override RecurrencePattern VisitInterval_ordinal_month(RecurParser.Interval_ordinal_monthContext context) {
        var ordinalContext = context.ORDINAL();
        var ordinal = 1;
        if (ordinalContext is not null) {
            ordinal = ParseOrdinal(ordinalContext.GetText());
        }

        var monthInp = context.MONTHNAME().GetText();
        var month = Array.IndexOf(monthInp.Length == 3 ? MonthAbbrevs : MonthNames, monthInp) + 1;

        var rp = new RecurrencePattern {
            Frequency = FrequencyType.Yearly,
            Interval = ordinal,
            ByMonth = new List<int> { month },
        };

        return rp;
    }

    public override RecurrencePattern VisitInterval_plain(RecurParser.Interval_plainContext context) {
        var rp = new RecurrencePattern {
            Frequency = context.GetText() switch {
                "day" => FrequencyType.Daily,
                "week" => FrequencyType.Weekly,
                "month" => FrequencyType.Monthly,
                "year" => FrequencyType.Yearly,
                "hour" => FrequencyType.Hourly,
                _ => throw new ArgumentOutOfRangeException(),
            },
        };
        return rp;
    }

    public override RecurrencePattern VisitInterval_plural(RecurParser.Interval_pluralContext context) {
        var rp = new RecurrencePattern {
            Interval = int.Parse(context.NUMBER().GetText()),
            Frequency = context.PFREQ().GetText() switch {
                "days" => FrequencyType.Daily,
                "weeks" => FrequencyType.Weekly,
                "months" => FrequencyType.Monthly,
                "years" => FrequencyType.Yearly,
                "hours" => FrequencyType.Hourly,
                _ => throw new ArgumentOutOfRangeException(),
            },
        };
        return rp;
    }

    public override RecurrencePattern VisitMonthlist(RecurParser.MonthlistContext context) {
        var rp = new RecurrencePattern();
        foreach (var t in context.MONTHNAME()) {
            var monthInp = t.GetText();
            var month = Array.IndexOf(monthInp.Length == 3 ? MonthAbbrevs : MonthNames, monthInp) + 1;
            rp.ByMonth.Add(month);
        }

        return rp;
    }

    public override RecurrencePattern VisitRecur(RecurParser.RecurContext context) {
        var intervalContext = context.interval();
        var monthListContext = context.monthlist();
        var dayListContext = context.daylist();
        RecurrencePattern rp;
        if (intervalContext is not null) {
            rp = Visit(intervalContext);
        }
        else if (monthListContext is not null) {
            rp = Visit(monthListContext);
            rp.Frequency = FrequencyType.Yearly;
        }
        else if (dayListContext is not null) {
            rp = Visit(dayListContext);
            rp.Frequency = FrequencyType.Weekly;
        }
        else if (context.WEEKDAY() is not null) {
            rp = new() {
                Frequency = FrequencyType.Weekly,
                ByDay = {
                    new(DayOfWeek.Sunday),
                    new(DayOfWeek.Monday),
                    new(DayOfWeek.Tuesday),
                    new(DayOfWeek.Wednesday),
                    new(DayOfWeek.Thursday),
                },
            };
        }
        else {
            return new RecurrencePattern();
        }

        var specContext = context.specon();
        var spec = specContext is null ? null : Visit(specContext);

        if (spec is not null) {
            rp.ByDay.AddRange(spec.ByDay);
            rp.ByMonthDay.AddRange(spec.ByMonthDay);
            rp.ByHour.AddRange(spec.ByHour);
            rp.ByMinute.AddRange(spec.ByMinute);
            rp.ByMonth.AddRange(spec.ByMonth);
            rp.BySecond.AddRange(spec.BySecond);
            rp.ByWeekNo.AddRange(spec.ByWeekNo);
            rp.ByYearDay.AddRange(spec.ByYearDay);
            rp.BySetPosition.AddRange(spec.BySetPosition);
        }

        return rp;
    }

    public override RecurrencePattern VisitSpec_onmonthday(RecurParser.Spec_onmonthdayContext context) {
        var rp = Visit(context.dayofmonthlist());

        return rp;
    }

    public override RecurrencePattern VisitSpec_onweekday(RecurParser.Spec_onweekdayContext context) {
        var rp = Visit(context.dayofweeklist());

        return rp;
    }

    public override RecurrencePattern VisitSpecat(RecurParser.SpecatContext context) {
        return Visit(context.timespec());
    }

    public override RecurrencePattern VisitTimespec(RecurParser.TimespecContext context) {
        var rc = context.NUMBER().Select(z => int.Parse(z.GetText())).ToList();

        var rp = new RecurrencePattern {
            ByHour = rc,
        };

        return rp;
    }

    private static int ParseOrdinal(string ordinal) => int.Parse(ordinal[..^2]);
}
