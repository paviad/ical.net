namespace Ical.Net.NaturalLanguage;

public partial class RecurParser {
    public bool IsTwoOrMore(string tok) {
        return int.Parse(tok) >= 2;
    }
}
