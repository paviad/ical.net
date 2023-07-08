using Antlr4.Runtime;
using Ical.Net.DataTypes;

namespace Ical.Net.NaturalLanguage;

public class AntlrParser {
    public RecurrencePattern? Parse(string rawExpression) {
        var stream = CharStreams.fromString(rawExpression.ToLower());
        var lexer = new RecurLexer(stream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new RecurParser(tokenStream);

        var context = parser.file();
        var visitor = new RecurVisitor();

        var rc = visitor.Visit(context);
        if (parser.NumberOfSyntaxErrors > 0) {
            return null;
        }

        return rc;
    }

    public static RecurrencePattern? ParseText(string s, Culture? culture = null) {
        var p = new AntlrParser();
        return p.Parse(s);
    }
}
