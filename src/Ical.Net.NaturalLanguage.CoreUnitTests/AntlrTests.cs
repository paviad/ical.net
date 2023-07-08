using Ical.Net.DataTypes;

namespace Ical.Net.NaturalLanguage.CoreUnitTests;

public class AntlrTests {
    [Theory]
    [InlineData("Every day", "FREQ=DAILY", "Every day at 5")]
    [InlineData("Every day at 10, 12 and 17", "FREQ=DAILY;BYHOUR=10,12,17")]
    [InlineData("Every sunday at 10, 12 and 17", "FREQ=WEEKLY;BYDAY=SU;BYHOUR=10,12,17", "Every week on Sunday at 10, 12 and 17")]
    [InlineData("Every week", "FREQ=WEEKLY", "Every week on Sunday at 5")]
    [InlineData("Every hour", "FREQ=HOURLY")]
    [InlineData("Every 4 hours", "FREQ=HOURLY;INTERVAL=4")]
    [InlineData("Every week on Tuesday", "FREQ=WEEKLY;BYDAY=TU", "Every week on Tuesday at 5")]
    [InlineData("Every week on Monday, Wednesday", "FREQ=WEEKLY;BYDAY=MO,WE", "Every week on Monday and Wednesday at 5")]
    [InlineData("Every week on Monday and Wednesday", "FREQ=WEEKLY;BYDAY=MO,WE", "Every week on Monday and Wednesday at 5")]
    [InlineData("Every weekday", "FREQ=WEEKLY;BYDAY=SU,MO,TU,WE,TH")]
    [InlineData("Every 2 weeks", "FREQ=WEEKLY;INTERVAL=2", "Every 2 weeks on Sunday at 5")]
    [InlineData("Every month", "FREQ=MONTHLY", "Every month on the 1st")]
    [InlineData("Every 6 months", "FREQ=MONTHLY;INTERVAL=6", "Every 6 months on the 1st")]
    [InlineData("Every February and August", "FREQ=YEARLY;BYMONTH=2,8", "Every February and August on the 1st")]
    [InlineData("Every year", "FREQ=YEARLY", "Every January on the 1st")]
    [InlineData("Every year on the 1st Friday", "FREQ=YEARLY;BYDAY=1FR", "Every year on the 1st Friday")]
    [InlineData("Every year on the 13th Friday", "FREQ=YEARLY;BYDAY=13FR")]
    [InlineData("Every month on the 4th", "FREQ=MONTHLY;BYMONTHDAY=4")]
    [InlineData("Every month on the 4th last", "FREQ=MONTHLY;BYMONTHDAY=-4")]
    [InlineData("Every month on the 3rd Tuesday", "FREQ=MONTHLY;BYDAY=3TU")]
    [InlineData("Every month on the 3rd last Tuesday", "FREQ=MONTHLY;BYDAY=-3TU")]
    [InlineData("Every month on the last Monday", "FREQ=MONTHLY;BYDAY=-1MO")]
    [InlineData("Every month on the 2nd last Friday", "FREQ=MONTHLY;BYDAY=-2FR")]
    [InlineData("Every week for 20 times", "FREQ=WEEKLY;COUNT=20", "Every week on Sunday at 5 for 20 times")]
    [InlineData("Every month on the 3rd sun at 5", "FREQ=MONTHLY;BYDAY=3SU;BYHOUR=5")]
    [InlineData("Every day at 0 and 12", "FREQ=DAILY;BYHOUR=0,12")]
    [InlineData("Every jan", "FREQ=YEARLY;BYMONTH=1")]
    [InlineData("Every week on sun, tue and thu at 2 and 16","FREQ=WEEKLY;BYDAY=SU,TU,TH;BYHOUR=2,16")]
    [InlineData("Every sun, tue and thu at 2 and 16", "FREQ=WEEKLY;BYDAY=SU,TU,TH;BYHOUR=2,16")]
    [InlineData("Every sun", "FREQ=WEEKLY;BYDAY=SU")]
    [InlineData("Every jan on the 3rd sun, tue and thu at 2 and 16", "FREQ=YEARLY;BYDAY=3SU,3TU,3TH;BYHOUR=2,16;BYMONTH=1")]
    public void TestAntlr(string text, string rpStr, string? returnText = null) {
        var p = new AntlrParser();
        var expected = returnText ?? text;
        var result = p.Parse(text);
        Assert.Equal(rpStr, result!.ToString());
    }
}
